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
    /// 恢复全部体力极限。
    /// 每恢复&a体力极限，会赋予1层蓄力。
    /// </summary>
    public class S_ElementHeal : Skill_Extended,
        ICanMerankoriRectification
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            int totalHealNum = 0;
            int require = GetRequire();
            foreach (var Target in Targets)
            {
                if (Target.HP < Target.Recovery)
                {
                    int num = Target.Recovery - Target.HP;
                    totalHealNum+=Target.Heal(this.BChar, (float)num, false, false, null);
                }
            }
            int stacks = Mathf.Min(totalHealNum / require, B_Charge.maxChargeStack);
            B_Charge.AddChargeStack(this.BChar, this.BChar, stacks);
        }

        private int GetRequire()
        {
            return 8;// Mathf.Max(1, (int)(this.BChar.GetStat.atk * 2.0f));
        }

        private int GetPredictHeal()
        {
            if(BattleSystem.instance==null || BattleSystem.instance.AllyTeam==null)
            {
                return 0;
            }
            int totalHealNum = 0;
            foreach (var ally in BattleSystem.instance.AllyTeam.AliveChars_Vanish)
            {
                totalHealNum += ally.Recovery - ally.HP;
            }
            return totalHealNum;
        }

        private string ProcessDesc(string s)
        {
            
            return s.Replace("&a", GetRequire().ToString())
                .Replace("&b", (GetPredictHeal()/GetRequire()).ToString());
        }

        public override string DescInit()
        {
            return ProcessDesc(base.DescInit());
        }

        public override string DescExtended(string desc)
        {
            return ProcessDesc(base.DescExtended(desc));
        }
    }
}