﻿namespace Moyo2
{
	public class HediffComp_DestroyOnMoyo : HediffComp
	{
		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			if (Pawn.def == Moyo2_RaceDefOf.Alien_Moyo)
			{
				Pawn.health.RemoveHediff(parent);
			}
			base.CompPostPostAdd(dinfo);
		}
	}
}
