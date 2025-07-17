using Verse.AI;

namespace Moyo2
{
	public class WorkGiver_GrowFish : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Moyo2_ThingDefOfs.Moyo_FishTank);
		// This looks for all the fish tanks that could be worked on

		public override PathEndMode PathEndMode => PathEndMode.Touch;


		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t is not ThingClass_FishTank fishTank || fishTank.FishFinishedGrowing)
			{
				// The building given isn't a fish tank
				// or the fish tank has finished growing it's current fish
				return false;
			}
			if (fishTank.CompPowerTrader != null && !fishTank.CompPowerTrader.PowerOn)
			{
				return false;
			}
			if (pawn is null || t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			if (t.IsBurning())
			{
				return false;
			}
			if (fishTank.GrowingFish && fishTank.LockedFishDef == fishTank.FishDef)
			{
				// If the fishDef that's currently growing isn't the same as the fishDef selected in the gizmo
				// a pawn will replace it with the fish on the gizmo
				return false;
			}
			return true;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo_GrowFish, t);
		}
	}
}
