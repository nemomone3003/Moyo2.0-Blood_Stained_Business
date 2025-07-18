global using System;
global using System.Collections.Generic;
global using RimWorld;
global using Verse;
global using UnityEngine;
global using HarmonyLib;

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
