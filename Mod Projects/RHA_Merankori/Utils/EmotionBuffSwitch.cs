using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    public class EmotionBuffSwitch
    {
        public static void SwitchToPanic(BattleChar battleChar)
        {
            if(IsPanic(battleChar))
            {
                return; //already panic
            }
            if (IsLockState(battleChar))
            {
                battleChar.BuffAdd(ModItemKeys.Buff_B_Panic, battleChar);
                return;
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
            if(IsLockState(battleChar))
            {
                battleChar.BuffAdd(ModItemKeys.Buff_B_Calm, battleChar);
                return;
            }

            battleChar.BuffRemove(ModItemKeys.Buff_B_Panic, true);
            battleChar.BuffAdd(ModItemKeys.Buff_B_Calm, battleChar);
        }

        public static bool IsLockState(BattleChar battleChar)
        {
            return battleChar.BuffFind(ModItemKeys.Buff_B_DLoop);
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
