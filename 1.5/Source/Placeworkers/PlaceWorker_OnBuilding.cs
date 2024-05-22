using System.Linq;

namespace Moyo2
{
    /// <summary>
    /// <para>Warning: Everything in vanilla assumes there's only 1 building per tile. Stuff might break</para>
    /// Remember to give the building you're giving this placeworker:<br></br>
    /// 1. The <isEdifice>false</isEdifice> field.<br></br>
    /// 2. The <altitudeLayer>BuildingOnTop</altitudeLayer> field, or other layer that's high.
    /// </summary>
    public class PlaceWorker_OnBuilding : PlaceWorker
    {
        /// <summary>
        /// <para>This works in tandem with a building with CompProperties_AffectedByFacilities.</para>
        /// This placeworker should be given to a building that has CompProperties_Facility, adding it's defName to the other building's linkableFacilities.<br />
        /// It will automatically prevent placing it anywhere except for a building that includes this one inside it's linkableFacilities.
        /// </summary>
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
#nullable enable
            Building? buildingOnCursor = loc.GetFirstBuilding(map);
#nullable disable
            // Gets CompProperties_AffectedByFacilities from the first building it finds on the tile the shadow's on 
            CompProperties_AffectedByFacilities compAffectedByFacilitiesOfBuilding = buildingOnCursor?.def?.GetCompProperties<CompProperties_AffectedByFacilities>();

            if (buildingOnCursor != null
                && compAffectedByFacilitiesOfBuilding != null // Incase the building found doesn't have CompProperties_AffectedByFacilities
                && compAffectedByFacilitiesOfBuilding.linkableFacilities.Any(building => building.defName == checkingDef.defName)
                // Incase the CompProperties_AffectedByFacilities found doesn't have this building on it's linkableFacilities list.
                && GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size).All(cell => cell.GetFirstBuilding(map) == buildingOnCursor))
            {
                return true;
            }
            return "Moyo2_Placeworker_OnBuilding_Failed".Translate();
        }
    }
}
