using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Moyo2_HPF
{
	public class CompProperties_ResourceHarvestable : CompProperties
	{
		public CompProperties_ResourceHarvestable()
		{
			compClass = typeof(CompResourceHarvestable);
		}

		public JobDef harvestJobDef;
		public ThingDef thingDef;
		public Constraint constraint;
		public float intervalDays = 15f;
		public int amount = 1;
		public string translationKey;
		public bool affectedByHunger;
		public List<StatModifier> speedAffectedStats = [];
		public List<StatModifier> productAffectedStats = [];
	}

	public class StatModifier
	{
		public StatDef statDef;

		public float offset;
		public float multiplier = 1f;
	}
}
