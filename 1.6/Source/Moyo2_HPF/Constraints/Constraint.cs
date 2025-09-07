using Verse;

namespace Moyo2_HPF
{
	public abstract class Constraint
	{
		public abstract bool Active(ThingComp comp, Pawn pawn = null, ThingWithComps equipment = null);

		public bool passIfNotProperParent;
	}
}
