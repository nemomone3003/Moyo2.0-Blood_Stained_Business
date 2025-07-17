namespace Moyo2
{
	public class ThingClass_MoyoFabricationBench : Building_WorkTable
	{
		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			var compAffectedByFacilities = this.TryGetComp<CompAffectedByFacilities>();

			foreach (Thing linkable in compAffectedByFacilities?.LinkedFacilitiesListForReading)
			{
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
