using HarmonyLib;
using Verse;

namespace Moyo2_ItemFormChange
{
	[HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.EquipmentTrackerTick))]
	internal static class Pawn_EquipmentTracker_EquipmentTrackerTick_Postfix
	{
		[HarmonyPostfix]
		public static void TickCompFormChange(Pawn_EquipmentTracker __instance)
		{
			for (int i = 0; i < __instance.AllEquipmentListForReading.Count; i++)
			{
				CompFormChange compFormChange = __instance.AllEquipmentListForReading[i].GetComp<CompFormChange>();
				compFormChange?.CompTick();
			}
		}
	}
}
