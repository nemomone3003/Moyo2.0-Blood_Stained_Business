namespace Moyo2
{
    public class PawnRenderNode_DraftedVariant : PawnRenderNode_Apparel
    {
        public new PawnRenderNodeProperties_DraftedVariant Props => (PawnRenderNodeProperties_DraftedVariant)props;

        public PawnRenderNode_DraftedVariant(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel) : base(pawn, props, tree, apparel)
        {
        }

        protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
        {
            // Gets the graphics corresponding to the undrafted variant of the apparel
            yield return GraphicDatabase.Get<Graphic_Multi>(
                Props.undraftedTexPath,
                GetShader(),
                Props.drawSize,
                ColorFor(pawn));

            // Gets the graphics corresponding to the drafted variant of the apparel
            yield return GraphicDatabase.Get<Graphic_Multi>(
                Props.draftedTexPath,
                GetShader(),
                Props.drawSize,
                ColorFor(pawn));
        }

        private Shader GetShader()
        {
            Shader shader = ShaderDatabase.Cutout;
            if (apparel.StyleDef?.graphicData.shaderType != null)
            {
                shader = apparel.StyleDef.graphicData.shaderType.Shader;
            }
            else if ((apparel.StyleDef is null && apparel.def.apparel.useWornGraphicMask) || (apparel.StyleDef is not null && apparel.StyleDef.UseWornGraphicMask))
            {
                shader = ShaderDatabase.CutoutComplex;
            }
            return shader;
        }
    }
}
