﻿using UnityEngine;
using Verse;

namespace Moyo2_ItemFormChange
{
	public class Command_Transform_Action : Command
	{
		private readonly CompFormChange compFormChange;
		private readonly TransformData transformData;


		public Command_Transform_Action(CompFormChange compFormChange, TransformData transformData)
		{
			this.compFormChange = compFormChange;
			this.transformData = transformData;
		}


		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			compFormChange.TryChangeForm(transformData);
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
