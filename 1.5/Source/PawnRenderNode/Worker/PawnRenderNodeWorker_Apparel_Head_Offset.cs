namespace Moyo2
{
    /// <summary>
    /// PawnRenderNodeWorker_Apparel_Head doesn't move hats to the same height heads are, I think vanilla hats are just drawn offset upwards
    /// </summary>
    public class PawnRenderNodeWorker_Apparel_Head_Offset : PawnRenderNodeWorker_Apparel_Head
    {
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
