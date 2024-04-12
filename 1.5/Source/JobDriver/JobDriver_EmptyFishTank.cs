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
            this.FailOn(() => !FishTank.FinishedGrowing);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            // Goes to fish tank

            yield return Toils_General.Wait(200).FailOnDespawnedNullOrForbidden(TargetIndex.A)
                .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .FailOn(() => !FishTank.FinishedGrowing)
                .WithProgressBarToilDelay(TargetIndex.A);
            // Waits 200 ticks with a progress bar

            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            // Unloads the fish
            toil.AddEndCondition(() =>
            {
                if (FishTank.LockedFishDef.fishTankSettings.pawnKindDef == null)
                {
                    Log.Error("Moyo2: Selected fishDef doesn't have a defined pawnKindDef in fishTankSettings");
                    return JobCondition.Errored;
                }
                return JobCondition.Ongoing;
            });
            toil.initAction = delegate
            {
                Thing fish = FishTank.UnloadFish();
                GenSpawn.Spawn(fish, pawn.Position, Map);
                StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(fish);
                // Tries to find a good place to haul the fish stack to
                if (StoreUtility.TryFindBestBetterStoreCellFor(fish, pawn, Map, currentPriority, pawn.Faction, out var foundCell))
                {
                    job.SetTarget(TargetIndex.B, foundCell);
                    job.SetTarget(TargetIndex.C, fish);
                    job.count = fish.stackCount;
                }
                else
                {
                    EndJobWith(JobCondition.Incompletable);
                    // If it can't it ends here
                    ReadyForNextToil();
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;

            yield return Toils_Reserve.Reserve(TargetIndex.B);
            // Reserves a cell

            yield return Toils_Reserve.Reserve(TargetIndex.C);
            // Reserves the fish stack

            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.ClosestTouch);
            // Goes to the fish stack

            yield return Toils_Haul.StartCarryThing(TargetIndex.C);
            // Carries the fish stack

            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
            // Goes to a cell with the fish stack on the hands
            yield return carryToCell;

            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, true);
            // Drops the stack on the cell
        }
    }
}
