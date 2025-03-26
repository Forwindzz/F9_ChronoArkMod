using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHA_Merankori
{


    [HarmonyPatch]
    static class BuffPatch
    {

        [HarmonyPatch(typeof(BattleChar),nameof(BattleChar.BuffAdd))]
        [HarmonyPrefix]
        static bool BattleChar_BuffAdd_Prefix(
            BattleChar __instance,
            string key, BattleChar UseState, bool hide, 
            int PlusTagPer, bool debuffnonuser, 
            int RemainTime, bool StringHide)
        {
            if(key== ModItemKeys.Buff_B_Panic)
            {
                return CheckCancel(__instance, key);
            }
            else if(key == ModItemKeys.Buff_B_Calm)
            {
                return CheckCancel(__instance, key);
            }
            return true;
        }

        private static bool CheckCancel(BattleChar __instance, string key)
        {
            bool cancelSwitch = false;
            Utils.InvokeAllIP<IP_BeforeEmotionSwitch>(
                ip => ip.BeforeEmotionSwitch(__instance, key, ref cancelSwitch));
            if (cancelSwitch)
            {
                return false;
            }
            return true;
        }
    }
}
