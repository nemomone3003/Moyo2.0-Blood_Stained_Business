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
            // The workgiver won't give the pawn a job if

            if (t is not ThingClass_FishTank thingClass_FishTank || thingClass_FishTank.FinishedGrowing)
            {
                // The building given isn't a fish tank
                // or the fish tank has finished growing it's current fish
                return false;
            }
            if (thingClass_FishTank.CompPowerTrader != null && !thingClass_FishTank.CompPowerTrader.PowerOn)
            {
                // The fish tank has CompPowerTrader and it's turned off
                return false;
            }
            if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
            {
                // The building is forbidden for the pawn
                // or the pawn can't reserve the fish tank
                return false;
            }
            if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
            {
                // The fish tank is set to deconstruct
                return false;
            }
            if (t.IsBurning())
            {
                // The fish tank is on fire
                return false;
            }
            if (thingClass_FishTank.LockedFishDef != thingClass_FishTank.FishDef)
            {
                // SPECIAL CASE
                // If the fishDef that's currently growing isn't the same as the fishDef selected in the gizmo
                // the pawn WILL go to work to plant a new fish
                return true;
            }
            if (thingClass_FishTank.GrowingFish)
            {
                // The fish tank is growing a fish already
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo_GrowFish, t);
            // Quite literally gives the pawn the growing job
        }
    }
}
