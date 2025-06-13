using System;
using UnityEngine;
using Verse;

namespace Moyo2_ItemFormChange
{
    [StaticConstructorOnStartup]
    public class Command_Transform_Action : Command
    {
		public Action action;

		public CompFormChange compFormChange;

		public TransformData transformData;


		public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            action();
        }


        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect baseRect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
            GizmoResult gizmoResult = base.GizmoOnGUI(topLeft, maxWidth, parms);

            bool onCooldown = compFormChange.cooldownNow > 0;
            if (onCooldown)
            {
                float fillPercent = Mathf.InverseLerp(compFormChange.cooldownMax, 0f, compFormChange.cooldownNow);
                Widgets.FillableBar(baseRect, Mathf.Clamp01(fillPercent), TextureLibrary.cooldownBarTex, null, false);
            }
			bool gizmoInteracted = gizmoResult.State == GizmoState.Interacted;

			return gizmoInteracted ? gizmoResult : new GizmoResult(gizmoResult.State);
		}
    }
}
