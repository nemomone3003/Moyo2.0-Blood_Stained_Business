namespace Moyo2
{
#pragma warning disable CA1051

    /// <summary>
    /// This is a custom ThingDef made for the fish tank, due to having problems when saving data.<br></br>
    /// It inherits from ThingDef, which means anything that works in ThingDef works for this one, including stuff like ParentName="".<br></br>
    /// </summary>

    /* XML Example
     * 
     * <Moyo2.FishDef>
     * <!-- All the regular ThingDef fields, like defName, label, etc -->
     * 
     *   <fishTankSettings>
     *     <ticksToGrow>800</ticksToGrow>
     *     <pawnKindDef>fishDef_PawnKindDef</pawnKindDef>
     *     <graphicData>
     *       <texPath>Things/Pawn/Animal/Chicken/Chicken_east</texPath>
     *       <graphicClass>Graphic_Single</graphicClass>
     *     </graphicData>
     *   </fishTankSettings>
     * </Moyo2.FishDef>
     */

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
#pragma warning restore CA1051
}
