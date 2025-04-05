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
    /// 抽取2个技能。
    /// 若指向的队友拥有至少5个正面效果，额外抽取1张牌。
    /// </summary>
    public class S_Retreat : Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            this.BChar.MyTeam.Draw(2);
            bool flag = false;
            foreach (BattleChar target in Targets)
            {
                if(target.GetBuffs(BattleChar.GETBUFFTYPE.BUFF,false,false).Count>=5)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                this.BChar.MyTeam.Draw(1);
            }
        }
    }
}