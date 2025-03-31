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
    /// 焰晶转化
    /// 选择的卡牌会被丢弃，然后将1张“湮裂的燐焰晶”放入手牌中，并额外抽1张卡。
    /// 弃牌技能的拥有者获得1层折射。
    /// </summary>
    public class S_ConvertCard : Skill_Extended
    {
        public override bool SkillTargetSelectExcept(Skill ExceptSkill)
        {
            if (ExceptSkill.ExtendedFind<S_ConvertCard>() == this) 
            {
                return true;
            }
            return base.SkillTargetSelectExcept(ExceptSkill);
        }

        public override void SkillTargetSingle(List<Skill> Targets)
        {
            base.SkillTargetSingle(Targets);
            foreach(var targetSkill in Targets)
            {
                if(targetSkill==null)
                {
                    continue;
                }
                Skill skill = Skill.TempSkill(ModItemKeys.Skill_S_Attack_All, this.BChar, this.BChar.MyTeam);
                this.BChar.MyTeam.Add(skill.CloneSkill(), true);
                if(targetSkill.Master!=null)
                {
                    targetSkill.Master.BuffAdd(ModItemKeys.Buff_B_Refraction, this.BChar);
                }
                targetSkill.Delete(false);
                this.BChar.MyTeam.Draw(1);
            }
        }
    }
}