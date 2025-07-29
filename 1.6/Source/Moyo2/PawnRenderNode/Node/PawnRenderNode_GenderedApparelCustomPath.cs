namespace Moyo2
{
	public class PawnRenderNode_GenderedApparelCustomPath : PawnRenderNode_Apparel
	{
		public PawnRenderNode_GenderedApparelCustomPath(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
		{
		}

		protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
		{
			yield return GraphicDatabase.Get<Graphic_Multi>(
				$"{Props.texPath}_{pawn?.story.bodyType.defName}",
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				ColorFor(pawn));
		}
	}
}
