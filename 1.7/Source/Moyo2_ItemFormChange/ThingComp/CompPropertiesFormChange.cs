using System;
using System.Collections.Generic;
using Verse;

namespace Moyo2_ItemFormChange
{
	public class CompPropertiesFormChange : CompProperties
	{
		public CompPropertiesFormChange()
		{
			compClass = typeof(CompFormChange);
		}


		public bool onlyShowWhenDrafted;
		public TransformData revertData;
		public List<TransformData> transformDatas = [];
		public List<Type> sharedComps = [];


		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			if (transformDatas.NullOrEmpty() && revertData is null)
			{
				yield return $"{parentDef.label} must define transformDatas or revertData";
			}
			else
			{
				foreach (TransformData data in transformDatas)
				{
					if (data.thingDef is null)
					{
						yield return $"One of {parentDef.label}'s transformDatas does not define a thingDef to transform into.";
					}
					if (data.label is null)
					{
						yield return $"One of {parentDef.label}'s transformDatas does not define a label.";
					}
					if (data.description is null)
					{
						yield return $"One of {parentDef.label}'s transformDatas does not define a description.";
					}
				}

				if (revertData is not null)
				{
					if (revertData.thingDef is null)
					{
						yield return $"One of {parentDef.label}'s transformDatas does not define a thingDef to transform into.";
					}
					if (revertData.label is null)
					{
						yield return $"One of {parentDef.label}'s transformDatas does not define a label.";
					}
					if (revertData.description is null)
					{
						yield return $"One of {parentDef.label}'s transformDatas does not define a description.";
					}
				}
			}
		}
	}
}
