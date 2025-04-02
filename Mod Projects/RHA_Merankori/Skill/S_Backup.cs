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
    /// 后备的燐焰晶
    /// 抽1张牌，
    /// 将“湮裂的燐焰晶”放入手中，直到填满手牌。
    /// </summary>
    public class S_Backup : Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            this.BChar.MyTeam.Draw(1);
            int num = 10 - BattleSystem.instance.AllyTeam.Skills.Count;
            S_Attack_All.GenCardToHand(this.BChar, num);
        }
    }
}