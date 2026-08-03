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
		private ModExtension modExtension;


		public ModExtension ModExtension
		{
			get
			{
				modExtension ??= job.def.GetModExtension<ModExtension>();
				return modExtension;
			}
		}
		private List<CompResourceHarvestable> Harvestables => (TargetA.Thing as ThingWithComps)?.GetComps<CompResourceHarvestable>()
																.Where(comp => comp.Props.harvestJobDef == job.def && comp.ActiveAndFull).ToList();


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (ModExtension is null)
			{
				Log.Error($"Job {job.def.defName} requires Moyo2_HPF.ModExtension to work properly.");
				yield break;
			}
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnNotCasualInterruptible(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);


			Toil wait = Toils_General.Wait(15000, TargetIndex.A);
			wait.initAction = delegate
			{
				if (TargetA.Pawn is not null)
				{
					PawnUtility.ForceWait(TargetA.Pawn, 15000, maintainPosture: true);
				}
			};
			wait.tickIntervalAction = delta =>
			{
				pawn.skills?.Learn(ModExtension.activeSkill, ModExtension.xpPerTick * delta, false);
				gatherProgress += pawn.GetStatValue(ModExtension.activeStat, true) * delta;

				if (gatherProgress >= ModExtension.totalWork)
				{
					foreach (CompResourceHarvestable comp in Harvestables)
					{
						comp.Gathered(pawn, ModExtension.activeStat);
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
			/* This is just checking again? Why do I need this?
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
			*/
			wait.defaultCompleteMode = ToilCompleteMode.Never;
			wait.WithProgressBar(TargetIndex.A, () => gatherProgress / ModExtension.totalWork);
			wait.activeSkill = () => ModExtension.activeSkill;
			yield return wait;
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref gatherProgress, "Moyo2_HPF_gatherProgress");
		}

	}
}