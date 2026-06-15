using System.Linq;
using Verse.AI;

namespace Moyo2
{
    public class JobGiver_ReloadArmor : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Log.Message("Jobgiver");
            var potentialApparel = pawn.apparel.WornApparel
                .Where(ap => ap.AllComps
                    .Any(comp => comp is Comp_ReloadableArmor rel
                        && rel.ShouldRefuel)).FirstOrFallback(null);

            if (CanRefuel(pawn, potentialApparel, out var foundFuel))
            {
                return JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo2_ReloadArmor, foundFuel);
            }
            return null;
        }


        private static bool CanRefuel(Pawn pawn, Thing t, out Thing foundFuel)
        {
            foundFuel = null;
            if (t is null)
            {
                return false;
            }
            Log.Message($"Found: {t}");
            Comp_ReloadableArmor reloadableArmor = t.TryGetComp<Comp_ReloadableArmor>();
            if (reloadableArmor is null || reloadableArmor.parent.Fogged() || !reloadableArmor.ShouldRefuel || !reloadableArmor.allowRefuel)
            {
                Log.Message($"Null or fogged or shouldnt refuel or cant refuel");
                return false;
            }
            if (!pawn.CanReserve(t))
            {
                Log.Message($"Cant reserve");
                return false;
            }
            if (FindBestFuel(pawn, t) is not Thing fuel)
            {
                JobFailReason.Is("NoFuelToRefuel".Translate(reloadableArmor.Props.armor.LabelCap));
                return false;
            }
            foundFuel = fuel;
            return true;
        }


        private static Thing FindBestFuel(Pawn pawn, Thing refuelable)
        {
            ThingDef fuel = refuelable.TryGetComp<Comp_ReloadableArmor>().Props.armor;
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(fuel), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, Validator);
            bool Validator(Thing x)
            {
                return !x.IsForbidden(pawn) && pawn.CanReserve(x);
            }
        }


        public override float GetPriority(Pawn pawn)
        {
            return 5.9f;
        }
    }
}
