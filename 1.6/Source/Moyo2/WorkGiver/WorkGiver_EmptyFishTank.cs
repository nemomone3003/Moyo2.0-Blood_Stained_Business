using Verse.AI;

namespace Moyo2
{
	public class WorkGiver_EmptyFishTank : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Moyo2_ThingDefOfs.Moyo_FishTank);

		public override PathEndMode PathEndMode => PathEndMode.Touch;


		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			List<Thing> fishTanksOnMap = pawn?.Map?.listerThings?.ThingsOfDef(Moyo2_ThingDefOfs.Moyo_FishTank);
			for (int i = 0; i < fishTanksOnMap.Count; i++)
			{
				if (((ThingClass_FishTank)fishTanksOnMap[i]).FishFinishedGrowing)
				{
					return false; // If any tank has finished growing, doesnt skip
				}
			}
			return true;
		}


		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t is not ThingClass_FishTank thingClass_FishTank || thingClass_FishTank.GrowingFish || !thingClass_FishTank.FishFinishedGrowing)
			{
				// The building found isn't a fish tank
				// or the fish tank is growing fish 
				// or the fish tank hasn't finished growing it's current fish
				return false;
			}
			CompForbiddable compForbiddable = t.TryGetComp<CompForbiddable>();
			if (compForbiddable != null && compForbiddable.Forbidden)
			{
				return false;
			}
			if (!pawn.CanReserve(t, 1, -1, null, forced))
			{
				return false;
			}
			if (t.IsBurning())
			{
				return false;
			}
			return true;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo_EmptyFishTank, t);
		}
	}
}
