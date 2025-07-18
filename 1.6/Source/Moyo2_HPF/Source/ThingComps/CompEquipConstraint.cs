using Verse;

namespace Moyo2_HPF
{
	public class CompEquipConstraint : ThingComp
	{
		public CompProperties_EquipConstraint Props => (CompProperties_EquipConstraint)props;


		public bool CanPawnEquip(Pawn pawn)
		{
			foreach (var constraint in Props.constraints)
			{
				if (!constraint.CheckActiveCondition(this, pawn, parent))
				{
					return false;
				}
			}
			return true;
		}
	}
}
