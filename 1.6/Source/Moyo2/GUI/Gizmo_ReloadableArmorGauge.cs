using Verse.Sound;
using Verse.Steam;

namespace Moyo2
{
	public class Gizmo_ReloadableArmorGauge : Gizmo_Slider
	{
		private readonly Comp_ReloadableArmor reloadableArmor;
		private static bool draggingBar;


		protected override float Target
		{
			get => reloadableArmor.TargetDurability / reloadableArmor.Props.maxDurability;
			set => reloadableArmor.TargetDurability = value * reloadableArmor.Props.maxDurability;
		}
		protected override float ValuePercent => reloadableArmor.MaxDurabilityPercent;
		protected override string Title => reloadableArmor.Props.gizmoLabel;
		protected override bool IsDraggable => true;
		protected override string BarLabel => reloadableArmor.ArmorDurability.ToStringDecimalIfSmall() + " / " + reloadableArmor.Props.maxDurability.ToStringDecimalIfSmall();
		protected override bool DraggingBar
		{
			get => draggingBar;
			set => draggingBar = value;
		}



		public Gizmo_ReloadableArmorGauge(Comp_ReloadableArmor reloadableArmor)
		{
			this.reloadableArmor = reloadableArmor;
		}


		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			if (SteamDeck.IsSteamDeckInNonKeyboardMode)
			{
				return base.GizmoOnGUI(topLeft, maxWidth, parms);
			}
			KeyCode keyCode = (KeyBindingDefOf.Command_ItemForbid != null) ? KeyBindingDefOf.Command_ItemForbid.MainKey : KeyCode.None;
			if (keyCode != 0 && !GizmoGridDrawer.drawnHotKeys.Contains(keyCode) && KeyBindingDefOf.Command_ItemForbid.KeyDownEvent)
			{
				ToggleAutoRefuel();
				Event.current.Use();
			}
			return base.GizmoOnGUI(topLeft, maxWidth, parms);
		}


		protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
		{
			headerRect.xMax -= 24f;
			Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
			GUI.DrawTexture(rect, reloadableArmor.Props.armor.GetUIIconForStuff(null));
			GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), reloadableArmor.allowRefuel ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
			if (Widgets.ButtonInvisible(rect))
			{
				ToggleAutoRefuel();
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, RefuelTip, 828267373);
				mouseOverElement = true;
			}
			base.DrawHeader(headerRect, ref mouseOverElement);
		}


		private void ToggleAutoRefuel()
		{
			reloadableArmor.allowRefuel = !reloadableArmor.allowRefuel;
			if (reloadableArmor.allowRefuel)
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
		}


		private string RefuelTip()
		{
			string text = string.Format("{0}", "CommandToggleAllowAutoRefuel".Translate()) + "\n\n";
			string str = reloadableArmor.allowRefuel ? "On".Translate() : "Off".Translate();
			string text2 = reloadableArmor.TargetDurability.ToString("F0").Colorize(ColoredText.TipSectionTitleColor);
			string text3 = string.Concat(text + "CommandToggleAllowAutoRefuelDesc".Translate(text2, str.UncapitalizeFirst().Named("ONOFF")).Resolve(), "\n\n");
			string text4 = KeyPrefs.KeyPrefsData.GetBoundKeyCode(KeyBindingDefOf.Command_ItemForbid, KeyPrefs.BindingSlot.A).ToStringReadable();
			return text3 + ("HotKeyTip".Translate() + ": " + text4);
		}


		protected override string GetTooltip()
		{
			return "";
		}
	}
}
