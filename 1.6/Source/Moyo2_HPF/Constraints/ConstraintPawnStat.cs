using RimWorld;
using System;
using Verse;

namespace Moyo2_HPF
{
	public class ConstraintPawnStat : Constraint
	{
		public StatDef stat;
		public float value;
		public StatCompareType compareType;


		public override bool Active(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null)
		{
			var pawnStatValue = pawn?.GetStatValue(stat);
			return compareType switch
			{
				StatCompareType.Equal => pawnStatValue == value,
				StatCompareType.NotEqual => pawnStatValue != value,
				StatCompareType.Over => pawnStatValue > value,
				StatCompareType.OverInclusive => pawnStatValue >= value,
				StatCompareType.Under => pawnStatValue < value,
				StatCompareType.UnderInclusive => pawnStatValue <= value,
				StatCompareType.Error => throw new InvalidOperationException("Undefined stat comparison type."),
				_ => throw new InvalidOperationException("Unsupported stat comparison type.")
			};
		}
	}
}
