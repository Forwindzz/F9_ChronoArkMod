using GameDataEditor;
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
    static class UIPatch
    {
        // note: UIComponent.MybChar is always null, do not use it!
        // Instead, use BattleChar.UI 

        [HarmonyPatch(
            typeof(BattleChar),
            nameof(BattleChar.UIUpdate)
            )]
        [HarmonyPostfix]
        static void BattleChar_UIUpdate_Postfix(
            BattleChar __instance
            )
        {
            MerankoriShieldUI.OnUIUpdate(__instance);
            MerankoriEmotionBattleFaceUI.OnUpdateUI(__instance);
        }

        [HarmonyPatch(
            typeof(BattleTeam),
            nameof(BattleTeam.init)
            )]
        [HarmonyPostfix]
        static void BattleTeam_init_Postfix(
            List<BattleChar> InputChars, bool Field,
            BattleTeam __instance
            )
        {
            // note: do not use InputChars, it sometimes is null, and usually inited character list is not here.
            // use __instance.Chars instead
            if(__instance==null)
            {
                return;
            }
            List<BattleChar> chars = __instance.Chars;
            if (chars == null)
            {
                return;
            }
            foreach (var battleChar in chars)
            {
                if (battleChar == null)
                {
                    continue;
                }
                MerankoriEmotionBattleFaceUI.TryToCreate(battleChar);
                MerankoriShieldUI.CreateUIForAlly(battleChar);
            }
        }
    }
}
