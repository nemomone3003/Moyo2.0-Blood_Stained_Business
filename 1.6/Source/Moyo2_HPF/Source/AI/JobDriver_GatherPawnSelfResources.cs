﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Moyo2_HPF
{
	public class JobDriver_GatherPawnSelfResources : JobDriver
	{
		private float gatherProgress;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			HPFJobDef def = job.def as HPFJobDef;
			if (def is null)
			{
				Log.Error("def is not HPFJobDef. please use HPF.HPFJobDef instead of JobDef.");
				yield break;
			}


			Toil wait = ToilMaker.MakeToil("WaitGatherSelfResources");
			wait.WithProgressBar(TargetIndex.A, () => gatherProgress / ((HPFJobDef)job.def).totalWork, false, -0.5f);
			wait.FailOnDespawnedOrNull(TargetIndex.A);
			wait.defaultCompleteMode = ToilCompleteMode.Never;
			wait.activeSkill = () => def.activeSkill;
			wait.initAction = wait.actor.pather.StopDead;
			wait.tickAction = delegate ()
			{
				Pawn actor = wait.actor;
				gatherProgress += actor.GetStatValue(def.activeStat, true);
				if (gatherProgress >= ((HPFJobDef)job.def).totalWork)
				{
					foreach (CompResourceHarvestable comp in from x in actor.GetComps<CompResourceHarvestable>()
															 where x.Props.harvestJobDef == job.def
															 select x)
					{
						comp.Gathered(actor, def.activeStat);
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