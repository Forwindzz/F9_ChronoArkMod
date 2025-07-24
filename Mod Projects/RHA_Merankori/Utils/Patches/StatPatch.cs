using HarmonyLib;
using RHA_Merankori;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Patches
{
    [HarmonyPatch]
    static class StatPatch
    {
        [HarmonyPatch(typeof(Character), "get_stat", MethodType.Getter)]
        public static class Patch_Character_get_stat
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                // 获取 StatC 方法引用
                var statCMethod = AccessTools.Method(typeof(Character), "StatC", new[] { typeof(Stat) });

                for (int i = 0; i < code.Count; i++)
                {
                    var ci = code[i];

                    // 找到 call Character::StatC(Stat)
                    if ((ci.opcode == OpCodes.Call || ci.opcode == OpCodes.Callvirt) &&
                        ci.operand is MethodInfo method &&
                        method == statCMethod)
                    {
                        // ------- 插入前置逻辑 -------
                        // 在调用 StatC 前插入 MyPrefix(this)
                        code.Insert(i, new CodeInstruction(OpCodes.Ldarg_0)); // this
                        code.Insert(i + 1, new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(Patch_Character_get_stat), nameof(MyPrefix))));

                        i += 2; // 移动下标

                        // ------- 找 stfld（StatC 的结果赋值给 G_get_stat） -------
                        for (int j = i + 1; j < code.Count; j++)
                        {
                            if (code[j].opcode == OpCodes.Stfld &&
                                code[j].operand is FieldInfo fi &&
                                fi.Name == "G_get_stat")
                            {
                                // 在 stfld 之后插入 MyPostfix(this)
                                code.Insert(j + 1, new CodeInstruction(OpCodes.Ldarg_0));
                                code.Insert(j + 2, new CodeInstruction(OpCodes.Call,
                                    AccessTools.Method(typeof(Patch_Character_get_stat), nameof(MyPostfix))));
                                break;
                            }
                        }

                        break; // 如果只插入一次就退出
                    }
                }

                return code;
            }

            //暂存中间值，因为是单线程逻辑这样写应该没事
            private static int tempDeadImmune = 0;

            public static void MyPrefix(Character __instance)
            {
                // 插入 StatC 调用前的逻辑
                tempDeadImmune = __instance.G_get_stat.DeadImmune;
            }

            public static void MyPostfix(Character __instance)
            {
                // 插入 StatC 调用后的逻辑
                BattleChar bChar = __instance.GetBattleChar;
                if (bChar!=null)
                {
                    if(bChar.BuffFind(ModItemKeys.Buff_B_DeadImmuneNoLimit)&& tempDeadImmune>80)
                    {
                        Stat newStat = __instance.G_get_stat;
                        //Debug.Log($"Try to modify Dead immune limit {__instance.Name}: {newStat.DeadImmune} -> {tempDeadImmune}");
                        newStat.DeadImmune = tempDeadImmune;
                        __instance.G_get_stat = newStat;
                    }
                }
            }
        }


    }
}
