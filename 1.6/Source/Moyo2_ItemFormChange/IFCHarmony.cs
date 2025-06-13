using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Moyo2_ItemFormChange
{
    // Token: 0x02000002 RID: 2
    [StaticConstructorOnStartup]
    public static class IFCHarmony
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static IFCHarmony()
        {
            Harmony harmony = new Harmony("IFC.Patch");
            harmony.PatchAll();
            harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "GetGizmos", null, null), null, new HarmonyMethod(typeof(IFCHarmony), "GetExtraEquipmentGizmosPassThrough", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "EquipmentTrackerTick", null, null), null, new HarmonyMethod(typeof(IFCHarmony), "IFCPostTickEquipment", null), null, null);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x000020DC File Offset: 0x000002DC
        public static void IFCPostTickEquipment(Pawn_EquipmentTracker __instance)
        {
            List<ThingWithComps> allEquipmentListForReading = __instance.AllEquipmentListForReading;
            for (int i = 0; i < allEquipmentListForReading.Count; i++)
            {
                CompFormChange comp = allEquipmentListForReading[i].GetComp<CompFormChange>();
                if (comp != null)
                {
                    comp.CompTick();
                }
            }
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002120 File Offset: 0x00000320
        public static IEnumerable<Gizmo> GetExtraEquipmentGizmosPassThrough(IEnumerable<Gizmo> values, Pawn_EquipmentTracker __instance)
        {
            foreach (Gizmo giz in values)
            {
                yield return giz;
            }
            bool flag = __instance.pawn.IsColonistPlayerControlled && PawnAttackGizmoUtility.CanShowEquipmentGizmos();
            if (flag)
            {
                List<ThingWithComps> list = __instance.AllEquipmentListForReading;
                int num;
                for (int i = 0; i < list.Count; i = num + 1)
                {
                    ThingWithComps eq = list[i];
                    CompFormChange twg = ThingCompUtility.TryGetComp<CompFormChange>(eq);
                    bool flag2 = twg != null;
                    if (flag2)
                    {
                        foreach (Gizmo giz2 in twg.HeldGizmos(__instance.pawn))
                        {
                            yield return giz2;
                        }
                    }
                    eq = null;
                    twg = null;
                    num = i;
                }
                list = null;
            }
            yield break;
        }
    }
}
