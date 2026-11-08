using Verse.AI;

namespace Moyo2
{
	public class JobDriver_NacrellingDownTarget : JobDriver
	{
		private bool firstHit = true;

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
	}
}
