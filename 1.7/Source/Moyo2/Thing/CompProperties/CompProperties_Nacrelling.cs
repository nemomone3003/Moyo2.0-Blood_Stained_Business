namespace Moyo2
{
	public class CompProperties_Nacrelling : CompProperties
	{
		public CompProperties_Nacrelling()
		{
			compClass = typeof(Comp_Nacrelling);
		}

		public string messageColonistEmerged;
		public string messageColonistEmergedDied;
		public string messageColonistSwallowed;
		public float nutritionPerTick;

		public SimpleCurve bodySizeDigestTimeCurve;

		public SimpleCurve timeDamageCurve;
	}
}
