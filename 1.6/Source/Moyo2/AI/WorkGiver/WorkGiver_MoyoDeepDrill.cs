namespace Moyo2
{
	public class WorkGiver_MoyoDeepDrill : WorkGiver_DeepDrill
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Moyo2_ThingDefOfs.Moyo2_DeepDrill);
	}
}
