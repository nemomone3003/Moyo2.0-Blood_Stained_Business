using System.Reflection.Emit;

namespace Moyo2
{
    /// <summary>
    /// Changes how the GenStep_PreciousLump generates the lump for Hadal spires.<br></br>
    /// Checks if forcedDefToScatter is the def we want, and if it is, we change the count of generated lumps to 3
    /// and the set the forced lump size to 1.<br></br>
    /// This spawns 3 individual spires instead of one lump of 3 spires close together.<br></br>
    /// kill me
    /// </summary>
    [HarmonyPatch(typeof(GenStep_PreciousLump), nameof(GenStep_PreciousLump.Generate))]
    internal static class GenStep_PreciousLump_Generate_Transpiler
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertLoop_Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilg)
            // ILGenerator is needed to move around labels
        {
            CodeMatcher codeMatcher = new(codeInstructions, ilg);


            // The instructions we want to match
            var instructionsToMatch = new CodeMatch[]
            {
                /*
                 * These four instructions are equivalent to this line in GenStep_PreciousLump.Generate(), just right at the end:
                 * 
                 * base.Generate(map, parms);
                 */
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Call),
            };


            codeMatcher.MatchStartForward(instructionsToMatch);
            // CodeMatcher rests at the first position (first one) if it finds something that matches 'instructionsToMatch'


            codeMatcher.CreateLabel(out var label);
            // Creates a sticker on the instruction CodeMatcher is resting at, that can be used in jump instructions to jump to the label. 


            // Our instructions we want to insert
            var instructionsToInsert = new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                // keyword -this-, because we're loading a field on the class, and not on the method
                new(OpCodes.Ldfld, AccessTools.Field(typeof(GenStep_ScatterLumpsMineable), nameof(GenStep_ScatterLumpsMineable.forcedDefToScatter))),
                // Loads the field GenStep_ScatterLumpsMineable.forcedDefToScatter into the stack, the def that's going to get scattered
                new(OpCodes.Ldsfld, AccessTools.Field(typeof(Moyo2_ThingDefOfs), nameof(Moyo2_ThingDefOfs.Moyo2_HadalSpire))),
                // Loads the static field Moyo2_ThingDefOfs.HadalSpire into the stack, the def that we're checking to change the generation behavior.
                new(OpCodes.Ceq),
                // Check equals. if forcedDefToScatter == HadalSpire.
                new(OpCodes.Brfalse, label),
                // If that check returns false, we jump to the label we created before, to the end of the method.
                // If it's true it keeps going down, reading the next instructions
                new(OpCodes.Ldarg_0),
                // keyword -this-
                new(OpCodes.Ldc_I4, Moyo2_HarmonySettingsDefOf.Moyo2_HarmonySettings.numberOfHadalSpireLumps),
                // Creates a new constant int with whatever value we've specified in the XML (defaults to 3)
                new(OpCodes.Stfld, AccessTools.Field(typeof(GenStep_Scatterer), nameof(GenStep_Scatterer.count))),
                // Saves that int in the field GenStep_Scatterer.count
                new(OpCodes.Ldarg_0),
                // keyword -this-
                new(OpCodes.Ldc_I4, Moyo2_HarmonySettingsDefOf.Moyo2_HarmonySettings.hadalSpiresPerLump),
                // Creates a new constant int with whatever value we've specified in the XML (defaults to 1)
                new(OpCodes.Stfld, AccessTools.Field(typeof(GenStep_ScatterLumpsMineable), nameof(GenStep_ScatterLumpsMineable.forcedLumpSize))),
                // Saves that int in the field GenStep_ScatterLumpsMineable.forcedLumpSize
            };


            if (codeMatcher.IsInvalid)
            {
                // If it's invalid, it warns you and returns the regular instructions of the method
                Log.Error("GenStep_PreciousLump_Generate_Transpiler couldn't patch it's intended method, report this to the Moyo mod.");
                return codeInstructions;
            }
            else
            {
                // CodeMatcher will insert all my instructions before the one i'm resting on, so no need to advance.
                codeMatcher.Insert(instructionsToInsert);
                return codeMatcher.InstructionEnumeration(); // And return the modified instructions to make the transpiler take effect
            }
        }
    }
}
