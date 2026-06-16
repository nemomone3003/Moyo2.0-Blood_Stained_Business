global using HarmonyLib;
global using RimWorld;
global using System;
global using System.Collections.Generic;
global using UnityEngine;
global using Verse;

namespace Moyo2
{
	[StaticConstructorOnStartup]
	public static class Moyo2
	{
		static Moyo2()
		{
			Harmony harmony = new("Nemonian.Moyo2.0");
			harmony.PatchAll();
		}
	}
}
