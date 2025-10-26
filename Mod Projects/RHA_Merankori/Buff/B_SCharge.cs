using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using I2.Loc;
using DarkTonic.MasterAudio;
using ChronoArkMod;
using ChronoArkMod.Plugin;
using ChronoArkMod.Template;
using Debug = UnityEngine.Debug;
namespace RHA_Merankori
{
    /// <summary>
    /// 冷却蓄能的实现
    /// </summary>
    public class B_SCharge : Buff,
        IP_BuffAddAfter
    {

        private bool lastIsCalm = false;

        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            if(BuffTaker==this.BChar)
            {
                bool currentIsCalm = EmotionBuffSwitch.IsCalm(this.BChar);
                if(currentIsCalm && !lastIsCalm)
                {
                    lastIsCalm = true;
                    B_Charge.AddChargeStack(this.BChar, this.BChar, 4);
                    //this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, 4);
                }
                lastIsCalm = currentIsCalm;
            }
        }

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            lastIsCalm = EmotionBuffSwitch.IsCalm(this.BChar);
        }
    }
}