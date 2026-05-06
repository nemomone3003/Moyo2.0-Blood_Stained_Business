namespace Moyo2
{
	public class PawnRenderNodeWorker_ApparelTurret : PawnRenderNodeWorker
	{
		public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
		{
			Quaternion result = base.RotationFor(node, parms);
			var nodeTurret = node as PawnRenderNode_ApparelTurret;
			result *= nodeTurret.turretComp.curRotation.ToQuat();
			return result;
		}
	}
}
