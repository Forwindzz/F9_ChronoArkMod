using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{

    public interface IP_BeforeEmotionSwitch
    {
        /// <summary>
        /// Invoked before switch the emotion
        /// </summary>
        /// <param name="battleChar"></param>
        /// <param name="planToSwitchID">should be IDs.ID_Buff_Calm or IDs.ID_Buff_Panic</param>
        /// <param name="cancelSwitch">if true, the switch will be cancelled</param>
        void BeforeEmotionSwitch(BattleChar battleChar, string planToSwitchID, ref bool cancelSwitch);
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
