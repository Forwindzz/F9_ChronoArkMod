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
    /// 暖焰
    /// 每当受到等体力值的治疗量，赋予1层燐色存护。
    /// </summary>
    public class B_WarmFire : 
        Buff,
        IP_Healed
    {

        private int totalHeal = 0;

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            totalHeal = 0;
        }

        public override void SelfdestroyPlus()
        {
            base.SelfdestroyPlus();
            totalHeal = 0;
        }

        public override void BuffStat()
        {
            base.BuffStat();
            this.PlusStat.DeadImmune = 5 * base.StackNum;
        }

        public void Healed(BattleChar Healer, BattleChar HealedChar, int HealNum, bool Cri, int OverHeal)
        {
            if(HealedChar==this.BChar)
            {
                int maxhp = this.BChar.GetStat.maxhp;
                totalHeal += HealNum;
                int times = 0;
                if (totalHeal >= maxhp)
                {
                    times = totalHeal / maxhp;
                    totalHeal -= times * maxhp;
                }
                this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Shield, this.BChar, times);
            }
        }

        public override string DescInit()
        {
            return base.DescInit().Replace("&a", totalHeal.ToString())
                .Replace("&b", this.BChar.GetStat.maxhp.ToString());
        }
    }
}