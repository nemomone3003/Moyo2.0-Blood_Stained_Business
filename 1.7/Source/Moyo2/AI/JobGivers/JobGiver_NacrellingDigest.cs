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
			if (pawn.mindState.enemyTarget is Corpse || pawn.mindState.enemyTarget is Pawn prey && prey.DeadOrDowned)
			{
				return 20f;
			}
			return 0f;
		}
	}
}
