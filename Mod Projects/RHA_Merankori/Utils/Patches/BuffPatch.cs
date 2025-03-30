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

        [HarmonyPatch(typeof(BattleChar),nameof(BattleChar.BuffAdd))]
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
            if(BattleSystem.instance==null)
            {
                return true;
            }
            bool cancelBuff = false;
            foreach (var ip in BattleSystem.instance.IReturn<IP_BeforeBuffAdd>())
            {
                if (ip != null)
                {
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
    }
}
