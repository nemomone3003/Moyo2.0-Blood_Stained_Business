namespace Moyo2
{
	public class PawnRenderNodeWorker_BodyDraftedVariant : PawnRenderNodeWorker_Apparel_Body
	{
		protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
		{
			if (parms.pawn.Drafted)
			{
				// Graphics[1] is the call we make to the graphic database, the one that returns the drafted variant of the apparel
				return node?.Graphics[1];
			}
			else
			{
				// In the same sense, Graphics[0] is the undrafted variant
				return node?.Graphics[0];
			}
		}
	}
}
