using UnityEngine;
using Verse;

namespace Moyo2_ItemFormChange
{
	[StaticConstructorOnStartup]
	internal static class TextureLibrary
	{
		internal static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));
	}
}
