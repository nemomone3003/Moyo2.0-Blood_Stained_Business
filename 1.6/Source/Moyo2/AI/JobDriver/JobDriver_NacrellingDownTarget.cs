using Verse.AI;

namespace Moyo2
{
	public class JobDriver_NacrellingDownTarget : JobDriver
	{
		private bool firstHit = true;
		private bool notifiedPlayerAttacking;

		private Thing Prey => pawn.mindState.enemyTarget;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			AddFinishAction(delegate
			{
				Map.attackTargetsCache.UpdateTarget(pawn);
			});

			Toil gotoPrey = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

			yield return Toils_General.DoAtomic(delegate
			{
				if (Prey is null)
					EndJobWith(JobCondition.Incompletable);
				else
					Map.attackTargetsCache.UpdateTarget(pawn);
			});

			Toil followAndAttack = Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, hitAction)
				.JumpIf(() => Prey is Corpse || Prey is Pawn pawn && pawn.DeadOrDowned, gotoPrey)
				.FailOn(() => Find.TickManager.TicksGame > startTick + 5000 && (Prey.Position - pawn.Position).LengthHorizontalSquared > 4f);
			followAndAttack.AddPreTickIntervalAction(CheckWarnPlayerInterval);
			yield return followAndAttack;
			yield return gotoPrey;


			void hitAction()
			{
				if (pawn.meleeVerbs.TryMeleeAttack(Prey, job.verbToUse, firstHit))
				{
					Map.attackTargetsCache.UpdateTarget(pawn);
					firstHit = false;
				}
			}
		}


		private void CheckWarnPlayerInterval(int delta)
		{
			if (notifiedPlayerAttacking)
			{
				return;
			}
			if (Prey is Pawn prey
				&& prey.Spawned
				&& prey.Faction == Faction.OfPlayer
				&& Find.TickManager.TicksGame > pawn.mindState.lastPredatorHuntingPlayerNotificationTick + 2500
				&& prey.Position.InHorDistOf(pawn.Position, 60f))
			{
				if (prey.RaceProps.Humanlike)
				{
					Find.LetterStack.ReceiveLetter("LetterLabelPredatorHuntingColonist".Translate(pawn.LabelShort, prey.LabelDefinite(), pawn.Named("PREDATOR"), prey.Named("PREY")).CapitalizeFirst(), "LetterPredatorHuntingColonist".Translate(pawn.LabelIndefinite(), prey.LabelDefinite(), pawn.Named("PREDATOR"), prey.Named("PREY")).CapitalizeFirst(), LetterDefOf.ThreatBig, pawn);
				}
				else
				{
					Messages.Message((prey.Name.Numerical ? "LetterPredatorHuntingColonist" : "MessagePredatorHuntingPlayerAnimal").Translate(pawn.Named("PREDATOR"), prey.Named("PREY")), pawn, MessageTypeDefOf.ThreatBig);
				}
				pawn.mindState.Notify_PredatorHuntingPlayerNotification();
				notifiedPlayerAttacking = true;
			}
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref firstHit, "firstHit", defaultValue: false);
			Scribe_Values.Look(ref notifiedPlayerAttacking, "notifiedPlayerAttacking", defaultValue: false);
		}
	}
}
