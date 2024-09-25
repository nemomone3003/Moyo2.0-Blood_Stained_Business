namespace Moyo2
{
    public class PawnRenderNode_GenderedDraftedVariant : PawnRenderNode_Apparel
    {
        public new PawnRenderNodeProperties_DraftedVariant Props => (PawnRenderNodeProperties_DraftedVariant)props;

        public PawnRenderNode_GenderedDraftedVariant(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
        {
            // Gets the graphics corresponding to the undrafted variant of the apparel, appending the defname of the body to the path
            yield return GraphicDatabase.Get<Graphic_Multi>(
                Props.undraftedTexPath + pawn?.story.bodyType.defName,
                GetShader(),
                Props.drawSize,
                ColorFor(pawn));

            // Gets the graphics corresponding to the undrafted variant of the apparel, appending the defname of the body to the path
            yield return GraphicDatabase.Get<Graphic_Multi>(
                Props.draftedTexPath + pawn?.story.bodyType.defName,
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
