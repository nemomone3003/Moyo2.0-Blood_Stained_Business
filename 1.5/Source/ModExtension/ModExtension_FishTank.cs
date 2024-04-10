namespace Moyo2
{
    /// <summary>
    /// Allows us to give the fish tank a list of fish defs to grow in it.
    /// </summary>

    /* XML Example
     * 
     * <modExtensions>
     *   <li Class="Moyo2.ModExtension_FishTank">
     * 	   <FishDefs>
     * 	 	<li>Fish1_defName</li>
     * 	 	<li>Fish2_defName</li>
     * 	   </FishDefs>
     * 	 </li>
     * </modExtensions>
     */

    public class ModExtension_FishTank : DefModExtension
    {
        public List<FishDef> FishDefs; // List of fishdef that can be grown in the fishtank
    }
}
