using System.Linq;
using System.Text;

namespace Moyo2
{
    public class ThingClass_FishTank : Building
    {
#nullable enable
        private ModExtension_FishTank? ModExtension => def.GetModExtension<ModExtension_FishTank>();
#nullable disable


        // --- Variables ---

        // The settings selected in the gizmo
        private FishDef lockedFishDef;
        private FishDef fishDef;

        // Fields for controlling different events from the building
        private float tickProgress;
        private bool growingFish;
        private bool finishedGrowing;

        // Fields for making the fish die when the power goes away
        private int ticksToDie = 15000;
        private CompPowerTrader compPowerTrader;

        // Field for a curve that makes the fish grow in size
        private SimpleCurve drawSizeCurve;

        // Sets the drawloc.y to building on top so the textures don't clip inside eachother
        private readonly float altitude = AltitudeLayer.BuildingOnTop.AltitudeFor();

        // --- Properties ---

        // If LockedFishDef is null, it will call FishDef.
        // If FishDef is null but LockedFishDef isn't, fishDef will be lockedFishDef.
        // If both are null, it will get the first member of the FishDefs list in the modextension
        public FishDef LockedFishDef
        {
            get
            {
                lockedFishDef ??= FishDef;
                return lockedFishDef;
            }
            set
            {
                lockedFishDef = value;
            }
        }
        public FishDef FishDef
        {
            get
            {
                if (fishDef == null)
                {
                    if (lockedFishDef != null)
                    {
                        fishDef = lockedFishDef;
                    }
                    else
                    {
                        fishDef = ModExtension.FishDefs.FirstOrDefault();
                    }
                }
                return fishDef;
            }
            set
            {
                fishDef = value;
            }
        }

        // Handles different stuff, like if the building is working, or if it's ready to harvest, to use as flags for the workgivers
        public bool GrowingFish
        {
            get
            {
                if (tickProgress > 0 && !FinishedGrowing) growingFish = true;
                if (FinishedGrowing) growingFish = false;
                return growingFish;
            }
            set
            {
                growingFish = value;
            }
        }
        public bool FinishedGrowing
        {
            get
            {
                if (tickProgress >= LockedFishDef.fishTankSettings.ticksToGrow)
                {
                    finishedGrowing = true;
                }
                return finishedGrowing; 
            }
            set
            {
                finishedGrowing = value;
            }
        }

        // Gets CompPower if it's present on the def
        public CompPowerTrader CompPowerTrader
        {
            get
            {
                if (compPowerTrader == null && this.HasComp<CompPowerTrader>())
                {
                    compPowerTrader = this.TryGetComp<CompPowerTrader>();
                }
                return compPowerTrader;
            }
        }


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            drawSizeCurve = new()
            {
                new (0f, 0.25f),
                // When the fish isn't at all progressed it will be 1/4 of the original size
                new (LockedFishDef.fishTankSettings.ticksToGrow, 1f)
                // And it will get to the original size when it reaches the ticks it needs to finish growing
            };
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            Command_Action command_Action = new()
            {
                defaultLabel = "FishTank_GizmoLabel".Translate(),
                icon = FishDef.uiIcon,
                action = delegate
                {
                    List<FloatMenuOption> options = new();
                    foreach (FishDef fish in ModExtension.FishDefs)
                    {
                        options.Add(new FloatMenuOption(fish.label.CapitalizeFirst(), delegate
                        {
                            FishDef = fish;
                        })); ;
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                },
            };
            yield return command_Action;
            // This is the gizmo to change between the fish.

            // Devmode buttons
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
            // This gizmo calls the same code the job does when planting a fish

            Command_Action devGizmoUnloadFish = new()
            {
                defaultLabel = "Unload fish",
                action = delegate
                {
                    UnloadFish();
                }
            };
            yield return devGizmoUnloadFish;
            // This gizmo calls the same code the ojb does when getting the fish

            Command_Action devGizmoReset = new()
            {
                defaultLabel = "Reset",
                action = Reset
            };
            yield return devGizmoReset;
            // This gizmo resets all variables
        }

        /// <summary>
        /// Handles telling the building to start growing the selected fishDef.
        /// </summary>
        public void LoadFish()
        {
            tickProgress = 0;
            GrowingFish = true;
            LockedFishDef = FishDef; // We save a reference to the fishDef selected, so they don't immediately change unless the pawn loads the fish again.
            drawSizeCurve = new()
            {
                new (0f, 0.25f),
                // When the fish isn't at all progressed it will be 1/4 of the original size
                new (LockedFishDef.fishTankSettings.ticksToGrow, 1f)
                // And it will get to the original size when it reaches the ticks it needs to finish growing
            };
        }

        public override void TickRare()
        {
            base.TickRare();
            // If the building has CompPowerTrader
            if (CompPowerTrader != null)
            {
                if (ticksToDie <= 0) // If ticksToDie is 0 it will reset the fish, esentially killing it and having to plant it again
                {
                    Messages.Message("FishTank_FishDied".Translate(), MessageTypeDefOf.NegativeEvent);
                    Reset();
                }
                if (GrowingFish && !FinishedGrowing)
                {
                    if (CompPowerTrader.PowerOn)
                    {
                        tickProgress += 250;
                    }
                    else // When the building isn't on it won't grow at all
                    {
                        ticksToDie -= 250;
                    }
                }
            }
            // If the building doesn't have CompPowerTrader it will just grow as normal
            else if (GrowingFish && !FinishedGrowing)
            {
                tickProgress += 250;
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (growingFish || finishedGrowing)
            {
                drawLoc.y = altitude;
                float curvePoint = drawSizeCurve.Evaluate(tickProgress);
                // This gets the value of the curve we have to change the drawsize, based on how progressed the growth is
                Vector2 vector2 = new(curvePoint, curvePoint);
                // And with that value we get a new drawsize, which is the same point twice as we want the texture to be squared

                LockedFishDef.fishTankSettings.graphicData.Graphic.drawSize = vector2;
                LockedFishDef.fishTankSettings.graphicData.Graphic.Draw(drawLoc, Rotation, this);
            }
        }

        /// <summary>
        /// Handles reseting the building when the pawn takes the fish out.
        /// </summary>
        /// <returns>A thing made from the lockedFishDef, or null if the pawn has tried to unload the fish without it being fully grown somehow</returns>
        public Thing UnloadFish()
        {
            if (!FinishedGrowing)
            {
                Log.Warning("Tried to take out fish but it's not fully grown.");
                return null;
            }

            Thing fish = ThingMaker.MakeThing(LockedFishDef);
            fish.stackCount = LockedFishDef.fishTankSettings.amountPerHarvest;
            Reset();
            return fish;
        }

        /// <summary>
        /// Sets the building's fields to their default state, as if it was freshly built
        /// </summary>
        private void Reset()
        {
            GrowingFish = false;
            FinishedGrowing = false;
            tickProgress = 0;
            ticksToDie = 15000;
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

            if (!GrowingFish)
            {
                if (!finishedGrowing)
                {
                    stringBuilder.AppendLine("FishTank_NotGrowingAnything".Translate());
                    // Not growing anything
                }
                else
                {
                    stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("FishTank_FinishedGrowing", LockedFishDef.label.CapitalizeFirst().Named("FishName")));
                    // Fish fully grown
                }
            }
            else
            {
                float growthPercent = (tickProgress / LockedFishDef.fishTankSettings.ticksToGrow) * 100;
                stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("FishTank_GrowthPercent", Mathf.Min(growthPercent, 100f).Named("%age")));
                // Fish are growing
            }
            if ((GrowingFish || finishedGrowing) && CompPowerTrader != null && !CompPowerTrader.PowerOn)
            {
                GenDate.TicksToPeriod(ticksToDie, out int _, out int _, out int _, out float hours);
                stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("FishTank_HoursFishDie", hours.Named("Hours")));
                // Time left until the harvest is lost
            }
            
            if (DebugSettings.showHiddenInfo) // Only shows with godmode on
            {
                stringBuilder.AppendLine($"Is growing now? {GrowingFish}");
                stringBuilder.AppendLine($"has finished growing? {FinishedGrowing}");
                stringBuilder.AppendLine($"Current progress: {tickProgress}");
                stringBuilder.AppendLine($"Ticks total: {LockedFishDef.fishTankSettings.ticksToGrow}");
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref lockedFishDef, "Moyo2_FishTank_SelectedFishDef");
            Scribe_Values.Look(ref tickProgress, "Moyo2_FishTank_tickProgress");
            Scribe_Values.Look(ref ticksToDie, "Moyo2_FishTank_ticksToDie");
            Scribe_Values.Look(ref growingFish, "Moyo2_FishTank_growingFish");
        }
    }
}
