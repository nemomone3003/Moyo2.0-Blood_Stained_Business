using Verse.AI.Group;

namespace Moyo2
{
	internal class Comp_Nacrelling : ThingComp, IThingHolder
	{
		private ThingOwner<Thing> innerContainer;
		public int ticksDigesting;
		private int ticksToDigestFully;


		public CompProperties_Nacrelling Props => (CompProperties_Nacrelling)props;
		public Thing DigestingThing
		{
			get
			{
				if (innerContainer.InnerListForReading.Count <= 0)
				{
					return null;
				}
				return innerContainer.InnerListForReading[0];
			}
		}
		public Pawn DigestingPawn
		{
			get
			{
				Thing digestingThing = DigestingThing;
				if (digestingThing == null)
				{
					return null;
				}
				if (digestingThing is Corpse corpse)
				{
					return corpse.InnerPawn;
				}
				return digestingThing as Pawn;
			}
		}
		public bool Digesting => DigestingThing != null;
		public bool FinishedDigesting => ticksDigesting >= ticksToDigestFully;
		public Pawn Pawn => parent as Pawn;


		public Comp_Nacrelling()
		{
			innerContainer = new ThingOwner<Thing>(this, LookMode.Deep, removeContentsIfDestroyed: false);
		}


		public override void CompTick()
		{
			if (Digesting)
			{
				ticksDigesting++;
			}
		}

		/*
		public override void Notify_Downed()
		{
			AbortDigestion(Pawn.MapHeld);
		}


		public override void Notify_Killed(Map prevMap, DamageInfo? _ = null)
		{
			AbortDigestion(prevMap);
		}
		*/

		public override void PostSwapMap()
		{
			/* Don't know when this runs
			if (Digesting)
			{
				asd
				Pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.DevourerDigest), JobCondition.InterruptForced);
			}
			*/
		}


		public void AbortDigestion(Map map)
		{
			if (!Digesting)
			{
				return;
			}
			Thing thing = DropThing(map);

			float damageAmount = Props.timeDamageCurve.Evaluate(ticksDigesting / 60f);
			DamageInfo dinfo = new(DamageDefOf.AcidBurn, damageAmount, 0f, -1f, Pawn);
			dinfo.SetApplyAllDamage(value: true);
			thing.TakeDamage(dinfo);

			if (thing.Faction == Faction.OfPlayer)
			{
				string str = thing is Corpse ? Props.messageColonistEmergedDied : Props.messageColonistEmerged;
				if (!str.NullOrEmpty())
				{
					str = str.Formatted(thing.Named("PAWN"));
					Messages.Message(str, thing, MessageTypeDefOf.NeutralEvent);
				}
			}
		}


		public void CompleteDigestion()
		{
			if (Digesting)
			{
				Thing thing = DropThing(Pawn.MapHeld);

				if (thing is Corpse)
				{
					thing.Destroy();
				}
				else
				{
					Pawn pawn = thing as Pawn;
					if (pawn.RaceProps.Humanlike)
					{
						DamageInfo dinfo = new(DamageDefOf.AcidBurn, 300f, 0f, -1f, Pawn);
						dinfo.SetApplyAllDamage(value: true);
						pawn.TakeDamage(dinfo);
						string str = pawn.Dead ? Props.messageColonistEmergedDied : Props.messageColonistEmerged;
						if (!str.NullOrEmpty())
						{
							str = str.Formatted(pawn.Named("PAWN"));
							Messages.Message(str, pawn, MessageTypeDefOf.NeutralEvent);
						}
					}
					else
					{
						// eaten!

						if (!pawn.Destroyed)
						{
							pawn.Destroy();
						}
						else
						{
							pawn.Corpse?.Destroy();
						}
					}
				}

				foreach (var product in Props.digestionProducts)
				{
					var insProduct = ThingMaker.MakeThing(product.thingDef, product.stuff);
					insProduct.stackCount = product.count;
					GenSpawn.Spawn(insProduct, Pawn.Position, Pawn.Map, WipeMode.VanishOrMoveAside);
				}
			}
		}


		public void StartDigesting(IntVec3 origin, LocalTargetInfo target)
		{
			Thing pawnOrCorpse = target.Thing;
			if (!target.Thing.Spawned) pawnOrCorpse = (Thing)target.Thing.ParentHolder;

			if (pawnOrCorpse is not null)
			{
				pawnOrCorpse.DeSpawn();
				innerContainer.TryAdd(pawnOrCorpse);
				ticksDigesting = 0;
				ticksToDigestFully = GetDigestionTicks() - 30;

				pawnOrCorpse.Rotation = Rot4.FromAngleFlat((parent.Position - origin).AngleFlat);

				if (pawnOrCorpse is Pawn pawn)
				{
					DamageInfo dinfo = new(DamageDefOf.AcidBurn, 99f, 0f, -1f, parent);

					pawn.GetLord()?.Notify_PawnDamaged(pawn, dinfo);

					if (!Props.messageColonistSwallowed.NullOrEmpty() && pawn.Faction == Faction.OfPlayer)
					{
						Messages.Message(Props.messageColonistSwallowed.Formatted(pawn.Named("PAWN")), Pawn, MessageTypeDefOf.NegativeEvent);
					}
				}
			}
		}


		private Thing DropThing(Map map)
		{
			if (!Digesting)
			{
				return null;
			}
			if (!innerContainer.TryDrop(DigestingThing, Pawn.PositionHeld, map, ThingPlaceMode.Near, out var lastResultingThing))
			{
				if (!RCellFinder.TryFindRandomCellNearWith(Pawn.PositionHeld, c => c.Standable(map), map, out var result, 1))
				{
					Debug.LogError("Could not drop digesting pawn from nacrelling!");
					return null;
				}
				lastResultingThing = GenSpawn.Spawn(innerContainer.Take(DigestingThing), result, map);
			}
			if (lastResultingThing is Corpse corpse)
			{
				return corpse.InnerPawn;
			}
			Thing thing = lastResultingThing;
			if (thing is Pawn pawn)
				pawn.stances.stunner.StunFor(60, Pawn, addBattleLog: false, showMote: false);
			return thing;
		}


		public int GetDigestionTicks()
		{
			if (DigestingThing == null)
			{
				return 0;
			}
			return Mathf.CeilToInt(Props.bodySizeDigestTimeCurve.Evaluate(DigestingPawn.BodySize) * 60f);
		}


		public int GetDigestionTicksFor(Pawn digestee)
		{
			if (digestee == null)
			{
				return 0;
			}
			return Mathf.CeilToInt(Props.bodySizeDigestTimeCurve.Evaluate(digestee.BodySize) * 60f);
		}


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksDigesting, "ticksDigesting", 0);
			Scribe_Values.Look(ref ticksToDigestFully, "ticksToDigestFully", 0);
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			if (Scribe.mode == LoadSaveMode.PostLoadInit && innerContainer.removeContentsIfDestroyed)
			{
				innerContainer.removeContentsIfDestroyed = false;
			}
		}


		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}


		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}
	}
}
