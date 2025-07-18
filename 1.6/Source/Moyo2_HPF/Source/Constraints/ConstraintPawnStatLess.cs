using RimWorld;
using Verse;

namespace Moyo2_HPF
{
	public class ConstraintPawnStatLess : ConstraintPawnStat
	{
		public override bool CheckActiveCondition(ThingComp comp, Pawn pawn, ThingWithComps equipment)
		{
			if (pawn == null || !this.equal)
			{
				return pawn.GetStatValue(this.stat, true) < this.value;
			}
			return pawn.GetStatValue(this.stat, true) <= this.value;
		}
	}
}
