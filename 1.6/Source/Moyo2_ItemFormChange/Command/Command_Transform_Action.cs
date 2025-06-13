using System;
using UnityEngine;
using Verse;

namespace Moyo2_ItemFormChange
{
	[StaticConstructorOnStartup]
	public class Command_Transform_Action : Command
	{
		private readonly CompFormChange compFormChange;


		public Command_Transform_Action(CompFormChange compFormChange)
		{
			this.compFormChange = compFormChange;
		}


		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			compFormChange.TryChangeForm();
		}


		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect baseRect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
			GizmoResult gizmoResult = base.GizmoOnGUI(topLeft, maxWidth, parms);

			bool onCooldown = compFormChange.CooldownNow > 0;
			if (onCooldown)
			{
				float fillPercent = Mathf.InverseLerp(compFormChange.CooldownMax, 0f, compFormChange.CooldownNow);
				Widgets.FillableBar(baseRect, Mathf.Clamp01(fillPercent), TextureLibrary.cooldownBarTex, null, false);
			}
			bool gizmoInteracted = gizmoResult.State == GizmoState.Interacted;

			return gizmoInteracted ? gizmoResult : new GizmoResult(gizmoResult.State);
		}
	}
}
