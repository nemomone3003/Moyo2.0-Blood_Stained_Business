using RimWorld;
using Verse;

namespace Moyo2_HPF
{
	public class ModExtension : DefModExtension
	{
		// ---- For JobDefs ----
		public bool isSelf;
		public float totalWork;
		public float xpPerTick;
		public SkillDef activeSkill;
		public StatDef activeStat;

		// ---- For WorkGiverDefs ---
		public JobDef harvestJobDef;
	}
}
