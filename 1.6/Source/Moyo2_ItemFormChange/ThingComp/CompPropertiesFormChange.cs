using System.Collections.Generic;
using Verse;

namespace Moyo2_ItemFormChange
{
#pragma warning disable CA1002, CA1051
	public class CompPropertiesFormChange : CompProperties
	{
		public CompPropertiesFormChange()
		{
			compClass = typeof(CompFormChange);
		}


		public bool onlyShowWhenDrafted;
		public TransformData revertData;
		public List<TransformData> transformDatas = [];
		public List<ThingComp> sharedComps = [];
	}
}
