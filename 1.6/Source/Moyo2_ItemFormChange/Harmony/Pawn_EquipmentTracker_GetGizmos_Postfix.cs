using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Moyo2_ItemFormChange
{
	[HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.GetGizmos))]
	internal static class Pawn_EquipmentTracker_GetGizmos_Postfix
	{
		[HarmonyPostfix]
		public static IEnumerable<Gizmo> GetCompFormChangeGizmos(IEnumerable<Gizmo> gizmos, Pawn_EquipmentTracker __instance)
		{
			foreach (Gizmo gizmo in gizmos)
			{
				yield return gizmo;
			}

			if (__instance.pawn.IsColonistPlayerControlled && PawnAttackGizmoUtility.CanShowEquipmentGizmos())
			{
				for (int i = 0; i < __instance.AllEquipmentListForReading.Count; i++)
				{
					CompFormChange comp = __instance.AllEquipmentListForReading[i].TryGetComp<CompFormChange>();
					if (comp is not null)
					{
						foreach (Gizmo gizmo in comp.HeldGizmos())
						{
							yield return gizmo;
						}
					}
				}
			}
			yield break;
		}
	}
}
