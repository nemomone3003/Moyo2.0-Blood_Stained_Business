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


		internal static Color ColorFor(Pawn pawn, PawnRenderNode node)
		{
			Color color;
			switch (node.Props.colorType)
			{
				case PawnRenderNodeProperties.AttachmentColorType.Hair:
					if (pawn.story == null)
					{
						Log.ErrorOnce("Trying to set render node color to hair for " + pawn.LabelShort + " without pawn story. Defaulting to white.", Gen.HashCombine(pawn.thingIDNumber, 828310001));
						color = Color.white;
					}
					else
					{
						color = pawn.story.HairColor;
					}
					break;
				case PawnRenderNodeProperties.AttachmentColorType.Skin:
					{
						if (pawn.Drawer.renderer.StatueColor.HasValue)
						{
							color = pawn.Drawer.renderer.StatueColor.Value;
						}
						else if (pawn.story == null)
						{
							Log.ErrorOnce("Trying to set render node color to skin for " + pawn.LabelShort + " without pawn story. Defaulting to white.", Gen.HashCombine(pawn.thingIDNumber, 228340903));
							color = Color.white;
						}
						else
						{
							color = pawn.story.SkinColor;
						}
						break;
					}
				default:
					color = node.Props.color ?? node.apparel.DrawColor;
					break;
			}
			color *= node.Props.colorRGBPostFactor;
			if (node.Props.useRottenColor && pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Rotting)
			{
				color = PawnRenderUtility.GetRottenColor(color);
			}
			return color;
		}


		internal static string GetTexPath(string texPath, Pawn pawn, PawnRenderNodeProperties_Gendered props)
		{
			return props.gendered ? $"{texPath}_{pawn?.story.bodyType.defName}" : texPath;
		}
	}
}
