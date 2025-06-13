using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Moyo2_ItemFormChange
{
	public class CompFormChange : ThingComp
	{
		private int revertTickCounter;
		private int cooldownNow;
		private int cooldownMax;
		private Pawn pawn;


		public int RevertTickCounter => revertTickCounter;
		public int CooldownNow => cooldownNow;
		public int CooldownMax => cooldownMax;
		public CompPropertiesFormChange Props => (CompPropertiesFormChange)props;
		private Pawn Pawn
		{
			get
			{
				pawn ??= (parent.ParentHolder as Pawn_EquipmentTracker)?.pawn;
				return pawn;
			}
		}


		public override void CompTick()
		{
			cooldownNow = Mathf.Max(cooldownNow - 1, 0);
			revertTickCounter++;
			bool shouldRevert = Props.revertData != null && Props.revertData.revertAfterTicks <= revertTickCounter;
			if (shouldRevert)
			{
				TryChangeForm();
			}
		}


		public void TryChangeForm()
		{
			//저격총 - 곤봉 처럼 왔다갔다 하면 에러 발생 할 수 있음. 가능하면 고정재료로만
			// The moon runes make the code work.

			ThingWithComps newWeapon = (ThingWithComps)ThingMaker.MakeThing(Props.revertData?.thingDef, parent.Stuff);

			newWeapon.HitPoints = parent.HitPoints;

			foreach (ThingComp weaponComp in newWeapon.AllComps)
			{
				if (Props.sharedComps.Contains(weaponComp))
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
			compFormChange.cooldownNow = Props.revertData.transformCooldown;
			compFormChange.cooldownMax = Props.revertData.transformCooldown;

			if (Pawn is null)
			{
				if (parent.Map is not null)
				{
					GenSpawn.Spawn(newWeapon, parent.Position, parent.Map);
					if (Props.revertData.moteOnTransform is not null)
					{
						MoteMaker.MakeStaticMote(parent.DrawPos, parent.Map, Props.revertData.moteOnTransform);
					}
					Props.revertData.soundOnTransform?.PlayOneShot(SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map, false)));
				}
				else
				{
					ParentHolder?.GetDirectlyHeldThings().TryAdd(newWeapon, true);
				}
			}
			else
			{
				Pawn.equipment?.AddEquipment(newWeapon);

				if (Pawn.Spawned)
				{
					if (Props.revertData.moteOnTransform is not null)
					{
						MoteMaker.MakeStaticMote(Pawn.DrawPos, Pawn.Map, Props.revertData.moteOnTransform, 1f);
					}
					Props.revertData.soundOnTransform?.PlayOneShot(SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map, false)));
				}
			}

			parent.Destroy(0);
		}


		public IEnumerable<Gizmo> HeldGizmos()
		{
			if (Props.onlyShowWhenDrafted && !Pawn.Drafted)
			{
				yield break;
			}


			foreach (TransformData transformData in Props.transformDatas)
			{
				bool isEnabled = false;
				if (transformData.needApparel is not null)
				{
					foreach (Apparel apparel in pawn?.apparel.WornApparel)
					{
						if (apparel.def == transformData.needApparel)
						{
							isEnabled = true;
						}
					}
				}

				yield return new Command_Transform_Action(this)
				{
					defaultLabel = transformData.label,
					defaultDesc = transformData.description,
					icon = transformData.thingDef.uiIcon,
					iconDrawScale = transformData.thingDef.uiIconScale,
					iconAngle = Props.revertData.thingDef.uiIconAngle,
					iconOffset = Props.revertData.thingDef.uiIconOffset,
					Disabled = (cooldownNow > 0) || !isEnabled,
					disabledReason = "appActive".Translate(transformData.needApparel.label),
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
			Scribe_Values.Look(ref revertTickCounter, "revertTickCounter", 0, false);
		}
	}
}
