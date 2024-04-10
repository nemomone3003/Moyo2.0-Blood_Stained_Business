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
    public static class Moyo2_BuildingDefOfs
    {
        static Moyo2_BuildingDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Moyo2_BuildingDefOfs));
        }

        public static ThingDef Moyo_FishTank;
    }
}
