using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Moyo2_HPF
{
	public class CompResourceHarvestable : ThingComp
	{
		private float fullness;

		public CompProperties_ResourceHarvestable Props => (CompProperties_ResourceHarvestable)props;


		protected bool Active
		{
			get
			{
				if (parent.Faction == null)
				{
					return false;
				}
				Pawn pawn = parent as Pawn;
				foreach (Constraint constraint in Props.constraints)
				{
					if (!constraint.CheckActiveCondition(this, pawn, null))
					{
						return false;
					}
				}
				return true;
			}
		}
		public bool ActiveAndFull => Active && fullness >= 1f;


		public override void CompTick()
		{
			if (Active)
			{
				float progressPerTick = 1f / (Props.intervalDays * 60000f);
				if (parent is Pawn pawn)
				{
					progressPerTick *= PawnUtility.BodyResourceGrowthSpeed(pawn);

					if (Props.speedAffectedStats?.Any() ?? false)
					{
						foreach (var statMultiplier in Props.speedAffectedStats)
						{
							progressPerTick = Mathf.Max(0, progressPerTick + (parent.GetStatValue(statMultiplier.statDef) + statMultiplier.offset) * statMultiplier.multiplier);
						}
					}
				}

				fullness += progressPerTick;
				if (fullness > 1f)
				{
					fullness = 1f;
				}
			}
		}


		public void Gathered(Pawn doer, StatDef stat)
		{
			if (!Active)
			{
				Log.Error(doer + " gathered body resources while not Active: " + parent);
			}
			if (!Rand.Chance(doer.GetStatValue(stat, true)))
			{
				MoteMaker.ThrowText((doer.DrawPos + parent.DrawPos) / 2f, parent.Map, "TextMote_ProductWasted".Translate(), 3.65f);
			}
			else
			{
				float baseAmount = Props.amount;
				if (Props.productAffectedStats?.Any() ?? false)
				{
					foreach (var statMultiplier in Props.productAffectedStats)
					{
						baseAmount = Mathf.Max(0, baseAmount + (parent.GetStatValue(statMultiplier.statDef) + statMultiplier.offset) * statMultiplier.multiplier);
					}
				}

				int totalAmount = GenMath.RoundRandom(baseAmount * fullness);
				while (totalAmount > 0)
				{
					int oneStackCount = Mathf.Clamp(totalAmount, 1, Props.thingDef.stackLimit);
					totalAmount -= oneStackCount;

					Thing thing = ThingMaker.MakeThing(Props.thingDef, null);
					thing.stackCount = oneStackCount;
					GenPlace.TryPlaceThing(thing, doer.Position, doer.Map, ThingPlaceMode.Near);
				}
			}
			fullness = 0f;
		}


		public override string CompInspectStringExtra()
		{
			if (!Active)
			{
				return null;
			}
			if (Props.inspectText is null)
			{
				return null;
			}
			return $"{Props.inspectText}: {fullness.ToStringPercent()}";
		}


		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.godMode)
			{
				yield return new Command_Action()
				{
					defaultLabel = $"DEV: Add {Props.thingDef.label} Progress +10%",
					action = () =>
					{
						fullness += 0.1f;
					}
				};
			}
		}


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref fullness, "Moyo2_HPF_fullness");
		}
	}
}
