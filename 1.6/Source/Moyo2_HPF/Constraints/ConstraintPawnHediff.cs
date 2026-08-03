using System;
using Verse;

namespace Moyo2_HPF
{
	public class ConstraintPawnHediff : Constraint
	{
		public HediffDef hediff;
		public HediffCompareType compareType;

		public override bool Active(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null)
		{
			var hediffSet = pawn?.health?.hediffSet;
			return compareType switch
			{
				HediffCompareType.Has => hediffSet.HasHediff(hediff),
				HediffCompareType.HasNot => !hediffSet.HasHediff(hediff),
				HediffCompareType.Error => throw new InvalidOperationException("Undefined hediff comparison type."),
				_ => throw new InvalidOperationException("Unsupported hediff comparison type.")
			};
		}
	}
}
