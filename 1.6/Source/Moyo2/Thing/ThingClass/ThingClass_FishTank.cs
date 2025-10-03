using System.Text;

namespace Moyo2
{
	public class ThingClass_FishTank : Building
	{
		// --- Variables ---

		// The settings selected in the gizmo
		private FishDef lockedFishDef;
		private FishDef wantedFishDef;

		private int ticksProgress;

		private int ticksDie;
		private CompPowerTrader compPowerTrader;

		private SimpleCurve xSizeCurve;
		private SimpleCurve ySizeCurve;

		private readonly float altitude = AltitudeLayer.BuildingOnTop.AltitudeFor();

		private ModExtension modExtension;

		// --- Properties ---

		private ModExtension ModExtension
		{
			get
			{
				modExtension ??= def.GetModExtension<ModExtension>();
				return modExtension;
			}
		}

		// If LockedFishDef is null, it will call FishDef.
		// If FishDef is null but LockedFishDef isn't, fishDef will be lockedFishDef.
		// If both are null, it will get the first item of the FishDefs list in the modextension
		public FishDef LockedFishDef
		{
			get => lockedFishDef;
			set => lockedFishDef = value;
		}


		public FishDef WantedFishDef
		{
			get => wantedFishDef ??= lockedFishDef ?? ModExtension.FishDefs[0];
			set => wantedFishDef = value;
		}


		public int TicksProgress
		{
			get => ticksProgress;
			set => ticksProgress = value == 0 ? 0 : Math.Min(value, LockedFishDef.fishTankSettings.ticksToGrow);
		}


		public int TicksDie
		{
			get => ticksDie;
			set => ticksDie = value == 0 ? 0 : Math.Min(value, LockedFishDef.fishTankSettings.ticksToDie);
		}


		public bool FishLoaded => LockedFishDef is not null;

		public bool FishFinishedGrowing => TicksProgress >= LockedFishDef?.fishTankSettings.ticksToGrow;

		// Gets CompPower if it's present on the def
		public CompPowerTrader CompPowerTrader
		{
			get
			{
				if (compPowerTrader is null && this.HasComp<CompPowerTrader>())
				{
					compPowerTrader = this.TryGetComp<CompPowerTrader>();
				}
				return compPowerTrader;
			}
		}


		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			if (FishLoaded)
			{
				RefreshCurve();
			}
		}


		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}

			Command_Action selectFish = new()
			{
				defaultLabel = "Moyo2_FishTank_GizmoLabel".Translate(),
				icon = WantedFishDef.uiIcon,
				action = delegate
				{
					List<FloatMenuOption> options = new();
					foreach (FishDef fish in ModExtension.FishDefs)
					{
						if (!IsFishResearchLocked(fish))
						{
							options.Add(new FloatMenuOption(fish.fishTankSettings.label.CapitalizeFirst(), delegate
							{
								WantedFishDef = fish;
							}));
						}
					}
					if (options.Count <= 0)
					{
						options.Add(new FloatMenuOption("Moyo2_FishTank_NoAvailableFish".Translate(), null));
					}
					Find.WindowStack.Add(new FloatMenu(options));
				},
			};
			yield return selectFish;

			if (!DebugSettings.ShowDevGizmos)
			{
				yield break;
			}

			Command_Action devGizmoLoadFish = new()
			{
				defaultLabel = "Load fish",
				action = LoadFish
			};
			yield return devGizmoLoadFish;


			Command_Action devGizmoUnloadFish = new()
			{
				defaultLabel = "Unload fish",
				action = delegate
				{
					UnloadFish();
				}
			};
			yield return devGizmoUnloadFish;


			Command_Action devGizmoFullyGrowFish = new()
			{
				defaultLabel = "Fully grow fish",
				action = delegate
				{
					TicksProgress = lockedFishDef.fishTankSettings.ticksToGrow;
				}
			};
			yield return devGizmoFullyGrowFish;


			Command_Action devGizmoReset = new()
			{
				defaultLabel = "Reset",
				action = Reset
			};
			yield return devGizmoReset;
		}


		public void LoadFish()
		{
			TicksProgress = 0;
			LockedFishDef = WantedFishDef; // Save the Fish selected so it doesn't change the result when selecting another one through the gizmo, only when running this method.
			RefreshCurve();
		}


		private void RefreshCurve()
		{
			xSizeCurve =
			[
				new (0f, 0.25f),
                // When the fish isn't at all progressed it will be 1/4 of the original X size
                new (LockedFishDef.fishTankSettings.ticksToGrow, LockedFishDef.fishTankSettings.graphicData.drawSize.x)
                // And it will get to the original X size when it reaches the ticks it needs to finish growing
            ];
			ySizeCurve =
			[
				new (0f, 0.25f),
				new (LockedFishDef.fishTankSettings.ticksToGrow, LockedFishDef.fishTankSettings.graphicData.drawSize.y)
			];
		}


		public override void TickRare()
		{
			base.TickRare();
			Log.Message(ticksProgress);
			if (FishLoaded)
			{
				// If the building has CompPowerTrader
				if (CompPowerTrader != null)
				{
					if (TicksDie >= LockedFishDef.fishTankSettings.ticksToDie) // If ticksToDie is 0 it will reset the fish, esentially killing it and having to plant it again
					{
						Messages.Message("Moyo2_FishTank_FishDied".Translate(), MessageTypeDefOf.NegativeEvent);
						Reset();
						return;
					}

					if (CompPowerTrader.PowerOn)
					{
						TicksProgress += GenTicks.TickRareInterval;
					}
					else // When the building isn't on it will DIE
					{
						TicksDie += GenTicks.TickRareInterval;
					}
				}
				// If the building doesn't have CompPowerTrader it will just grow as normal
				else if (!FishFinishedGrowing)
				{
					TicksProgress += GenTicks.TickRareInterval;
				}
			}
		}


		protected override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			if (FishLoaded)
			{
				drawLoc.y = altitude;

				int lastTick = -1;
				float Xsize;
				float Ysize;
				Vector2 prevSize = LockedFishDef.fishTankSettings.graphicData.Graphic.drawSize;

				try
				{
					if (lastTick != TicksProgress) // Every 250 ticks because TicksProgress updates every TickRare
					{
						lastTick = TicksProgress;

						Xsize = xSizeCurve.Evaluate(TicksProgress);
						Ysize = ySizeCurve.Evaluate(TicksProgress);
						// This gets the value of the X and Y curves, based on how progressed the growth is
						Vector2 vector2 = new(Xsize, Ysize);
						// With those values we change the fish' drawsize incrementally

						LockedFishDef.fishTankSettings.graphicData.Graphic.drawSize = vector2;
					}
					LockedFishDef.fishTankSettings.graphicData.Graphic.Draw(drawLoc, Rotation, this);
				}
				finally
				{
					LockedFishDef.fishTankSettings.graphicData.Graphic.drawSize = prevSize;
				}

			}
		}


		public Thing UnloadFish()
		{
			if (!FishFinishedGrowing)
			{
				Log.Warning("Tried to take out fish but it's not fully grown.");
				return null;
			}

			Thing fish = ThingMaker.MakeThing(LockedFishDef);
			Reset();
			return fish;
		}


		private void Reset()
		{
			TicksProgress = 0;
			TicksDie = 0;
			LockedFishDef = null;
		}


		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new();
			stringBuilder.Append(base.GetInspectString());
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			// Gets the text from the parent class and shows it if there's anything to show

			if (!FishLoaded)
			{
				stringBuilder.AppendLine("Moyo2_FishTank_NotGrowingAnything".Translate());
				// Not growing anything
				{

				}
			}
			else
			{
				if (FishFinishedGrowing)
				{
					stringBuilder.AppendLine("Moyo2_FishTank_FinishedGrowing".Translate(LockedFishDef.fishTankSettings.label.CapitalizeFirst().Named("FishName")));
					// Fish fully grown
				}
				if (CompPowerTrader != null && !CompPowerTrader.PowerOn)
				{
					GenDate.TicksToPeriod(lockedFishDef.fishTankSettings.ticksToDie - TicksDie, out int _, out int _, out int _, out float hours);
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("Moyo2_FishTank_HoursFishDie", hours.Named("Hours")));
					// Time left until the harvest is lost
				}
				else
				{
					stringBuilder.AppendLine("Moyo2_FishTank_NowGrowingFish".Translate(LockedFishDef.fishTankSettings.label.CapitalizeFirst().Named("FishName")));
					// Now growing: fish
					float growthPercent = TicksProgress / (float)LockedFishDef.fishTankSettings.ticksToGrow * 100f;
					stringBuilder.AppendLine("Moyo2_FishTank_GrowthPercent".Translate(growthPercent.Named("%age")));
					// progress: %
				}
			}


			if (DebugSettings.showHiddenInfo) // Only shows with godmode on
			{
				stringBuilder.AppendLine($"Is growing now? {FishLoaded}");
				stringBuilder.AppendLine($"has finished growing? {FishFinishedGrowing}");
				stringBuilder.AppendLine($"Current progress: {TicksProgress}");
				stringBuilder.AppendLine($"Ticks total: {LockedFishDef.fishTankSettings.ticksToGrow}");
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}


		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			if (!BeingTransportedOnGravship)
			{
				Reset();
			}
			base.DeSpawn(mode);
		}


		private static bool IsFishResearchLocked(FishDef fishDef)
		{
			List<ResearchProjectDef> farmResearchPrerequisites = fishDef.researchPrerequisites;
			if (farmResearchPrerequisites == null)
			{
				return false;
			}
			for (int i = 0; i < farmResearchPrerequisites.Count; i++)
			{
				if (!farmResearchPrerequisites[i].IsFinished)
				{
					return true;
				}
			}
			return false;
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref lockedFishDef, "Moyo2_FishTank_SelectedFishDef");
			Scribe_Values.Look(ref ticksProgress, "Moyo2_FishTank_tickProgress");
			Scribe_Values.Look(ref ticksDie, "Moyo2_FishTank_ticksToDie");
		}
	}
}
