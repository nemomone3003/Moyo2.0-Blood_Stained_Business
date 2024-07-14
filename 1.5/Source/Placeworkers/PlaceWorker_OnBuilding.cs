using System.Linq;

namespace Moyo2
{
    public class PlaceWorker_OnBuilding : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
#nullable enable
            Building? buildingOnCursor = loc.GetFirstBuilding(map);
#nullable disable
            // Gets CompProperties_AffectedByFacilities from the first building it finds on the tile the ghost is on 
            CompProperties_AffectedByFacilities compAffectedByFacilitiesOfBuilding = buildingOnCursor?.def?.GetCompProperties<CompProperties_AffectedByFacilities>();

            if (buildingOnCursor != null
                && compAffectedByFacilitiesOfBuilding != null // Incase the building found doesn't have CompProperties_AffectedByFacilities
                && compAffectedByFacilitiesOfBuilding.linkableFacilities.Any(building => building == checkingDef)
                // Incase the CompProperties_AffectedByFacilities found doesn't have this building on it's linkableFacilities list.
                && GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size).All(cell => cell.GetThingList(map).Any(t => t == buildingOnCursor)))
            {
                return true;
            }
            return "Moyo2_Placeworker_OnBuilding_Failed".Translate();
        }
    }
}
