namespace Moyo2
{
	[HarmonyPatch(typeof(EquipmentUtility), nameof(EquipmentUtility.CanEquip), [typeof(Thing), typeof(Pawn), typeof(string), typeof(bool)], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal])]
	internal static class EquipmentUtility_CanEquip_Postfix
	{
		[HarmonyPostfix]
		internal static void PreventPawnsFromWearingGenderLockedApparel(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
		{
			var modExt = thing.def.GetModExtension<ModExtension>();

			if (modExt?.genderLockedTo is not null && modExt?.genderLockedTo != pawn.gender)
			{
				cantReason = "Moyo2_CantWearApparel_GenderLocked".Translate(pawn.gender.ToString());
				__result = false;
			}
		}
	}
}
