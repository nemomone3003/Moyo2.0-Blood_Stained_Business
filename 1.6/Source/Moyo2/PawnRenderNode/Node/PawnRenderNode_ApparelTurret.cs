namespace Moyo2
{
	public class PawnRenderNode_ApparelTurret : PawnRenderNode
	{
		public Comp_ApparelTurret turretComp;


		public PawnRenderNode_ApparelTurret(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
		{
			// Assuming this node runs when the pawn is wearing the apparel, if we look for ourselves, we should find the comp on the same apparel this node is at.
			// Won't work if more than one apparel in the pawn have different nodes of this currently
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


		public override Graphic GraphicFor(Pawn pawn)
		{
			return GraphicDatabase.Get<Graphic_Single>(turretComp.Props.turretDef.graphicData.texPath, ShaderForTurret());
		}


		private Shader ShaderForTurret()
		{
			if (props.shaderTypeDef?.Shader != null)
			{
				return props.shaderTypeDef.Shader;
			}
			return DefaultShader; // Cutout
		}
	}
}
