using HarmonyLib;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Entities;

namespace KitchenSuperCrane.Patches
{
    [HarmonyPatch]
    static class SetCraneStatus_Patch
    {
        static readonly Type TARGET_TYPE = typeof(SetCraneStatus);
        const bool IS_ORIGINAL_LAMBDA_BODY = false;
        const int LAMBDA_BODY_INDEX = 0;
        const string TARGET_METHOD_NAME = "OnUpdate";
        const string DESCRIPTION = "Allow Crane outside of restaurant"; // Logging purpose of patch

        const int EXPECTED_MATCH_COUNT = 1;

        static readonly List<OpCode> OPCODES_TO_MATCH = new List<OpCode>()
        {
            OpCodes.Ldloca, // Load Kitchen.SetCraneStatus/'<>c__DisplayClass1_0'
            OpCodes.Ldarg,
            OpCodes.Call,
            OpCodes.Brtrue,
            OpCodes.Ldarg,
            OpCodes.Call,
            OpCodes.Brfalse,
            OpCodes.Ldarg,
            OpCodes.Call,
            OpCodes.Ldc_I4,
            OpCodes.Ceq,
            OpCodes.Br
        };

        // null is ignore
        static readonly List<object> OPERANDS_TO_MATCH = new List<object>()
        {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            typeof(GenericSystemBase).GetMethod("Has", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null).MakeGenericMethod(typeof(SKitchenMarker)),
            null,
            null,
            null
        };

        static readonly List<OpCode> MODIFIED_OPCODES = new List<OpCode>()
        {
            OpCodes.Ldloca,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Call,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Br
        };

        // null is ignore
        static readonly List<object> MODIFIED_OPERANDS = new List<object>()
        {
            null,
            null,
            null,
            null,
            null,
            typeof(SetCraneStatus_Patch).GetMethod("ShouldRemoveCranes", BindingFlags.NonPublic | BindingFlags.Static),
            null,
            null,
            null,
            null,
            null,
            null
        };

        public static MethodBase TargetMethod()
        {
            Type type = IS_ORIGINAL_LAMBDA_BODY ? AccessTools.FirstInner(TARGET_TYPE, t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob{LAMBDA_BODY_INDEX}")) : TARGET_TYPE;
            return AccessTools.FirstMethod(type, method => method.Name.Contains(IS_ORIGINAL_LAMBDA_BODY ? "OriginalLambdaBody" : TARGET_METHOD_NAME));
        }

        static bool _shouldRemoveAllCranes = false;
        internal static SetCraneStatus system;
        internal static bool ShouldRemoveCranes()
        {
            return _shouldRemoveAllCranes;
        }

        [HarmonyPrefix]
        static void OnUpdate_Prefix(ref SetCraneStatus __instance)
        {
            system = __instance;

            //Main.LogInfo($"Practice = {__instance.HasSingleton<SPracticeMode>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_DURING_PRACTICE_ID)}");
            //Main.LogInfo($"Day = {__instance.HasSingleton<SIsDayTime>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_DURING_DAY_ID)}");
            //Main.LogInfo($"Prep = {__instance.HasSingleton<SIsNightTime>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_DURING_PREP_ID)}");
            //Main.LogInfo($"HQ = {__instance.HasSingleton<SFranchiseMarker>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_IN_HQ_ID)}");
            //Main.LogInfo($"Builder = {__instance.HasSingleton<SFranchiseBuilderMarker>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_IN_FRANCHISE_BUILDER_ID)}");

            _shouldRemoveAllCranes = false;
            if (__instance.HasSingleton<SIsNightTime>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_DURING_PREP_ID))
            {
                //Main.LogInfo("Disabled for prep");
                _shouldRemoveAllCranes = true;
            }
            else if (__instance.HasSingleton<SFranchiseMarker>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_IN_HQ_ID))
            {
                //Main.LogInfo("Disabled for HQ");
                _shouldRemoveAllCranes = true;
            }
            else if (__instance.HasSingleton<SFranchiseBuilderMarker>() && !Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_IN_FRANCHISE_BUILDER_ID))
            {
                //Main.LogInfo("Disabled for builder");
                _shouldRemoveAllCranes = true;
            }
            else if (__instance.HasSingleton<SIsDayTime>())
            {
                if (__instance.HasSingleton<SPracticeMode>())
                {
                    if (!Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_DURING_PRACTICE_ID))
                    {
                        //Main.LogInfo("Disabled for practice");
                        _shouldRemoveAllCranes = true;
                    }
                }
                else if (!Main.PrefManager.Get<bool>(Main.ALLOW_CRANE_DURING_DAY_ID))
                {
                    //Main.LogInfo("Disabled for day");
                    _shouldRemoveAllCranes = true;
                }
            }
            
            if (!_shouldRemoveAllCranes)
            {
                //Main.LogInfo("Crane allowed");
            }

        }

        [HarmonyPostfix]
        static void OnUpdate_Postfix(HashSet<Entity> ___PlayersToRefresh)
        {
            if (___PlayersToRefresh == null)
                return;

            foreach (Entity playerEntity in ___PlayersToRefresh)
            {
                Main.LogInfo($"Refresh Entity {playerEntity.Index}");
            }
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> OriginalLambdaBody_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Main.LogInfo($"{TARGET_TYPE.Name} Transpiler");
            if (!(DESCRIPTION == null || DESCRIPTION == string.Empty))
                Main.LogInfo(DESCRIPTION);
            List<CodeInstruction> list = instructions.ToList();

            int matches = 0;
            int windowSize = OPCODES_TO_MATCH.Count;
            for (int i = 0; i < list.Count - windowSize; i++)
            {
                for (int j = 0; j < windowSize; j++)
                {
                    if (OPCODES_TO_MATCH[j] == null)
                    {
                        Main.LogError("OPCODES_TO_MATCH cannot contain null!");
                        return instructions;
                    }

                    string logLine = $"{j}:\t{OPCODES_TO_MATCH[j]}";

                    int index = i + j;
                    OpCode opCode = list[index].opcode;
                    if (j < OPCODES_TO_MATCH.Count && opCode != OPCODES_TO_MATCH[j])
                    {
                        if (j > 0)
                        {
                            logLine += $" != {opCode}";
                            Main.LogInfo($"{logLine}\tFAIL");
                        }
                        break;
                    }
                    logLine += $" == {opCode}";

                    if (j == 0)
                        Debug.Log("-------------------------");

                    if (j < OPERANDS_TO_MATCH.Count && OPERANDS_TO_MATCH[j] != null)
                    {
                        logLine += $"\t{OPERANDS_TO_MATCH[j]}";
                        object operand = list[index].operand;
                        if (OPERANDS_TO_MATCH[j] != operand)
                        {
                            logLine += $" != {operand}";
                            Main.LogInfo($"{logLine}\tFAIL");
                            break;
                        }
                        logLine += $" == {operand}";
                    }
                    Main.LogInfo($"{logLine}\tPASS");

                    if (j == OPCODES_TO_MATCH.Count - 1)
                    {
                        Main.LogInfo($"Found match {++matches}");
                        if (matches > EXPECTED_MATCH_COUNT)
                        {
                            Main.LogError("Number of matches found exceeded EXPECTED_MATCH_COUNT! Returning original IL.");
                            return instructions;
                        }

                        // Perform replacements
                        for (int k = 0; k < MODIFIED_OPCODES.Count; k++)
                        {
                            int replacementIndex = i + k;
                            if (MODIFIED_OPCODES[k] == null || list[replacementIndex].opcode == MODIFIED_OPCODES[k])
                            {
                                continue;
                            }
                            OpCode beforeChange = list[replacementIndex].opcode;
                            list[replacementIndex].opcode = MODIFIED_OPCODES[k];
                            Main.LogInfo($"Line {replacementIndex}: Replaced Opcode ({beforeChange} ==> {MODIFIED_OPCODES[k]})");
                        }

                        for (int k = 0; k < MODIFIED_OPERANDS.Count; k++)
                        {
                            if (MODIFIED_OPERANDS[k] != null)
                            {
                                int replacementIndex = i + k;
                                object beforeChange = list[replacementIndex].operand;
                                list[replacementIndex].operand = MODIFIED_OPERANDS[k];
                                Main.LogInfo($"Line {replacementIndex}: Replaced operand ({beforeChange ?? "null"} ==> {MODIFIED_OPERANDS[k] ?? "null"})");
                            }
                        }
                    }
                }
            }

            Main.LogWarning($"{(matches > 0 ? (matches == EXPECTED_MATCH_COUNT ? "Transpiler Patch succeeded with no errors" : $"Completed with {matches}/{EXPECTED_MATCH_COUNT} found.") : "Failed to find match")}");
            return list.AsEnumerable();
        }
    }
}
