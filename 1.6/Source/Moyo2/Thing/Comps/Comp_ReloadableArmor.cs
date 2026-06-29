using System.Text;

namespace Moyo2
{
	public class Comp_ReloadableArmor : ThingComp
	{
		private float armorDurability = -1f;
		private float durabilityTargetValue = -1f;
		public bool allowRefuel = true;

		public CompProperties_ReloadableArmor Props => (CompProperties_ReloadableArmor)props;
		public float TargetDurability
		{
			get => durabilityTargetValue >= 0f
				? durabilityTargetValue
				: Props.initialDurabilityTargetValue;

			set => durabilityTargetValue = Mathf.Clamp(value, 0f, Props.maxDurability);
		}
		public float TargetDurabilityPercent => ArmorDurability / TargetDurability;
		public float MaxDurabilityPercent => ArmorDurability / Props.maxDurability;
		public float ArmorDurability
		{
			get
			{
				if (armorDurability < 0f)
				{
					ArmorDurability = Props.maxDurability;
				}
				return armorDurability;
			}
			set => armorDurability = Mathf.Clamp(Mathf.Ceil(value), 0f, Props.maxDurability);
		}
		public bool IsFull => ArmorDurability >= Props.maxDurability;
		public bool IsEmpty => ArmorDurability <= 0f;
		public bool ShouldRefuel => ArmorDurability < TargetDurability;
		private Apparel Apparel => parent as Apparel;


		public override string CompInspectStringExtra()
		{
			StringBuilder sb = new(base.CompInspectStringExtra());
			sb.Append($"Armor durability: {TargetDurabilityPercent} / {MaxDurabilityPercent}");
			return sb.ToString();
		}


		public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
		{
			if (!IsEmpty && Apparel.def.apparel.CoversBodyPart(dinfo.HitPart))
			{
				ArmorDurability -= dinfo.Amount;
				dinfo.SetAmount(0f);
			}
			base.PostPreApplyDamage(ref dinfo, out absorbed);
		}


		public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
				yield return gizmo;


			if (Find.Selector.SelectedObjects.Count == 1)
			{
				yield return new Gizmo_ReloadableArmorGauge(this);
			}
		}


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref armorDurability, "Moyo2_Comp_ReloadableArmor_ArmorDurability", -1f);
			Scribe_Values.Look(ref durabilityTargetValue, "Moyo2_Comp_ReloadableArmor_DurabilityTargetValue", -1f);
			Scribe_Values.Look(ref allowRefuel, "Moyo2_Comp_ReloadableArmor_AllowRefuel", defaultValue: false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				allowRefuel = true;
			}
		}
	}
}
