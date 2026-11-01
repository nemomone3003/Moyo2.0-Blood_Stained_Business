using System;
using Verse;

namespace Moyo2_HPF
{
	public class ConstraintPawnGender : Constraint
	{
		public Gender gender;
		public GenderCompareType compareType;

		public override bool Active(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null)
		{
			var pawnGender = pawn?.gender;
			return compareType switch
			{
				GenderCompareType.Is => pawnGender == gender,
				GenderCompareType.IsNot => pawnGender != gender,
				GenderCompareType.Error => throw new InvalidOperationException("Undefined gender comparison type."),
				_ => throw new InvalidOperationException("Unsupported gender comparison type.")
			};
		}
	}
}
