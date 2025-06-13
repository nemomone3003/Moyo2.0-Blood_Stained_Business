using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace Moyo2_ItemFormChange
{
    public class CompFormChange : ThingComp
    {
		public int revertTickCounter;

		public int cooldownNow;

		public int cooldownMax;

        public CompPropertiesFormChange Props => (CompPropertiesFormChange)props;


		public override void CompTick()
		{
			cooldownNow = Mathf.Max(cooldownNow - 1, 0);
			revertTickCounter++;
			bool shouldRevert = Props.revertData != null && Props.revertData.revertAfterTicks <= revertTickCounter;
			if (shouldRevert)
			{
                Pawn pawn = (parent.ParentHolder as Pawn_EquipmentTracker)?.pawn;
				TryChangeForm(pawn, Props.revertData);
			}
		}


		public void TryChangeForm(Pawn pawn, TransformData tsd)
        {
            //저격총 - 곤봉 처럼 왔다갔다 하면 에러 발생 할 수 있음. 가능하면 고정재료로만
            // The moon runes make the code work.

            ThingWithComps newWeapon = (ThingWithComps)ThingMaker.MakeThing(tsd?.thingDef, parent.Stuff);

            newWeapon.HitPoints = parent.HitPoints;

            foreach (ThingComp weaponComps in newWeapon.AllComps)
            {
                if (Props.SharedCompsResolved.Contains(weaponComps.GetType()))
                {
                    ThingComp parentComp = parent.GetCompByDefType(weaponComps.props);

					newWeapon.AllComps.Remove(weaponComps);
                    newWeapon.AllComps.Add(parentComp);

					// Are these redundant?
					parent.AllComps.Remove(parentComp);
					parent.AllComps.Add(weaponComps);
				}
            }

            CompFormChange compFormChange = newWeapon.TryGetComp<CompFormChange>();
            compFormChange.cooldownNow = tsd.transformCooldown;
            compFormChange.cooldownMax = tsd.transformCooldown;

            if (pawn is null)
            {
                if (parent.Map is not null)
                {
                    GenSpawn.Spawn(newWeapon, parent.Position, parent.Map);
                    if (tsd.moteOnTransform is not null)
                    {
                        MoteMaker.MakeStaticMote(parent.DrawPos, parent.Map, tsd.moteOnTransform);
                    }
					tsd.soundOnTransform?.PlayOneShot(SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map, false)));
				}
                else
                {
                    ParentHolder?.GetDirectlyHeldThings().TryAdd(newWeapon, true);
                }
            }
            else
            {
                pawn.equipment?.AddEquipment(newWeapon);

                if (pawn.Spawned)
                {
					if (tsd.moteOnTransform is not null)
					{
						MoteMaker.MakeStaticMote(pawn.DrawPos, pawn.Map, tsd.moteOnTransform, 1f);
					}
					tsd.soundOnTransform?.PlayOneShot(SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map, false)));
				}
			}

			parent.Destroy(0);
		}


        public IEnumerable<Gizmo> HeldGizmos(Pawn pawn)
        {
            if (Props.onlyShowWhenDrafted && !pawn.Drafted)
            {
                yield break;
            }


            foreach (TransformData tsd2 in Props.transformData)
            {
                bool appActive = true;
                if (tsd2.needApparel != null)
                {
                    appActive = false;
                    foreach (var pawnApparel in pawn.apparel.WornApparel)
                    {
                        if (pawnApparel.def == tsd2.needApparel)
                        {
                            appActive = true;
                        }
                    }
                }
                TransformData tsdP = tsd2;
                yield return new Command_Transform_Action
                {
                    defaultLabel = tsd2.label,
                    defaultDesc = tsd2.description,
                    compFormChange = this,
                    transformData = tsdP,
                    icon = tsd2.thingDef.uiIcon,
                    Disabled = (cooldownNow > 0) || !appActive,
                    disabledReason = appActive ? (TaggedString)"" : "appActive".Translate(tsd2.needApparel.label),
                    action = delegate
                    {
                        TryChangeForm(pawn, tsdP);
                    }
                };
            }
            if (Props.revertData != null)
            {
                TransformData tsd = Props.revertData;
                yield return new Command_AutoReversion_Action(this)
                {
                    defaultLabel = tsd.label,
                    defaultDesc = tsd.description,
                    //transformData = tsd,
                    icon = tsd.thingDef.uiIcon,
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
