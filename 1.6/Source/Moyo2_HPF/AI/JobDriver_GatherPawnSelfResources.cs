using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Moyo2_HPF
{
	public class JobDriver_GatherPawnSelfResources : JobDriver
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
			return true;
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (ModExtension is null)
			{
				Log.Error($"Job {job.def.defName} requires Moyo2_HPF.ModExtension to work properly.");
				yield break;
			}

			Toil wait = ToilMaker.MakeToil("WaitGatherSelfResources");
			wait.WithProgressBar(TargetIndex.A, () => gatherProgress / ModExtension.totalWork);
			wait.FailOnDespawnedOrNull(TargetIndex.A);
			wait.defaultCompleteMode = ToilCompleteMode.Never;
			wait.activeSkill = () => ModExtension.activeSkill;
			wait.initAction = wait.actor.pather.StopDead;
			wait.tickIntervalAction = delta =>
			{
				Pawn actor = wait.actor;
				gatherProgress += actor.GetStatValue(ModExtension.activeStat, true) * delta;
				if (gatherProgress >= ModExtension.totalWork)
				{
					foreach (CompResourceHarvestable comp in Harvestables)
					{
						comp.Gathered(actor, ModExtension.activeStat);
					}
					actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
				}
			};
			yield return wait;
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref gatherProgress, "Moyo2_HPF_gatherProgress");
		}
	}
}