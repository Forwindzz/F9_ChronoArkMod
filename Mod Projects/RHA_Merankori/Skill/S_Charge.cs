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
    /// 冷却蓄能
    /// 倒计时4
    /// 在此技能倒计时期间，梅朗柯莉每次进入<color=#BAC8FF>冷静</color>状态时，获得4层蓄能。
    /// 若以<color=#BAC8FF>冷静</color>状态完成蓄能，将2张<color=#FF6767>湮裂的燐焰晶</color>放入手中。
    /// </summary>
    public class S_Charge :
        Merankori_BaseSkill,
        IP_SkillCastingStart
    {
        public override bool CanApplyCalm => false;
        public override bool CanApplyPanic => false;
        public override bool UseParticleEffect => false;

        public override void Init()
        {
            base.Init();
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            if (this.IsCalm())
            {
                FinishCastingReward();
            }
        }

        public void SkillCasting(CastingSkill ThisSkill)
        {
            if (ThisSkill.skill == this.MySkill)
            {
                this.BChar.BuffAdd(ModItemKeys.Buff_B_SCharge, this.BChar);
            }
        }

        public void SkillCastingQuit(CastingSkill ThisSkill)
        {
            if (ThisSkill.skill == this.MySkill)
            {
                this.BChar.BuffRemove(ModItemKeys.Buff_B_SCharge, this.BChar);
                if (IsCalm())
                {
                    FinishCastingReward();
                }
            }
        }

        private void FinishCastingReward()
        {
            S_Attack_All.GenCardToHand(this.BChar);
            S_Attack_All.GenCardToHand(this.BChar);
        }
    }
}