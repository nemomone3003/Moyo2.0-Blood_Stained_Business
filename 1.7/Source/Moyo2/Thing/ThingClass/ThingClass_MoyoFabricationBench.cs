namespace Moyo2
{
	public class ThingClass_MoyoFabricationBench : Building_WorkTable
	{
		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			var compAffectedByFacilities = this.TryGetComp<CompAffectedByFacilities>();
			if (compAffectedByFacilities is null) return;

			for (int i = compAffectedByFacilities.LinkedFacilitiesListForReading.Count - 1; i >= 0; i--)
			{
				Thing linkable = compAffectedByFacilities.LinkedFacilitiesListForReading[i];
				if (!linkable.def.selectable)
				{
					if (linkable.def.Minifiable)
					{
						Thing minifiedLinkable = linkable.TryMakeMinified();
						GenSpawn.Spawn(minifiedLinkable, Position, Map);
					}
					else
					{
						linkable.Destroy(DestroyMode.Refund);
					}
				}
			}
			base.DeSpawn(mode);
		}
	}
}
