using HarmonyLib;
using Verse;

namespace Moyo2_ItemFormChange
{
	[StaticConstructorOnStartup]
	public static class HarmonyInit
	{
		static HarmonyInit()
		{
			Harmony harmony = new("Moyo2.ItemFormChange");
			harmony.PatchAll();
		}
	}
}
