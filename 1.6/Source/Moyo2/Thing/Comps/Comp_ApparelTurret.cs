using Verse.AI;

namespace Moyo2
{
	public class Comp_ApparelTurret : ThingComp, IAttackTargetSearcher
	{
		private const int StartShootIntervalTicks = 10;
		private const float IdleTurnDegreesPerTick = 0.26f;
		private const int IdleTurnDuration = 140;
		private const int IdleTurnIntervalMin = 150;
		private const int IdleTurnIntervalMax = 350;

		private static readonly CachedTexture ToggleTurretIcon = new("UI/Gizmos/ToggleTurret");


		private int ticksUntilIdleTurn;
		private bool idleTurnClockwise;
		private int idleTurnTicksLeft;
		private bool fireAtWill = true;
		private LocalTargetInfo lastAttackedTarget = LocalTargetInfo.Invalid;
		private int lastAttackTargetTick;
		private bool updatedGunVerbs = false;
		protected int burstCooldownTicksLeft;
		protected int burstWarmupTicksLeft;
		protected LocalTargetInfo currentTarget = LocalTargetInfo.Invalid;
		public Thing gun;
		public float curRotation;

		private Pawn Wearer => (parent as Apparel).Wearer;
		public Thing Thing => Wearer;
		public CompProperties_ApparelTurret Props => (CompProperties_ApparelTurret)props;
		public Verb CurrentEffectiveVerb => AttackVerb;
		public LocalTargetInfo LastAttackedTarget => lastAttackedTarget;
		public int LastAttackTargetTick => lastAttackTargetTick;
		public CompEquippable GunCompEq => gun.TryGetComp<CompEquippable>();
		public Verb AttackVerb => GunCompEq.PrimaryVerb;
		private bool WarmingUp => burstWarmupTicksLeft > 0;
		public bool AutoAttack => Props.autoAttack;
		public LocalTargetInfo CurrentTarget => currentTarget;


		private bool CanShoot
		{
			get
			{
				if (!Wearer.Spawned || Wearer.Downed || Wearer.Dead || !Wearer.Awake())
				{
					Log.Message("despawned or downed or dead or asleep");
					return false;
				}
				if (Wearer.stances.stunner.Stunned)
				{
					Log.Message("Stunned");
					return false;
				}
				if (!fireAtWill)
				{
					Log.Message("No shoot");
					return false;
				}
				return true;
			}
		}


		public override void PostPostMake()
		{
			base.PostPostMake();
			MakeGun();
		}


		private void MakeGun()
		{
			gun = ThingMaker.MakeThing(Props.turretDef);
			UpdateGunVerbs();
		}


		private void UpdateGunVerbs()
		{
			List<Verb> allVerbs = gun.TryGetComp<CompEquippable>().AllVerbs;
			for (int i = 0; i < allVerbs.Count; i++)
			{
				Verb verb = allVerbs[i];
				verb.caster = Wearer;
				verb.castCompleteCallback = delegate
				{
					burstCooldownTicksLeft = AttackVerb.verbProps.defaultCooldownTime.SecondsToTicks();
				};
			}
		}


		public override void CompTick()
		{
			if (Wearer is null) return;

			if (!updatedGunVerbs)
			{
				updatedGunVerbs = true;
				UpdateGunVerbs();
			}


			if (currentTarget.IsValid)
			{
				curRotation = (currentTarget.Cell.ToVector3Shifted() - Wearer.DrawPos).AngleFlat() + Props.angleOffset;
			}
			if (!CanShoot)
			{
				return;
			}
			AttackVerb.VerbTick();
			if (AttackVerb.state == VerbState.Bursting)
			{
				return;
			}
			if (WarmingUp)
			{
				burstWarmupTicksLeft--;
				if (burstWarmupTicksLeft == 0)
				{
					Wearer.Drawer.renderer.renderTree.SetDirty();
					AttackVerb.TryStartCastOn(currentTarget, surpriseAttack: false, canHitNonTargetPawns: true, preventFriendlyFire: false, nonInterruptingSelfCast: true);
					lastAttackTargetTick = Find.TickManager.TicksGame;
					lastAttackedTarget = currentTarget;
				}
				return;
			}
			if (burstCooldownTicksLeft > 0)
			{
				burstCooldownTicksLeft--;
			}
			if (burstCooldownTicksLeft <= 0 && Wearer.IsHashIntervalTick(StartShootIntervalTicks))
			{
				currentTarget = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(this, TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable);
				//currentTarget = (Thing)AttackTargetFinder.BestAttackTarget(this, TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable, minDist: 0f, maxDist: AttackVerb.EffectiveRange);
				if (currentTarget.IsValid)
				{
					burstWarmupTicksLeft = 1;
				}
				else
				{
					ResetCurrentTarget();
				}
			}
			IdleTurretRotationTick();
		}


		private void IdleTurretRotationTick()
		{
			if (ticksUntilIdleTurn > 0)
			{
				ticksUntilIdleTurn--;
				if (ticksUntilIdleTurn == 0)
				{
					idleTurnClockwise = Rand.Value < 0.5f;
					idleTurnTicksLeft = IdleTurnDuration;
				}
			}
			else
			{
				if (idleTurnClockwise)
				{
					curRotation += IdleTurnDegreesPerTick;
				}
				else
				{
					curRotation -= IdleTurnDegreesPerTick;
				}
				idleTurnTicksLeft--;
				if (idleTurnTicksLeft <= 0)
				{
					ticksUntilIdleTurn = Rand.RangeInclusive(IdleTurnIntervalMin, IdleTurnIntervalMax);
				}
			}
		}


		private void ResetCurrentTarget()
		{
			//Wearer.Drawer.renderer.renderTree.SetDirty();
			currentTarget = LocalTargetInfo.Invalid;
			burstWarmupTicksLeft = 0;
		}


		public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
		{
			foreach (Gizmo item in base.CompGetWornGizmosExtra())
			{
				yield return item;
			}
			Command_Toggle command_Toggle = new()
			{
				defaultLabel = "CommandToggleTurret".Translate(),
				defaultDesc = "CommandToggleTurretDesc".Translate(),
				isActive = () => fireAtWill,
				icon = ToggleTurretIcon.Texture,
				toggleAction = delegate
				{
					fireAtWill = !fireAtWill;
				}
			};
			yield return command_Toggle;
		}


		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			if (Props.turretDef != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.PawnCombat, "Turret".Translate(), Props.turretDef.LabelCap, "Stat_Thing_TurretDesc".Translate(), 5600, null, Gen.YieldSingle(new Dialog_InfoCard.Hyperlink(Props.turretDef)));
			}
		}


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref burstCooldownTicksLeft, "burstCooldownTicksLeft", 0);
			Scribe_Values.Look(ref burstWarmupTicksLeft, "burstWarmupTicksLeft", 0);
			Scribe_TargetInfo.Look(ref currentTarget, "currentTarget");
			Scribe_Deep.Look(ref gun, "gun");
			Scribe_Values.Look(ref fireAtWill, "fireAtWill", defaultValue: true);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (gun == null)
				{
					Log.Error("CompTurrentGun had null gun after loading. Recreating.");
					MakeGun();
				}
			}
		}
	}
}
