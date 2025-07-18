using System.Collections.Generic;
using Verse;

namespace Moyo2_HPF
{
#pragma warning disable CA2211
	public class CompProperties_EquipConstraint : CompProperties
	{
		public CompProperties_EquipConstraint()
		{
			compClass = typeof(CompEquipConstraint);
		}


		public static Dictionary<ThingDef, CompProperties_EquipConstraint> cachedThingDefs = [];
		public List<Constraint> constraints = [];


		public override void PostLoadSpecial(ThingDef parent)
		{
			cachedThingDefs.Add(parent, this);
		}
	}
}
