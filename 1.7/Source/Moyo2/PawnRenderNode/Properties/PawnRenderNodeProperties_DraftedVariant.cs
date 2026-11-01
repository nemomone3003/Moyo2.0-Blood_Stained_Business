namespace Moyo2
{
	public class PawnRenderNodeProperties_DraftedVariant : PawnRenderNodeProperties_Gendered
	{
		public PawnRenderNodeProperties_DraftedVariant()
		{
			nodeClass = typeof(PawnRenderNode_DraftedVariant);
		}

		public string undraftedTexPath;
		public string draftedTexPath;
	}
}
