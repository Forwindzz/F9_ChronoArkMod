using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    /// <summary>
    /// 控制buff切换
    /// 机制：执行一个动作的时候开始切换，动作结束时切换。
    /// 如果有慌张动作，那么结果一定是慌张，无论有多少次冷静
    /// </summary>
    public class EmotionBuffSwitch
    {
        private static bool delayInputProcessing;
        private static bool delayInputIsCalm;

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
            if (!delayInputProcessing)
            {
                if(BattleSystem.instance==null)
                {
                    return;
                }
                delayInputProcessing = true;
                BattleSystem.DelayInput(DelayedBuffSwitch(battleChar));
            }
            delayInputIsCalm = false;

        }

        private static IEnumerator DelayedBuffSwitch(BattleChar battleChar)
        {
            if(delayInputIsCalm)
            {
                battleChar.BuffRemove(ModItemKeys.Buff_B_Panic, true);
                battleChar.BuffAdd(ModItemKeys.Buff_B_Calm, battleChar);
            }
            else
            {
                battleChar.BuffRemove(ModItemKeys.Buff_B_Calm, true);
                battleChar.BuffAdd(ModItemKeys.Buff_B_Panic, battleChar);
            }
            delayInputProcessing = false;

            yield break;
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

            if (!delayInputProcessing)
            {
                if (BattleSystem.instance == null)
                {
                    return;
                }
                delayInputProcessing = true;
                delayInputIsCalm = true;
                BattleSystem.DelayInput(DelayedBuffSwitch(battleChar));
            }

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
