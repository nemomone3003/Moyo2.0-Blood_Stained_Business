using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Moyo2_HPF
{
	public class WorkGiver_GatherPawnResources : WorkGiver_Scanner
	{
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


		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			foreach (Pawn potentialPawn in pawn.Map.mapPawns.FreeColonistsAndPrisonersSpawned)
			{
				if (potentialPawn.GetComp<CompResourceHarvestable>() is not null)
				{
					yield return potentialPawn;
				}
			}
		}


		public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			if (def is not HPFWorkGiverDef hpfworkGiverDef)
			{
				return false;
			}
			if (hpfworkGiverDef.harvestJobDef is not HPFJobDef jobDef)
			{
				return false;
			}
			if (thing is not Pawn foundPawn)
			{
				return false;
			}

			if (jobDef.isSelf)
			{
				if (pawn == thing &&
					pawn.GetComps<CompResourceHarvestable>().Any(x => x.Props.harvestJobDef == jobDef && x.ActiveAndFull))
				{
					return true;
				}
			}
			else // job is not allowed on themselves
			{
				if (pawn == thing)
				{
					return false;
				}

				foreach (CompResourceHarvestable comp in from x in foundPawn.GetComps<CompResourceHarvestable>()
														 where x.Props.harvestJobDef == jobDef && x.ActiveAndFull
														 select x)
				{
					if (PawnUtility.CanCasuallyInteractNow(foundPawn, false) && pawn.CanReserve(foundPawn))
					{
						return true;
					}
				}
			}
			return false;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (def is not HPFWorkGiverDef hpfworkGiverDef)
			{
				return null;
			}
			return new Job(hpfworkGiverDef.harvestJobDef, t);
		}
	}
}
