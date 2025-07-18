using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Moyo2_ItemFormChange
{
	public class CompFormChange : ThingComp
	{
		private int ticksToRevert;
		private int cooldownNow;
		private int cooldownMax;
		private Pawn pawn;


		// Cooldown and ticks to revert are different things.
		// Ticks to revert is automatically turning the weapon back.
		// Cooldown disallows clicking the button.
		public int TicksToRevert
		{
			get => ticksToRevert;
			private set => ticksToRevert = value;
		}
		public int CooldownNow => cooldownNow;
		public int CooldownMax => cooldownMax;
		public CompPropertiesFormChange Props => (CompPropertiesFormChange)props;
		private Pawn Pawn => pawn ??= (parent.ParentHolder as Pawn_EquipmentTracker)?.pawn;


		public override void CompTick()
		{
			if (Props.revertData is null) return;

			cooldownNow = Mathf.Max(cooldownNow - 1, 0);
			TicksToRevert++;
			if (TicksToRevert >= Props.revertData.revertAfterTicks)
			{
				TryChangeForm(Props.revertData);
			}
		}


		public void TryChangeForm(TransformData transformData)
		{
			//저격총 - 곤봉 처럼 왔다갔다 하면 에러 발생 할 수 있음. 가능하면 고정재료로만
			// The moon runes make the code work.

			ThingWithComps newWeapon = (ThingWithComps)ThingMaker.MakeThing(transformData.thingDef, parent.Stuff);

			newWeapon.HitPoints = parent.HitPoints;

			for (int i = newWeapon.AllComps.Count - 1; i >= 0; i--)
			{
				ThingComp weaponComp = newWeapon.AllComps[i];
				if (Props.sharedComps.Contains(weaponComp.GetType()))
				{
					ThingComp parentComp = parent.GetCompByDefType(weaponComp.props);

					newWeapon.AllComps.Remove(weaponComp);
					newWeapon.AllComps.Add(parentComp);

					// Are these redundant?
					parent.AllComps.Remove(parentComp);
					parent.AllComps.Add(weaponComp);
				}
			}

			CompFormChange compFormChange = newWeapon.TryGetComp<CompFormChange>();
			compFormChange.cooldownNow = transformData.transformCooldown;
			compFormChange.cooldownMax = transformData.transformCooldown;

			if (Pawn is null)
			{
				if (parent.Map is not null)
				{
					GenSpawn.Spawn(newWeapon, parent.Position, parent.Map);
					if (transformData.moteOnTransform is not null)
					{
						MoteMaker.MakeStaticMote(parent.DrawPos, parent.Map, transformData.moteOnTransform);
					}
					transformData.soundOnTransform?.PlayOneShot(SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map, false)));
				}
				else
				{
					ParentHolder?.GetDirectlyHeldThings().TryAdd(newWeapon, true);
				}
			}
			else
			{
				Pawn.equipment?.TryDropEquipment(parent, out _, Pawn.Position, false);
				Pawn.equipment?.AddEquipment(newWeapon);

				if (Pawn.Spawned)
				{
					if (transformData.moteOnTransform is not null)
					{
						MoteMaker.MakeStaticMote(Pawn.DrawPos, Pawn.Map, transformData.moteOnTransform, 1f);
					}
					transformData.soundOnTransform?.PlayOneShot(SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map, false)));
				}
			}
			parent.Destroy();
		}


		public IEnumerable<Gizmo> HeldGizmos()
		{
			if (Props.onlyShowWhenDrafted && !Pawn.Drafted)
			{
				yield break;
			}


			foreach (TransformData transformData in Props.transformDatas)
			{
				bool isEnabled = true;
				if (transformData.needApparel is not null)
				{
					isEnabled = false;
					foreach (Apparel apparel in pawn.apparel.WornApparel)
					{
						if (apparel.def == transformData.needApparel)
						{
							isEnabled = true;
							break;
						}
					}
				}

				yield return new Command_Transform_Action(this, transformData)
				{
					defaultLabel = transformData.label,
					defaultDesc = transformData.description,
					icon = transformData.thingDef.uiIcon,
					iconDrawScale = transformData.thingDef.uiIconScale,
					iconAngle = transformData.thingDef.uiIconAngle,
					iconOffset = transformData.thingDef.uiIconOffset,
					Disabled = (cooldownNow > 0) || !isEnabled,
					disabledReason = "appActive".Translate(transformData.needApparel?.label),
					// Ask nemo about what this is supposed to say
				};
			}
			if (Props.revertData is not null)
			{
				yield return new Command_AutoReversion_Action(this)
				{
					defaultLabel = Props.revertData.label,
					defaultDesc = Props.revertData.description,
					icon = Props.revertData.thingDef.uiIcon,
					iconDrawScale = Props.revertData.thingDef.uiIconScale,
					iconAngle = Props.revertData.thingDef.uiIconAngle,
					iconOffset = Props.revertData.thingDef.uiIconOffset,
					Disabled = true,
					disabledReason = string.Empty,
				};
			}
		}


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref cooldownNow, "cooldownNow", 0, false);
			Scribe_Values.Look(ref cooldownMax, "cooldownMax", 0, false);
			Scribe_Values.Look(ref ticksToRevert, "revertTickCounter", 0, false);
		}
	}
}
