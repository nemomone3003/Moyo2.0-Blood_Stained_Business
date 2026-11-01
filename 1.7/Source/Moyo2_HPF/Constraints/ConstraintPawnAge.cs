using System;
using Verse;

namespace Moyo2_HPF
{
	public class ConstraintPawnAge : Constraint
	{
		public int age;
		public AgeCompareType compareType;

		public override bool Active(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null)
		{
			var pawnAge = pawn?.ageTracker?.AgeBiologicalYears;
			return compareType switch
			{
				AgeCompareType.Equal => pawnAge == age,
				AgeCompareType.NotEqual => pawnAge != age,
				AgeCompareType.Over => pawnAge > age,
				AgeCompareType.OverInclusive => pawnAge >= age,
				AgeCompareType.Under => pawnAge < age,
				AgeCompareType.UnderInclusive => pawnAge <= age,
				AgeCompareType.Error => throw new InvalidOperationException("Undefined age comparison type."),
				_ => throw new InvalidOperationException("Unsupported age comparison type.")
			};
		}
	}
}
