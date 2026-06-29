using Verse.AI;

namespace Moyo2
{
	public class FloatMenuProvider_ReloadableArmor : FloatMenuOptionProvider
	{
		protected override bool Drafted => true;
		protected override bool Undrafted => true;
		protected override bool Multiselect => false;


		public override IEnumerable<FloatMenuOption> GetOptionsFor(Thing clickedThing, FloatMenuContext context)
		{
			if (clickedThing.IsForbidden(context.FirstSelectedPawn) || clickedThing.IsBurning())
			{
				yield break;
			}

			foreach (Apparel apparel in context.FirstSelectedPawn.apparel.WornApparel)
			{
				if (apparel.TryGetComp<Comp_ReloadableArmor>() is { } comp)
				{
					if (comp.Props.armor == clickedThing.def
						&& !comp.IsFull)
					{
						yield return new FloatMenuOption("Moyo2_FloatMenu_ReloadableArmorLabel".Translate(clickedThing.LabelCapNoCount, apparel), () =>
						{
							Job job = JobMaker.MakeJob(Moyo2_JobDefOfs.Moyo2_ReloadArmor, clickedThing, apparel);
							context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
						});
					}
				}
			}
		}
	}
}