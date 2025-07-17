using UnityEngine;
using Verse;

namespace Moyo2_ItemFormChange
{
	public class Command_AutoReversion_Action : Command
	{
		private readonly CompFormChange compFormChange;
		//internal TransformData transformData;


		public Command_AutoReversion_Action(CompFormChange compFormChange)
		{
			this.compFormChange = compFormChange;
		}


		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
		}


		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect baseRect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
			GizmoResult gizmoResult = base.GizmoOnGUI(topLeft, maxWidth, parms);

			float fillPercent = 1f - Mathf.InverseLerp(compFormChange.Props.revertData.revertAfterTicks, 0f, compFormChange.TicksToRevert);
			Widgets.FillableBar(baseRect, Mathf.Clamp01(fillPercent), TextureLibrary.cooldownBarTex, null, false);

			bool onCooldown = compFormChange.CooldownNow > 0;
			if (onCooldown)
			{
				using (new TextBlock(GameFont.Tiny, TextAnchor.UpperCenter))
				{
					Widgets.Label(baseRect, fillPercent.ToStringPercent("F0"));
				}
			}
			bool gizmoInteracted = gizmoResult.State == GizmoState.Interacted;

			return gizmoInteracted ? gizmoResult : new GizmoResult(gizmoResult.State);
		}
	}
}
