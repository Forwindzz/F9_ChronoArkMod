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
    /// 丢弃选择的技能，抽取1个技能。
    /// 丢弃技能的拥有者获得2层频移微调。
    /// <color=#FFC5BA><b>慌张</b>：将1张<color=#FF6767>湮裂的燐焰晶</color>放入手中。</color>
    /// <color=#BAC8FF><b>冷静</b>：赋予此技能迅速</color>
    /// </summary>
    public class S_ConvertCard : Merankori_BaseSkill,
        ICanMerankoriRectification
    {
        public override bool CanApplyCalm => true;
        public override bool CanApplyPanic => true;
        public override bool UseParticleEffect => false;

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
                if (targetSkill.Master!=null)
                {
                    BattleChar master = targetSkill.Master;
                    master.BuffAdd(ModItemKeys.Buff_B_FreqShift, this.BChar);
                    master.BuffAdd(ModItemKeys.Buff_B_FreqShift, this.BChar);
                }
                targetSkill.Delete(false);
                this.BChar.MyTeam.Draw(1);
                if(EmotionBuffSwitch.IsPanic(this.BChar))
                {
                    S_Attack_All.GenCardToHand(this.BChar, 1);
                }
            }
        }

        protected override void OnEmotionCalm()
        {
            base.OnEmotionCalm();
            this.NotCount = true;
        }

        protected override void OnEmotionPanic()
        {
            base.OnEmotionPanic();
            this.NotCount = false;
        }
    }
}