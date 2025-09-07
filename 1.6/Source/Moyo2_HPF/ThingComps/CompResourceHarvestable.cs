using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Moyo2_HPF
{
	public class CompResourceHarvestable : ThingComp
	{
		private float fullness;


		private Pawn Pawn => parent as Pawn;
		public CompProperties_ResourceHarvestable Props => (CompProperties_ResourceHarvestable)props;


		protected bool Active => Props.constraint.Active(this, Pawn, null);
		public float Fullness
		{
			get => fullness;
			set
			{
				if (value > 1f) fullness = 1f;
				else if (value < 0f) fullness = 0f;
				else fullness = value;
			}
		}
		public bool ActiveAndFull => Active && Fullness >= 1f;


		public override void CompTickInterval(int delta)
		{
			if (Active)
			{
				float baseOffset = 0f;
				foreach (var speedMod in Props.speedAffectedStats)
				{
					baseOffset += (parent.GetStatValue(speedMod.statDef) + speedMod.offset) * speedMod.multiplier;
				}
				if (Props.affectedByHunger && parent is Pawn pawn)
				{
					baseOffset *= PawnUtility.BodyResourceGrowthSpeed(pawn);
				}

				Fullness += baseOffset / (GenDate.TicksPerDay * Props.intervalDays) * delta;
				/*
				if (parent is Pawn pawn)
				{
					baseOffset *= PawnUtility.BodyResourceGrowthSpeed(pawn);
				}
				float progressPerTick = 1f / GenDate.TicksPerDay * Props.intervalDays;
				if (parent is Pawn pawn)
				{
					progressPerTick *= PawnUtility.BodyResourceGrowthSpeed(pawn);
				}
				if (Props.speedAffectedStats?.Any() ?? false)
				{
					foreach (var statMultiplier in Props.speedAffectedStats)
					{
						progressPerTick = Mathf.Max(0, progressPerTick + (parent.GetStatValue(statMultiplier.statDef) + statMultiplier.offset) * statMultiplier.multiplier);
					}
				}*/
			}
			base.CompTickInterval(delta);
		}


		public void Gathered(Pawn doer, StatDef stat)
		{
			if (!Active)
			{
				Log.Error(doer + " gathered body resources while not Active: " + parent);
				return;
			}
			if (!Rand.Chance(doer.GetStatValue(stat, true)))
			{
				MoteMaker.ThrowText((doer.DrawPos + parent.DrawPos) / 2f, parent.Map, "TextMote_ProductWasted".Translate(), 3.65f);
			}
			else
			{
				float baseAmount = Props.amount;
				foreach (var statMultiplier in Props.productAffectedStats)
				{
					baseAmount += (parent.GetStatValue(statMultiplier.statDef) + statMultiplier.offset) * statMultiplier.multiplier;
				}

				int totalAmount = Mathf.RoundToInt(baseAmount);
				int stacks = Mathf.CeilToInt((float)totalAmount / Props.thingDef.stackLimit);

				for (int i = 0; i < stacks; ++i)
				{
					int stackCount = Mathf.Min(totalAmount, Props.thingDef.stackLimit);
					totalAmount -= stackCount;

					Thing thing = ThingMaker.MakeThing(Props.thingDef, null);
					thing.stackCount = stackCount;
					GenPlace.TryPlaceThing(thing, doer.Position, doer.Map, ThingPlaceMode.Near);
				}
			}
			Fullness = 0f;
		}


		public override string CompInspectStringExtra()
		{
			if (!Active)
			{
				return null;
			}
			if (Props.translationKey is null)
			{
				return null;
			}
			return $"{Props.translationKey.Translate()}: {fullness.ToStringPercent()}";
		}


		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.godMode)
			{
				yield return new Command_Action()
				{
					defaultLabel = $"DEV: Add {Props.thingDef.label} progress +10%",
					action = () =>
					{
						fullness += 0.1f;
					}
				};
				yield return new Command_Action()
				{
					defaultLabel = $"DEV: Add {Props.thingDef.label} progress +100%",
					action = () =>
					{
						fullness += 1f;
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
