using Verse.AI;

namespace Moyo2
{
    public class JobDriver_EmptyFishTank : JobDriver
    {
        // TargetA == Fish tank
        // TargetB == cell to haul to
        // TargetC == fish stack to haul

        private ThingClass_FishTank FishTank => (ThingClass_FishTank)job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOn(() => !FishTank.FishFinishedGrowing);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            yield return Toils_General.Wait(200).FailOnDespawnedNullOrForbidden(TargetIndex.A)
                .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .FailOn(() => !FishTank.FishFinishedGrowing)
                .WithProgressBarToilDelay(TargetIndex.A);

            Toil unloadFishToil = ToilMaker.MakeToil("UnloadFish");
            unloadFishToil.initAction = delegate
            {
                Thing fish = FishTank.UnloadFish();
                GenSpawn.Spawn(fish, pawn.Position, Map);
                StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(fish);

                // Find place to haul fish to
                if (StoreUtility.TryFindBestBetterStoreCellFor(fish, pawn, Map, currentPriority, pawn.Faction, out var foundCell))
                {
                    job.SetTarget(TargetIndex.B, foundCell);
                    job.SetTarget(TargetIndex.C, fish);
                    job.count = fish.stackCount;
                }
            };
            unloadFishToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return unloadFishToil;

            Toil endToil = ToilMaker.MakeToil("EndToil");

            yield return Toils_Jump.JumpIf(endToil, () => !TargetB.IsValid || !TargetC.IsValid);

			yield return Toils_Reserve.Reserve(TargetIndex.B);

            yield return Toils_Reserve.Reserve(TargetIndex.C);

            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.ClosestTouch);

            yield return Toils_Haul.StartCarryThing(TargetIndex.C);

            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.B);

            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, endToil, true);

            yield return endToil;
        }
    }
}
