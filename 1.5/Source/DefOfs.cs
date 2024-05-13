namespace Moyo2
{
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
        public static ThingDef HadalSpire;
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
}
