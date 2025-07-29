namespace Moyo2
{
	public class PawnRenderNode_ApparelCustomPath : PawnRenderNode_Apparel
	{
		public PawnRenderNode_ApparelCustomPath(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
		{
		}


		protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
		{
			yield return GraphicDatabase.Get<Graphic_Multi>(
				Props.texPath,
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				ColorFor(pawn));
		}
	}
}
