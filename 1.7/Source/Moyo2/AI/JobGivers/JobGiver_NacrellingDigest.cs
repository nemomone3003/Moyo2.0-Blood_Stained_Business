using Verse.AI;

namespace Moyo2
{
	internal class JobGiver_NacrellingDigest : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			return JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo2_NecrallingDigest, pawn.mindState.enemyTarget);
		}


		public override float GetPriority(Pawn pawn)
		{
			if (pawn.MentalStateDef == MentalStateDefOf.Manhunter)
			{
				Log.Message("In mental state");
				var found = GenClosest.ClosestThingReachable(
					pawn.Position,
					pawn.Map,
					ThingRequest.ForGroup(ThingRequestGroup.Pawn),
					PathEndMode.OnCell,
					TraverseParms.For(pawn),
					validator: (thing) => thing is Pawn p && p.RaceProps.Humanlike && p.Downed && !p.Dead && p.Spawned);

				if (found is not null)
				{
					Log.Message(found);
					pawn.mindState.enemyTarget = found;
					return 20f;
				}
			}
			if (pawn.mindState.enemyTarget is Corpse || pawn.mindState.enemyTarget is Pawn prey && prey.DeadOrDowned)
			{
				Log.Message("Enemy target: " + pawn.mindState.enemyTarget);
				return 20f;
			}
			return 0f;
		}
	}
}
