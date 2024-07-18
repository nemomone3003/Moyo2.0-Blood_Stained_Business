using System.Linq;

namespace Moyo2
{
    public class PlaceWorker_OnBuilding : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            Building buildingOnCursor = loc.GetEdifice(map); // Only 1 edifice can exist on a tile, saves myself needing to find every thing on the tile

            // Gets CompProperties_AffectedByFacilities from the first building it finds on the tile the ghost is on 
            CompProperties_AffectedByFacilities compAffectedByFacilities = buildingOnCursor?.def.GetCompProperties<CompProperties_AffectedByFacilities>();

            if (buildingOnCursor is not null
                && compAffectedByFacilities is not null // Incase the building found doesn't have CompProperties_AffectedByFacilities
                && compAffectedByFacilities.linkableFacilities.Any(buildingDef => buildingDef == checkingDef)
                // The building must have the checkingDef (the thing this placeworker is on) in the linkableFacilities list
                && GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size).All(cell => cell.GetThingList(map).Any(t => t == buildingOnCursor)))
                // And the ghost of the building we're trying to place must be built fully on the building on our cursor
            {
                return true;
            }
            return "Moyo2_Placeworker_OnBuilding_Failed".Translate();
        }
    }
}
