﻿namespace Moyo2
{
	public class FishDef : ThingDef
	{
		public FishTankSettings fishTankSettings;
	}

	public class FishTankSettings
	{
		public int ticksToGrow; // ticks that the fish needs to grow
		public GraphicData graphicData; // This is where the texture of the fish growing inside the fish tank goes, not the fish itself.
		public string label; // Name that will show on the fish tank
		public int ticksToDie = 15000; // Ticks that need to pass for the fish to die when the fish tank doesnt have power
									   // IMPORTANT: Use Graphic_Single
	}
}
