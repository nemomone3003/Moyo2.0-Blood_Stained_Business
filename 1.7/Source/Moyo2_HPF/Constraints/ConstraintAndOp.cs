using System.Collections.Generic;
using Verse;

namespace Moyo2_HPF
{
	public class ConstraintAndOp : Constraint
	{
		public List<Constraint> constraints = [];


		public override bool Active(ThingComp comp, Pawn pawn, ThingWithComps equipment)
		{
			foreach (Constraint constraint in constraints)
			{
				if (!constraint.Active(comp, pawn, equipment))
				{
					return false;
				}
			}
			return true;
		}
	}
}
