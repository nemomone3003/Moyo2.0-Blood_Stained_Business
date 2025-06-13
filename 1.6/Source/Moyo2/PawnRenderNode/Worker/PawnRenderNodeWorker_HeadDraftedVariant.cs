namespace Moyo2
{
    public class PawnRenderNodeWorker_HeadDraftedVariant : PawnRenderNodeWorker_Apparel_Head
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

        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 result = base.OffsetFor(node, parms, out pivot) + parms.pawn.Drawer.renderer.BaseHeadOffsetAt(parms.facing);
            if (parms.pawn.story.headType.narrow && node?.Props.narrowCrownHorizontalOffset != 0f && parms.facing.IsHorizontal)
            {
                if (parms.facing == Rot4.East)
                {
                    result.x -= node.Props.narrowCrownHorizontalOffset;
                }
                else if (parms.facing == Rot4.West)
                {
                    result.x += node.Props.narrowCrownHorizontalOffset;
                }
                result.z -= node.Props.narrowCrownHorizontalOffset;
            }
            return result;
        }
    }
}
