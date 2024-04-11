namespace Moyo2
{
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
     *     <amountPerHarvest>6</amountPerHarvest>
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
        // IMPORTANT: Use Graphic_Single
        public PawnKindDef pawnKindDef; // Fish kinddef to spawn the inner pawn from
    }
}
