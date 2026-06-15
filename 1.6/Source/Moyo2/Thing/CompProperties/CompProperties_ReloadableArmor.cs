
namespace Moyo2
{
    public class CompProperties_ReloadableArmor : CompProperties
    {
        public CompProperties_ReloadableArmor()
        {
            compClass = typeof(Comp_ReloadableArmor);
        }


        public ThingDef armor;
        public float valuePerArmor;
        public float? armorOverrideSharp;
        public float? armorOverrideBlunt;
        public float? armorOverrideHeat;
        public float initialDurabilityTargetValue;
        public float maxDurability;
        public string gizmoLabel;


        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }

            if (armor is null)
                yield return $"No armor to be used defined for {parentDef}";

            if (maxDurability <= 0)
                yield return $"Armor durability is 0 for {parentDef}";
        }
    }
}
