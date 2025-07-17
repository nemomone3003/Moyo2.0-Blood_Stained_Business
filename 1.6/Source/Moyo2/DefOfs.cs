namespace Moyo2
{
#pragma warning disable CA2211
	[DefOf]
	public static class Moyo2_JobDefOfs
	{
		static Moyo2_JobDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(Moyo2_JobDefOfs));
		}

		public static JobDef Moyo_GrowFish;
		public static JobDef Moyo_EmptyFishTank;
	}


	[DefOf]
	public static class Moyo2_ThingDefOfs
	{
		static Moyo2_ThingDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(Moyo2_ThingDefOfs));
		}

		public static ThingDef Moyo_FishTank;
		public static ThingDef Moyo2_HadalSpire;
		public static ThingDef Moyo2_DeepDrill;
	}


	[DefOf]
	public static class Moyo2_RaceDefOf
	{
		static Moyo2_RaceDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(Moyo2_RaceDefOf));
		}

		public static AlienRace.ThingDef_AlienRace Alien_Moyo;
	}


	[DefOf]
	public static class Moyo2_HarmonySettingsDefOf
	{
		static Moyo2_HarmonySettingsDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(Moyo2_HarmonySettingsDefOf));
		}

		public static HarmonySettings Moyo2_HarmonySettings;
	}
}
