using AlienRace.ExtendedGraphics;
using System.Linq;

namespace Moyo2
{
	public class ConditionApparelTag : Condition
	{
		public new const string XmlNameParseKey = "ApparelTags";

		public string tag = string.Empty;
		//public List<string> tags = [];

		public override bool Satisfied(ExtendedGraphicsPawnWrapper pawn, ref ResolveData data)
		{
			Log.Message("Is it firing?");
			Log.Message($"We're looking for {tag}");

			return !pawn.GetWornApparelProps().Where(props => props.tags.Any(tag => this.tag.Contains(tag))).Any();
			/*
			foreach (Apparel ap in pawn.GetWornApparel)
			{
				foreach (string tag in ap.def.apparel.tags)
				{
					if (found) break;
					Log.Message($"Found tag: {tag}");
					if (this.tag.Contains(tag))
					{
						Log.Message($"Matched {tag}");
						found = true;
						break;
					}
				}
			}
			*/
		}
	}
}
