using GameDataEditor;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHA_Merankori
{
    [HarmonyPatch]
    static class UIPatch
    {
        [HarmonyPatch(
            typeof(UIComponent),
            "Start"
            )]
        [HarmonyPostfix]
        static void UIComponent_Start_Postfix(
            UIComponent __instance
            )
        {
            MerankoriShieldUI.CreateUIForAlly(__instance);
        }

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
        }
    }
}
