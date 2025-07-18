namespace Moyo2
{
	public class PawnRenderNode_GenderedDraftedVariant : PawnRenderNode_Apparel
	{
		public new PawnRenderNodeProperties_DraftedVariant Props => (PawnRenderNodeProperties_DraftedVariant)props;

		public PawnRenderNode_GenderedDraftedVariant(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel) : base(pawn, props, tree, apparel)
		{
		}

		protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
		{
			// Gets the graphics corresponding to the undrafted variant of the apparel, appending the defname of the body to the path
			yield return GraphicDatabase.Get<Graphic_Multi>(
				$"{Props.undraftedTexPath}_{pawn?.story.bodyType.defName}",
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				ColorFor(pawn));

			// Gets the graphics corresponding to the undrafted variant of the apparel, appending the defname of the body to the path
			yield return GraphicDatabase.Get<Graphic_Multi>(
				$"{Props.undraftedTexPath}_{pawn?.story.bodyType.defName}",
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				ColorFor(pawn));
		}
	}
}
