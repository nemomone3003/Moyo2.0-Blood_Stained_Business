namespace Moyo2
{
	public class PawnRenderNodeWorker_ApparelTurret : PawnRenderNodeWorker
	{
		private Comp_ApparelTurret turretComp;


		public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
		{
			GetTurretComp(parms.pawn);

			return turretComp is not null &&
				(parms.pawn.Drafted || turretComp.CurrentTarget != LocalTargetInfo.Invalid) && base.CanDrawNow(node, parms);
		}


		public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
		{
			Quaternion result = base.RotationFor(node, parms);
			var nodeTurret = node as PawnRenderNode_ApparelTurret;
			result *= nodeTurret.turretComp.curRotation.ToQuat();
			return result;
		}


		private void GetTurretComp(Pawn pawn)
		{
			if (turretComp is not null) return;


			foreach (Apparel apparel in pawn.apparel.WornApparel)
			{
				if (apparel.def.apparel.RenderNodeProperties.NullOrEmpty()) continue;

				foreach (var nodeProps in apparel.def.apparel.RenderNodeProperties)
				{
					if (typeof(PawnRenderNode_ApparelTurret).IsAssignableFrom(nodeProps.nodeClass))
					{
						turretComp = apparel.GetComp<Comp_ApparelTurret>();
					}
				}
			}
		}
	}
}
