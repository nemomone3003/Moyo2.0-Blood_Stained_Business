using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Moyo2_HPF
{
	public class WorkGiver_GatherPawnResources : WorkGiver_Scanner
	{
		private ModExtension modExtension;


		public ModExtension ModExtension
		{
			get
			{
				modExtension ??= def.GetModExtension<ModExtension>();
				return modExtension;
			}
		}


		/*
		public override PathEndMode PathEndMode
		{
			get
			{
				if (def is not HPFWorkGiverDef hpfworkGiverDef)
				{
					return PathEndMode.None;
				}
				if (hpfworkGiverDef.harvestJobDef is not HPFJobDef jobDef)
				{
					return PathEndMode.None;
				}
				return jobDef.isSelf ? PathEndMode.None : PathEndMode.Touch;
			}
		}
		*/


		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			foreach (Pawn potentialPawn in pawn.Map.mapPawns.FreeColonistsAndPrisonersSpawned)
			{
				if (potentialPawn.GetComp<CompResourceHarvestable>() is CompResourceHarvestable comp
					&& comp.ActiveAndFull)
				{
					yield return potentialPawn;
				}
			}
		}


		public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			if (ModExtension is null)
			{
				Log.Error($"Job {def.defName} requires Moyo2_HPF.ModExtension to work properly.");
				return false;
			}
			if (thing is not Pawn foundPawn)
			{
				return false;
			}

			if (ModExtension.isSelf)
			{
				if (pawn == foundPawn &&
					foundPawn.GetComps<CompResourceHarvestable>()
						.Any(x => x.Props.harvestJobDef == ModExtension.harvestJobDef && x.ActiveAndFull))
				{
					return true;
				}
			}
			else // job is not allowed on themselves
			{
				if (pawn == foundPawn)
				{
					return false;
				}

				bool anyHarvestable = foundPawn.GetComps<CompResourceHarvestable>()
					.Any(comp => comp.Props.harvestJobDef == ModExtension.harvestJobDef && comp.ActiveAndFull);

				if (anyHarvestable && PawnUtility.CanCasuallyInteractNow(foundPawn, false) && pawn.CanReserve(foundPawn))
				{
					return true;
				}
			}
			return false;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(ModExtension.harvestJobDef, t);
		}
	}
}
