using Verse.AI;

namespace Moyo2
{
	public class JobDriver_NacrellingDigest : JobDriver
	{
		private Comp_Nacrelling comp;
		private Comp_Nacrelling Comp => comp ??= pawn.GetComp<Comp_Nacrelling>();

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			int digestionTicks = Comp.GetDigestionTicksFor(pawn.mindState.enemyTarget as Pawn);
			AddFinishAction((condition) =>
			{
				if (Comp.FinishedDigesting)
				{
					Comp.CompleteDigestion();
				}
				else
				{
					Comp.AbortDigestion(pawn.Map);
				}
				pawn.mindState.enemyTarget = null;
				Comp.ticksDigesting = 0;
			});
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell, true);
			yield return Toils_General.Do(() =>
			{
				Comp.StartDigesting(pawn.Position, TargetA);
			});
			Toil stopWhileDigesting = Toils_General.Wait(digestionTicks).WithProgressBarToilDelay(TargetIndex.None, digestionTicks);
			//stopWhileDigesting.FailOn(() => !Comp.Digesting);
			stopWhileDigesting.AddPreTickIntervalAction((delta) =>
			{
				pawn.needs.food.CurLevel += Comp.Props.nutritionPerTick * delta;
			});
			yield return stopWhileDigesting;
		}
	}
}
