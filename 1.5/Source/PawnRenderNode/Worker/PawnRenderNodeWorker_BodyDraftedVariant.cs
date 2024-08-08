namespace Moyo2
{
    public class PawnRenderNodeWorker_BodyDraftedVariant : PawnRenderNodeWorker_Apparel_Body
    {
        protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
        {
            if (parms.pawn.Drafted)
            {
                return node?.Graphics[1];
            }
            else
            {
                return node?.Graphics[0];
            }
        }
    }
}
