using Verse;

namespace Moyo2_HPF
{
	public class ConstraintNotOp : Constraint
	{
		public Constraint constraint;


		public override bool Active(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null)
		{
			return !constraint.Active(comp, pawn, equipment);
		}
	}
}
