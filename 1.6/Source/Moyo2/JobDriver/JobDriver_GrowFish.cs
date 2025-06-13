using Verse.AI;

namespace Moyo2
{
    public class JobDriver_GrowFish : JobDriver
    {
        // TargetA == Fish tank

        private ThingClass_FishTank FishTank => (ThingClass_FishTank)job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
        }


        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            // Goes to fish tank

            Toil waitToilWithSkillGain = Toils_General.Wait(400).FailOnDespawnedNullOrForbidden(TargetIndex.A)
                .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .FailOn(() => FishTank.FinishedGrowing)
                .WithProgressBarToilDelay(TargetIndex.A);
            waitToilWithSkillGain.tickIntervalAction = delegate(int delta)
            {
                waitToilWithSkillGain.actor.skills?.Learn(SkillDefOf.Animals, 0.085f * delta);
            };
            waitToilWithSkillGain.activeSkill = () => SkillDefOf.Animals;
            // Wait 400 ticks, and get experience on the animal skill while doing so
            yield return waitToilWithSkillGain;

            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            // Starts growing fish in fish tank
            toil.initAction = FishTank.LoadFish;
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;
        }
    }
}
