namespace Moyo2
{
	public class PawnRenderNode_DraftedVariant : PawnRenderNode_Apparel
	{
		public new PawnRenderNodeProperties_DraftedVariant Props => (PawnRenderNodeProperties_DraftedVariant)props;


		public PawnRenderNode_DraftedVariant(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
		{
		}


		protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
		{
			// Gets the graphics corresponding to the undrafted variant of the apparel
			yield return GraphicDatabase.Get<Graphic_Multi>(
				Props.undraftedTexPath,
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				ColorFor(pawn));

			// Gets the graphics corresponding to the drafted variant of the apparel
			yield return GraphicDatabase.Get<Graphic_Multi>(
				Props.draftedTexPath,
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				ColorFor(pawn));
		}
	}
}
