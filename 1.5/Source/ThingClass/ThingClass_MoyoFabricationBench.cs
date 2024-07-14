namespace Moyo2
{
    public class ThingClass_MoyoFabricationBench : Building_WorkTable
    {
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            var compAffectedByFacilities = this.TryGetComp<CompAffectedByFacilities>();

            if (compAffectedByFacilities is null) return;

            for (int i = 0; i < compAffectedByFacilities.LinkedFacilitiesListForReading.Count; i++)
            {
                Log.Message(i + 1);
                Thing linkable = compAffectedByFacilities.LinkedFacilitiesListForReading[i];
                if (linkable.def.selectable == false)
                {
                    if (linkable.def.Minifiable)
                    {
                        Log.Message("Minifiable");
                        Thing minifiedLinkable = linkable.TryMakeMinified();
                        GenSpawn.Spawn(minifiedLinkable, Position, Map);
                    }
                    else
                    {
                        Log.Message("Destroyed");
                        linkable.Destroy(DestroyMode.Refund);
                    }
                }
            }
            base.Destroy(mode);
        }
    }
}
