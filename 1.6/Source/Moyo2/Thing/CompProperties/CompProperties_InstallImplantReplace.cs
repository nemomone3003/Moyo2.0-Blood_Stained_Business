namespace Moyo2
{
	public class CompProperties_InstallImplantReplace : CompProperties_UseEffectInstallImplant
	{
		public CompProperties_InstallImplantReplace()
		{
			compClass = typeof(Comp_InstallImplantReplace);
		}


		public Dictionary<HediffDef, ThingDef> incompatibleImplants;
	}
}
