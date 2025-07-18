using Verse;

namespace Moyo2_HPF
{
	public class ConstraintNotOp : Constraint
	{
		public override bool CheckActiveCondition(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null)
		{
			return !this.constraint.CheckActiveCondition(comp, pawn, equipment);
		}

		public Constraint constraint;
	}
}
