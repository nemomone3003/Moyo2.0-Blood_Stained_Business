using Verse.AI;

namespace Moyo2
{
	public class JobGiver_NacrellingDownTarget : ThinkNode_JobGiver
	{
		public bool forceScanWholeMap;


		protected override Job TryGiveJob(Pawn pawn)
		{
			Need_Food food = pawn.needs.food;
			if (food == null)
			{
				return null;
			}
			FoodPreferability foodPreferability = FoodPreferability.Undefined;
			bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;
			bool allowCorpse = true;
			FoodPreferability minPrefOverride = foodPreferability;
			if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out var foodSource, out var _, canRefillDispenser: true, canUseInventory: true, canUsePackAnimalInventory: true, allowForbidden: false, allowCorpse, allowSociallyImproper: false, pawn.IsWildMan(), forceScanWholeMap, ignoreReservations: false, calculateWantedStackCount: false, allowVenerated: false, minPrefOverride))
			{
				return null;
			}
			if (foodSource is Thing prey)
			{
				pawn.mindState.enemyTarget = prey;
				Job job = JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo2_NecrallingDownPrey, prey);
				job.killIncappedTarget = false;
				return job;
			}
			return null;
		}


		public override float GetPriority(Pawn pawn)
		{
			if (pawn.mindState.enemyTarget is Pawn prey && prey.DeadOrDowned)
			{
				return 0f; // We let the JobGiver_NacrellingDigest handle it
			}
			Need_Food food = pawn.needs.food;
			if (food == null)
			{
				return 0f;
			}
			if (!FoodUtility.ShouldBeFedBySomeone(pawn))
			{
				if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
				{
					return 9.5f;
				}
			}
			return 0f;
		}


		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_NacrellingDownTarget obj = (JobGiver_NacrellingDownTarget)base.DeepCopy(resolve);
			obj.forceScanWholeMap = forceScanWholeMap;
			return obj;
		}
	}
}
