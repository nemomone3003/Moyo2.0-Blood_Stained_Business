namespace Moyo2
{
    public class ThingClass_Fish : ThingWithComps
    {
        protected override void IngestedCalculateAmounts(Pawn ingester, float nutritionWanted, out int numTaken, out float nutritionIngested)
        {
            float HpDamage = nutritionWanted * 10;
            if (HitPoints > HpDamage)
            {
                HitPoints -= (int)HpDamage;
                numTaken = 0;
            }
            else
            {
                numTaken = 1;
            }
            nutritionIngested = nutritionWanted;
        }
    }
}
