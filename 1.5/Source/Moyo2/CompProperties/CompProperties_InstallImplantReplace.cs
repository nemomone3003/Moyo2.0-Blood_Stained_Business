namespace Moyo2
{
#pragma warning disable CA1051
	public class CompProperties_InstallImplantReplace : CompProperties_UseEffectInstallImplant
	{
		public CompProperties_InstallImplantReplace()
		{
			compClass = typeof(Comp_InstallImplantReplace);
		}


		public Dictionary<HediffDef, ThingDef> incompatibleImplants;
	}
}
