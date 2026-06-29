using System.Text;

namespace Moyo2
{
	public abstract class StatPart_ReloadableArmor : StatPart
	{
		private readonly Dictionary<Thing, Comp_ReloadableArmor> Cache = [];


		protected Comp_ReloadableArmor GetComp(StatRequest req)
		{
			if (!Cache.TryGetValue(req.Thing, out var comp))
			{
				Cache.Add(req.Thing, req.Thing.TryGetComp<Comp_ReloadableArmor>());
			}
			return comp;
		}


		protected abstract float? GetArmorOverride(StatRequest req);


		public override string ExplanationPart(StatRequest req)
		{
			StringBuilder stringBuilder = new();

			if (GetComp(req) is not null && !GetComp(req).IsEmpty && GetArmorOverride(req) is not null)
			{
				stringBuilder.AppendLine("Moyo2_StatPart_ReloadableArmorOverride".Translate(((float)GetArmorOverride(req)).ToStringPercent()));
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}


		public override void TransformValue(StatRequest req, ref float val)
		{
			if (GetComp(req) is not null && !GetComp(req).IsEmpty && GetArmorOverride(req) is not null)
			{
				val = (float)GetArmorOverride(req);
			}
		}
	}


	public class StatPart_ReloadableArmor_Sharp : StatPart_ReloadableArmor
	{
		protected override float? GetArmorOverride(StatRequest req)
		{
			return GetComp(req).Props.armorOverrideSharp;
		}
	}


	public class StatPart_ReloadableArmor_Blunt : StatPart_ReloadableArmor
	{
		protected override float? GetArmorOverride(StatRequest req)
		{
			return GetComp(req).Props.armorOverrideBlunt;
		}
	}


	public class StatPart_ReloadableArmor_Heat : StatPart_ReloadableArmor
	{
		protected override float? GetArmorOverride(StatRequest req)
		{
			return GetComp(req).Props.armorOverrideHeat;
		}
	}
}
