namespace Moyo2
{
    /// <summary>
    /// Checks if the pawn on which this hediff is on is a moyo, if so, the hediff gets removed
    /// </summary>
    
    /* XML Example
     * <comps>
     *   <li>
     *     <compClass>Moyo2.HediffComp_DestroyOnMoyo</compClass>
     *   </li>
     * </comps>
     */

    public class HediffComp_DestroyOnMoyo : HediffComp
    {
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (Pawn.def == Moyo2_RaceDefOf.Alien_Moyo)
            {
                Pawn.health.RemoveHediff(parent);
            }
        }
    }
}
