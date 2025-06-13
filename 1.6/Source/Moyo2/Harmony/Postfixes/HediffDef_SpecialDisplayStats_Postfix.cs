using System.Linq;
using System.Text;

namespace Moyo2
{
    /// <summary>
    /// HediffDef.SpecialDisplayStats is hardcoded to only show the stats if there's 1 stage.
    /// This shows the stats of all the stages the hediffs marked with our modExtension.
    /// </summary>
    [HarmonyPatch(typeof(HediffDef), nameof(HediffDef.SpecialDisplayStats))]
    internal static class HediffDef_SpecialDisplayStats_Postfix
    {

        [HarmonyPostfix]
        static IEnumerable<StatDrawEntry> AddDeepblueDrugsEffects(IEnumerable<StatDrawEntry> statDrawEntries, HediffDef __instance)
        {
            var modExt = __instance.GetModExtension<Moyo2_ModExtension>();

            if (modExt is not null && modExt.deepblueDrugEffectsSettings.isDeepblueDrug && __instance.stages.Count > 0)
            {
                int counter = 0; // Increases by 100 per loop iteration, makes the stats show in order, from first stage to last

                for (int j = 0; j < __instance.stages.Count; j++)
                {
                    HediffStage stage = __instance.stages[j];

                    if (stage == null)
                    {
                        yield break;
                    }
                    
                    yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Moyo2_StatDrawEntry_Stage".Translate(stage.label.CapitalizeFirst().Named("stageName")),
                        string.Empty,
                        "Moyo2_StatDrawEntry_StageReportText".Translate(),
                        displayPriorityWithinCategory: 4085 - counter);

                    if (j > 0)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects,
                            string.Empty, string.Empty, string.Empty, 4090 - counter);
                    }

                    // From this point on, it's a slightly modified version of the code from HediffStage.SpecialDisplayStats, without any of the code that needs a Hediff instance to work.
                    if (stage.partEfficiencyOffset != 0f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.Basics, "PartEfficiency".Translate(), stage.partEfficiencyOffset.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Offset), "Stat_Hediff_PartEfficiency_Desc".Translate(), 4050 - counter);
                    }
                    if (stage.painFactor < 1)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, label: "Pain".Translate(), "x" + stage.painFactor.ToStringPercent(), "Stat_Hediff_Pain_Desc".Translate(), 4050 - counter);
                    }
                    if (stage.AffectsMemory || stage.AffectsSocialInteractions)
                    {
                        StringBuilder stringBuilder = new();
                        if (stage.AffectsMemory)
                        {
                            string text = "MemoryLower".Translate();
                            if (stringBuilder.Length != 0)
                            {
                                stringBuilder.Append(", ");
                            }
                            else
                            {
                                text = text.CapitalizeFirst();
                            }
                            stringBuilder.Append(text);
                        }
                        if (stage.AffectsSocialInteractions)
                        {
                            string text2 = "SocialInteractionsLower".Translate();
                            if (stringBuilder.Length != 0)
                            {
                                stringBuilder.Append(", ");
                            }
                            else
                            {
                                text2 = text2.CapitalizeFirst();
                            }
                            stringBuilder.Append(text2);
                        }
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Affects".Translate(), stringBuilder.ToString(), "Stat_Hediff_Affects_Desc".Translate(), 4080 - counter);
                    }
                    if (stage.hungerRateFactor != 1f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "HungerRate".Translate(), "x" + stage.hungerRateFactor.ToStringPercent(), "Stat_Hediff_HungerRateFactor_Desc".Translate(), 4051 - counter);
                    }
                    if (stage.hungerRateFactorOffset != 0f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Stat_Hediff_HungerRateOffset_Name".Translate(), stage.hungerRateFactorOffset.ToStringSign() + stage.hungerRateFactorOffset.ToStringPercent(), "Stat_Hediff_HungerRateOffset_Desc".Translate(), 4051 - counter);
                    }
                    if (stage.restFallFactor != 1f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Tiredness".Translate(), "x" + stage.restFallFactor.ToStringPercent(), "Stat_Hediff_TirednessFactor_Desc".Translate(), 4050 - counter);
                    }
                    if (stage.restFallFactorOffset != 0f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Stat_Hediff_TirednessOffset_Name".Translate(), stage.restFallFactorOffset.ToStringSign() + stage.restFallFactorOffset.ToStringPercent(), "Stat_Hediff_TirednessOffset_Desc".Translate(), 4050 - counter);
                    }
                    if (stage.makeImmuneTo != null)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "PreventsInfection".Translate(), stage.makeImmuneTo.Select((HediffDef im) => im.label).ToCommaList().CapitalizeFirst(), "Stat_Hediff_PreventsInfection_Desc".Translate(), 4050 - counter);
                    }
                    if (stage.totalBleedFactor != 1f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Stat_Hediff_TotalBleedFactor_Name".Translate(), stage.totalBleedFactor.ToStringPercent(), "Stat_Hediff_TotalBleedFactor_Desc".Translate(), 4041 - counter);
                    }
                    if (stage.naturalHealingFactor != -1f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Stat_Hediff_NaturalHealingFactor_Name".Translate(), stage.naturalHealingFactor.ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Factor), "Stat_Hediff_NaturalHealingFactor_Desc".Translate(), 4020 - counter);
                    }
                    if (ModsConfig.AnomalyActive && stage.regeneration != -1f && stage.showRegenerationStat)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Stat_Hediff_Regeneration_Name".Translate(), "Stat_Hediff_Regeneration_Stat".Translate($"{stage.regeneration:0}"), "Stat_Hediff_Regeneration_Desc".Translate(), 4025 - counter);
                    }
                    if (stage.foodPoisoningChanceFactor != 1f)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Stat_Hediff_FoodPoisoningChanceFactor_Name".Translate(), stage.foodPoisoningChanceFactor.ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Factor), "Stat_Hediff_FoodPoisoningChanceFactor_Desc".Translate(), 4030 - counter);
                    }
                    if (stage.statOffsets != null)
                    {
                        for (int i = 0; i < stage.statOffsets.Count; i++)
                        {
                            StatModifier statModifier = stage.statOffsets[i];
                            if (statModifier.stat.CanShowWithLoadedMods())
                            {
                                float num3 = statModifier.value;
                                yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, statModifier.stat.LabelCap, statModifier.stat.Worker.ValueToString(num3, finalized: false, ToStringNumberSense.Offset), statModifier.stat.description, 4070 - counter);
                            }
                        }
                    }
                    if (stage.statFactors != null)
                    {
                        for (int i = 0; i < stage.statFactors.Count; i++)
                        {
                            StatModifier statModifier2 = stage.statFactors[i];
                            if (statModifier2.stat.CanShowWithLoadedMods())
                            {
                                float num4 = statModifier2.value;
                                yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, statModifier2.stat.LabelCap, statModifier2.stat.Worker.ValueToString(num4, finalized: false, ToStringNumberSense.Factor), statModifier2.stat.description, 4070 - counter);
                            }
                        }
                    }
                    if (stage.capMods is not null)
                    {
                        List<PawnCapacityModifier> capMods = stage.capMods;

                        foreach (PawnCapacityModifier capMod in capMods)
                        {
                            if (capMod.offset != 0f)
                            {
                                yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, capMod.capacity.GetLabelFor().CapitalizeFirst(), (capMod.offset * 100f).ToString("+#;-#") + "%", capMod.capacity.description, 4075 - counter);
                            }
                            if (capMod.postFactor != 1f)
                            {
                                yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, capMod.capacity.GetLabelFor().CapitalizeFirst(), "x" + capMod.postFactor.ToStringPercent(), capMod.capacity.description, 4075 - counter);
                            }
                            if (capMod.SetMaxDefined)
                            {
                                yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects, label: capMod.capacity.GetLabelFor().CapitalizeFirst(), "max".Translate().CapitalizeFirst() + " " + capMod.setMax.ToStringPercent(), capMod.capacity.description, 4075 - counter);
                            }
                        }
                    }
                    if (!modExt.deepblueDrugEffectsSettings.moodOffsetsGivenPerStage.NullOrEmpty())
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.CapacityEffects,
                            label: "Moyo2_StatDrawEntry_MoodLabel".Translate(),
                            modExt.deepblueDrugEffectsSettings.moodOffsetsGivenPerStage[j].ToStringWithSign(),
                            "Moyo2_StatDrawEntry_MoodReportText".Translate(),
                            4075 - counter);
                    }
                    if (stage.damageFactors.NullOrEmpty())
                    {
                        counter += 100; // So the first iteration, which should be the first stage, shows up first
                        continue; // Was a yield break, changed to continue so we get the rest of the stages instead of skipping.
                    }
                    for (int i = 0; i < stage.damageFactors.Count; i++)
                    {
                        DamageFactor damageFactor = stage.damageFactors[i];
                        float num5 = damageFactor.factor;
                        if (!(Math.Abs(num5 - 1f) < 0.001f))
                        {
                            yield return new StatDrawEntry(label: (!(num5 >= 1f)) ? ((string)"DamageFactorResistance".Translate(damageFactor.damageDef.Named("DAMAGE")).CapitalizeFirst()) : ((string)"DamageFactorWeakness".Translate(damageFactor.damageDef.Named("DAMAGE")).CapitalizeFirst()), category: StatCategoryDefOf.CapacityEffects, valueString: "x" + num5.ToStringPercent(), reportText: "DamageFactor".Translate(damageFactor.damageDef.Named("DAMAGE")).CapitalizeFirst(), displayPriorityWithinCategory: 4080);
                        }
                    }
                    counter += 100; // So the first iteration, which should be the first stage, shows up first
                }
            }
        }
    }
}
