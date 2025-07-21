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
    /// 每当一次性接受&a点以上的治疗量时，赋予1层燐色存护。
    /// 根据无法战斗抗性（&b%）百分比，减少治疗量阈值（已减少&c）。
    /// </summary>
    public class B_WarmFire : 
        Buff,
        IP_Healed,
        IMerankoriCanExtraStackBuff
    {

        //private int totalHeal = 0;
        private const int BASE_HEAL_THRESHOLD = 20;

        private float GetBaseThreshold()
        {
            return this.StackNum ==0? BASE_HEAL_THRESHOLD: BASE_HEAL_THRESHOLD / this.StackNum;
        }

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            //totalHeal = 0;
        }

        public override void SelfdestroyPlus()
        {
            base.SelfdestroyPlus();
            //totalHeal = 0;
        }

        public override void BuffStat()
        {
            base.BuffStat();
            //this.PlusStat.DeadImmune = 10 * base.StackNum;
        }

        public void Healed(BattleChar Healer, BattleChar HealedChar, int HealNum, bool Cri, int OverHeal)
        {
            if(HealedChar==this.BChar)
            {
                /*
                int maxhp = this.BChar.GetStat.maxhp;
                totalHeal += HealNum;
                int times = 0;
                if (totalHeal >= maxhp)
                {
                    times = totalHeal / maxhp;
                    totalHeal -= times * maxhp;
                }*/
                int threshold = GetThreshold();
                if(HealNum>=threshold)
                {
                    this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Shield, this.BChar, 1);
                }
                //this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Shield, this.BChar, times);
            }
        }

        public int GetThreshold()
        {
            return 
                Mathf.Max(
                1,
                (int)(Mathf.Max(
                    0, 
                    (100-this.BChar.GetStat.DeadImmune)) 
                * GetBaseThreshold() * 0.01f));
        }

        public override string DescInit()
        {
            int threshold = GetThreshold();
            return base.DescInit()
                .Replace("&a", threshold.ToString())
                .Replace("&b", this.BChar.GetStat.DeadImmune.ToString())
                .Replace("&c", (GetBaseThreshold() - threshold).ToString());

        }
    }
}