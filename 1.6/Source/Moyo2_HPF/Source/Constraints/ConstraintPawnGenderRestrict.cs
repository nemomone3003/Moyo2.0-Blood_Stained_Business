using Verse;

namespace Moyo2_HPF
{
	public class ConstraintPawnGenderRestrict : ConstraintPawnGender
	{
		public override bool CheckActiveCondition(ThingComp comp, Pawn pawn, ThingWithComps equipment)
		{
			return pawn != null && pawn.gender != this.gender;
		}
	}
}
