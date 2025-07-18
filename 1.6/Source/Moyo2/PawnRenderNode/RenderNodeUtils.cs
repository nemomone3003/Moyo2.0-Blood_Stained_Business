namespace Moyo2
{
	internal static class RenderNodeUtils
	{
		internal static Shader GetShader(Apparel apparel, bool forStatue)
		{
			Shader shader = ShaderDatabase.Cutout;
			if (!forStatue)
			{
				if (apparel.StyleDef?.graphicData.shaderType != null)
				{
					shader = apparel.StyleDef.graphicData.shaderType.Shader;
				}
				else if ((apparel.StyleDef == null && apparel.def.apparel.useWornGraphicMask) || (apparel.StyleDef != null && apparel.StyleDef.UseWornGraphicMask))
				{
					shader = ShaderDatabase.CutoutComplex;
				}
			}
			return shader;
		}
	}
}
