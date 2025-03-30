using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{

    public interface IP_BeforeBuffAdd
    {
        void BeforeBuffAdd(
            BattleChar battleChar,
            ref string key, 
            ref BattleChar UseState, 
            ref bool hide, 
            ref int PlusTagPer, 
            ref bool debuffnonuser, 
            ref int RemainTime, 
            ref bool StringHide, 
            ref bool cancelbuff);
    }

    public class EmotionBuffSwitch
    {
        public static void SwitchToPanic(BattleChar battleChar)
        {
            if(IsPanic(battleChar))
            {
                return; //already panic
            }
            battleChar.BuffRemove(ModItemKeys.Buff_B_Calm, true);
            battleChar.BuffAdd(ModItemKeys.Buff_B_Panic, battleChar);

        }

        public static void SwitchToCalm(BattleChar battleChar)
        {
            if (IsCalm(battleChar))
            {
                return; //already panic
            }

            battleChar.BuffRemove(ModItemKeys.Buff_B_Panic, true);
            battleChar.BuffAdd(ModItemKeys.Buff_B_Calm, battleChar);
        }

        public static bool IsCalm(BattleChar battleChar)
        {
            return battleChar.BuffFind(ModItemKeys.Buff_B_Calm);
        }

        public static bool IsPanic(BattleChar battleChar)
        {
            return battleChar.BuffFind(ModItemKeys.Buff_B_Panic);
        }
    }
}
