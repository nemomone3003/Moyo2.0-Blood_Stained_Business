namespace Moyo2
{
	public class CompProperties_ApparelTurret : CompProperties
	{
		public ThingDef turretDef;
		public float angleOffset;
		public bool autoAttack = true;


		public CompProperties_ApparelTurret()
		{
			compClass = typeof(Comp_ApparelTurret);
		}


		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			bool foundRequiredNode = false;
			int foundRequiredNodes = 0;
			foreach (PawnRenderNodeProperties props in parentDef.apparel.RenderNodeProperties)
			{
				if (typeof(PawnRenderNode_ApparelTurret).IsAssignableFrom(props.nodeClass)
					&& typeof(PawnRenderNodeWorker_ApparelTurret).IsAssignableFrom(props.workerClass))
				{
					foundRequiredNode = true;
					foundRequiredNodes++;
				}
			}

			if (!foundRequiredNode)
			{
				yield return $"Required PawnRenderNode_ApparelTurret and PawnRenderNodeWorker_ApparelTurret not found in RenderNode list of {parentDef}.";
			}
			if (foundRequiredNodes > 1)
			{
				yield return $"You can only have one PawnRenderNode_ApparelTurret and PawnRenderNodeWorker_ApparelTurret per apparel.";
			}
		}
	}
}
