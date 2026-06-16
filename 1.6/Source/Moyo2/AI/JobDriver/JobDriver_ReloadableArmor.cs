using System.Linq;
using Verse.AI;

namespace Moyo2
{
    public class JobDriver_ReloadableArmor : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job);
        }


        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.OnCell);
            yield return Toils_General.WaitWith(TargetIndex.A, 200, true);
            yield return Toils_General.Do(() =>
            {
                var potentialApparel = pawn.apparel.WornApparel
                .Where(ap => ap.AllComps
                    .Any(comp => comp is Comp_ReloadableArmor rel
                        && Comp_ReloadableArmor.ShouldRefuel
                        && rel.Props.armor == TargetA.Thing.def)).FirstOrFallback(null);

                if (potentialApparel is not null)
                {
                    var comp = potentialApparel.GetComp<Comp_ReloadableArmor>();
                    comp.ArmorDurability += comp.Props.valuePerArmor;
                    TargetA.Thing.SplitOff(1).Destroy();
                }
                else
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            });
        }
    }
}
