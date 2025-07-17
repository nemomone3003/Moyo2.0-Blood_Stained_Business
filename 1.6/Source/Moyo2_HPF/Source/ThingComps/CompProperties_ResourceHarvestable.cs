using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Moyo2_HPF
{
	public class StatModifier
	{
		public StatDef statDef;

		public float offset;
		public float multiplier;
	}


	public class CompProperties_ResourceHarvestable : CompProperties
	{
		public CompProperties_ResourceHarvestable()
		{
			compClass = typeof(CompResourceHarvestable);
		}

		public float intervalDays = 15f;
		public int amount = 1;
		public string inspectText;
		public JobDef harvestJobDef;
		public ThingDef thingDef;
		public List<Constraint> constraints = [];
		public List<StatModifier> speedAffectedStats = [];
		public List<StatModifier> productAffectedStats = [];
	}
}
