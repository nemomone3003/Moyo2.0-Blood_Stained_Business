using System.Linq;
using Verse.AI;

namespace Moyo2
{
	public class FloatMenuProvider_ReloadableArmor : FloatMenuOptionProvider
	{
		protected override bool Drafted => true;
		protected override bool Undrafted => true;
		protected override bool Multiselect => false;


		protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
		{
			if (clickedThing.IsForbidden(context.FirstSelectedPawn) || clickedThing.IsBurning())
			{
				return null;
			}

			var potentialApparel = context.FirstSelectedPawn.apparel.WornApparel
				.Where(ap => ap.AllComps
					.Any(comp => comp is Comp_ReloadableArmor rel
						&& rel.Props.armor == clickedThing.def)).FirstOrFallback(null);

			if (potentialApparel is null)
			{
				return null;
			}

			return new FloatMenuOption("Moyo2_FloatMenu_ReloadableArmorLabel".Translate(clickedThing), () =>
			{
				Job job = JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo2_ReloadArmor, clickedThing);
				context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			});
		}
	}
}
