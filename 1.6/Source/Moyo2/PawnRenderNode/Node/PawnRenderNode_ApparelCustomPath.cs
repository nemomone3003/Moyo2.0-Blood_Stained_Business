namespace Moyo2
{
	public class PawnRenderNode_ApparelCustomPath : PawnRenderNode_Apparel
	{
		public new PawnRenderNodeProperties_Gendered Props => (PawnRenderNodeProperties_Gendered)props;


		public PawnRenderNode_ApparelCustomPath(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
		{
		}


		protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
		{
			yield return GraphicDatabase.Get<Graphic_Multi>(
				RenderNodeUtils.GetTexPath(Props.texPath, pawn, Props),
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				RenderNodeUtils.ColorFor(pawn, this));
		}
	}
}
