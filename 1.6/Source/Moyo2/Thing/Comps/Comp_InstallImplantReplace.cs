namespace Moyo2
{
	public class Comp_InstallImplantReplace : CompUseEffect_InstallImplant
	{
		public new CompProperties_InstallImplantReplace Props => (CompProperties_InstallImplantReplace)props;


		public override void DoEffect(Pawn user)
		{
			for (int i = user.health.hediffSet.hediffs.Count - 1; i >= 0; i--)
			{
				Hediff hediff = user.health.hediffSet.hediffs[i];
				if (Props.incompatibleImplants.TryGetValue(hediff.def, out ThingDef implantThingDef))
				{
					user.health.RemoveHediff(hediff);
					GenSpawn.Spawn(implantThingDef, user.Position, user.Map);
				}
			}
			base.DoEffect(user);
		}
	}
}
