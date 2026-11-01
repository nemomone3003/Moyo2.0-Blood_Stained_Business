using System;
using Verse;

namespace Moyo2_HPF
{
	public class ConstraintPawnRace : Constraint
	{
		public ThingDef race;
		public RaceCompareType compareType;

		public override bool Active(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null)
		{
			var pawnRace = pawn.kindDef.race;
			return compareType switch
			{
				RaceCompareType.Is => pawnRace == race,
				RaceCompareType.IsNot => pawnRace != race,
				RaceCompareType.Error => throw new InvalidOperationException("Undefined race comparison type."),
				_ => throw new InvalidOperationException("Unsupported race comparison type."),
			};
		}
	}
}
