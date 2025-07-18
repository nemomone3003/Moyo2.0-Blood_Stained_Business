using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Moyo2_HPF
{
	public class JobDriver_GatherPawnResources : JobDriver
	{
		private float gatherProgress;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			HPFJobDef def = job.def as HPFJobDef;
			if (def is null)
			{
				Log.Error("def is not HPFJobDef. please use HPF.HPFJobDef instead of JobDef.");
				yield break;
			}
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnNotCasualInterruptible(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);


			Toil wait = Toils_General.Wait(15000, TargetIndex.A);
			wait.initAction = delegate
			{
				PawnUtility.ForceWait(TargetA.Pawn, 15000, maintainPosture: true);
			};
			wait.tickIntervalAction = delta =>
			{
				HPFJobDef hpfjobDef = job.def as HPFJobDef;
				pawn.skills.Learn(def.activeSkill, def.xpPerTick, false);
				gatherProgress += pawn.GetStatValue(def.activeStat, true);
				if (gatherProgress >= hpfjobDef.totalWork)
				{
					foreach (CompResourceHarvestable item in from x in (job.GetTarget(TargetIndex.A).Thing as ThingWithComps)?.GetComps<CompResourceHarvestable>()
															 where x.Props.harvestJobDef == job.def
															 select x)
					{
						item.Gathered(pawn, hpfjobDef.activeStat);
					}
					pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
				}
			};
			wait.AddFinishAction(delegate
			{
				if (TargetA.Pawn is not null &&
					(TargetA.Pawn.CurJobDef == JobDefOf.Wait_MaintainPosture || TargetA.Pawn.CurJobDef == JobDefOf.Wait))
				{
					TargetA.Pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
				}
			});
			wait.AddEndCondition(delegate
			{
				foreach (CompResourceHarvestable item in from x in (TargetA.Thing as ThingWithComps)?.GetComps<CompResourceHarvestable>()
														 where x.Props.harvestJobDef == job.def
														 select x)
				{
					if (!item.ActiveAndFull)
					{
						return JobCondition.Incompletable;
					}
				}

				return JobCondition.Ongoing;
			});

			wait.defaultCompleteMode = ToilCompleteMode.Never;
			wait.WithProgressBar(TargetIndex.A, () => gatherProgress / ((HPFJobDef)job.def).totalWork, false, -0.5f);
			wait.activeSkill = () => def.activeSkill;
			yield return wait;
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref gatherProgress, "Moyo2_HPF_gatherProgress");
		}

	}
}