
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
				RenderNodeUtils.GetTexPath(Props.undraftedTexPath, pawn, Props),
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				RenderNodeUtils.ColorFor(pawn, this));

			// Gets the graphics corresponding to the drafted variant of the apparel
			yield return GraphicDatabase.Get<Graphic_Multi>(
				RenderNodeUtils.GetTexPath(Props.draftedTexPath, pawn, Props),
				RenderNodeUtils.GetShader(apparel, pawn.Drawer.renderer.StatueColor.HasValue),
				Props.drawSize,
				RenderNodeUtils.ColorFor(pawn, this));
		}
	}
}
