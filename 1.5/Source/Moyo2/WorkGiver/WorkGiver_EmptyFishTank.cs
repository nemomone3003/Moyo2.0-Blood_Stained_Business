using Verse.AI;

namespace Moyo2
{
	public class WorkGiver_EmptyFishTank : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Moyo2_ThingDefOfs.Moyo_FishTank);
		// This looks for all the fish tanks that could be worked on

		public override PathEndMode PathEndMode => PathEndMode.Touch;


		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			List<Thing> fishTanksOnMap = pawn?.Map.listerThings.ThingsOfDef(Moyo2_ThingDefOfs.Moyo_FishTank);
			for (int i = 0; i < fishTanksOnMap.Count; i++)
			{
				if (((ThingClass_FishTank)fishTanksOnMap[i]).FinishedGrowing)
				{
					return false; // If any of the fistTanks on the map has FinishedGrowing as true, the workgiver will NOT be skipped
				}
			}
			return true; // If it can't find any, it will be skipped
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			// The workgiver will not give a job if:
			if (t is not ThingClass_FishTank thingClass_FishTank || thingClass_FishTank.GrowingFish || !thingClass_FishTank.FinishedGrowing)
			{
				// The building found isn't a fish tank
				// or the fish tank is growing fish 
				// or the fish tank hasn't finished growing it's current fish
				return false;
			}
			// Comp forbiddable?
			if (!pawn.CanReserve(t, 1, -1, null, forced))
			{
				// The pawn can't reserve this building to work at
				return false;
			}
			if (t.IsBurning())
			{
				// The building is on fire lol
				return false;
			}
			// If none of those conditions are met, the job will be given to the pawn
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo_EmptyFishTank, t);
			// Quite literally the jobdef we give to the pawn, nothing else
		}
	}
}
