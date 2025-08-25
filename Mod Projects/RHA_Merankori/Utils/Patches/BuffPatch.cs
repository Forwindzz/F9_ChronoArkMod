using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{


    [HarmonyPatch]
    static class BuffPatch
    {

        [HarmonyPatch(typeof(BattleChar), nameof(BattleChar.BuffAdd))]
        [HarmonyPrefix]
        static bool BattleChar_BuffAdd_Prefix(
            BattleChar __instance,
            ref string key,
            ref BattleChar UseState,
            ref bool hide,
            ref int PlusTagPer,
            ref bool debuffnonuser,
            ref int RemainTime,
            ref bool StringHide)
        {
            if (BattleSystem.instance == null)
            {
                return true;
            }
            bool cancelBuff = false;
            foreach (var ip in BattleSystem.instance.IReturn<IP_BeforeBuffAdd>())
            {
                if (ip != null)
                {
                    //Debug.Log("Check buff cancel");
                    ip.BeforeBuffAdd(
                        __instance,
                        ref key,
                        ref UseState,
                        ref hide,
                        ref PlusTagPer,
                        ref debuffnonuser,
                        ref RemainTime,
                        ref StringHide,
                        ref cancelBuff);
                }
            }
            return !cancelBuff;
        }

        [HarmonyPatch(typeof(BattleChar), nameof(BattleChar.Dead))]
        [HarmonyPrefix]
        static bool BattleChar_Dead_Prefix(
            BattleChar __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"Judge BattleChar.Dead {__instance.Info.KeyData}");
            bool result = B_Shield.Judge(__instance);
            return result;
        }

        [HarmonyPatch(typeof(BattleAlly), nameof(BattleAlly.Dead))]
        [HarmonyPrefix]
        static bool BattleAlly_Dead_Prefix(
            BattleAlly __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"Judge BattleChar.Dead {__instance.Info.KeyData}");
            bool result = B_Shield.Judge(__instance);
            return B_Shield.Judge(__instance);
        }

        //-------------------

        [HarmonyPatch(typeof(BattleAlly), nameof(BattleAlly.Dead))]
        [HarmonyPostfix]
        static void BattleAlly_Dead_Postfix(
            BattleAlly __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"JudgeEnd BattleAlly.Dead {__instance.Info.KeyData}");
            B_Shield.JudgeEnd(__instance);
        }

        [HarmonyPatch(typeof(BattleChar), nameof(BattleAlly.Dead))]
        [HarmonyPostfix]
        static void BattleChar_Dead_Postfix(
            BattleAlly __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"JudgeEnd BattleAlly.Dead {__instance.Info.KeyData}");
            B_Shield.JudgeEnd(__instance);
        }


    }
}
