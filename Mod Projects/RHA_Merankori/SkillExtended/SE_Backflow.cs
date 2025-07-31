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
    /// 逆流的燐焰晶
    /// 根据队友已损失的体力值增加攻击力，每?点增加?%伤害
    /// </summary>
    public class SE_Backflow : Skill_Extended, IP_HPChange1
    {
        public override string DescInit()
        {
            return base.DescInit()
                .Replace("&a",B_Backflow.PER_LOSS_HP.ToString())
                .Replace("&b",B_Backflow.PER_INCREASE_PERCENT.ToString())
                .Replace("&c",B_Backflow.ComputeFinalIncreasePercent().ToString() + "%");
        }

        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc)
                .Replace("&a", B_Backflow.PER_LOSS_HP.ToString())
                .Replace("&b", B_Backflow.PER_INCREASE_PERCENT.ToString())
                .Replace("&c", B_Backflow.ComputeFinalIncreasePercent().ToString() + "%");
        }

        public void HPChange1(BattleChar Char, bool Healed, int PreHPNum, int NewHPNum)
        {
            UpdateData();
        }

        public override void Init()
        {
            base.Init();
            UpdateData();
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
        }

        private void UpdateData()
        {
            this.PlusPerStat.Damage = B_Backflow.ComputeFinalIncreasePercent();
        }

    }
}