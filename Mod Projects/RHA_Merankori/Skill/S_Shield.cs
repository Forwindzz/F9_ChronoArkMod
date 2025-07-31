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
    /// 燐晶电场 S_Shield
    /// 3费，全体队友
    /// 赋予2层“燐色存护”
    /// 下1次“湮裂的燐焰晶”会根据队友已损失的体力值增加，每5点增加4%伤害。
    /// 冷静：处于濒死状态的队友额外获得2层“燐色存护”。
    /// </summary>
    public class S_Shield: 
        Merankori_BaseSkill,
        IP_HPChange1,
        IP_HPZero
    {
        public override bool CanApplyCalm => true;
        public override bool CanApplyPanic => false;

        public void HPChange1(BattleChar Char, bool Healed, int PreHPNum, int NewHPNum)
        {
            CheckParticleEffect();
        }

        public void HPZero()
        {
            CheckParticleEffect();
        }

        public override void Init()
        {
            base.Init();
            this.effectSetting = StateForVisualEffect.Calm;
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            foreach (BattleChar b in Targets)
            {
                if (b.BuffFind(GDEItemKeys.Buff_B_Neardeath))
                {
                    b.BuffAddWithStacks(ModItemKeys.Buff_B_Shield, this.BChar, 2);
                }
            }
        }

        protected override void OnEmotionCalm()
        {
            base.OnEmotionCalm();
            CheckParticleEffect();
        }

        private void CheckParticleEffect()
        {
            if(Utils.IsAnyAllyDying() && IsCalm())
            {
                base.SkillParticleOn();
            }
            else
            {
                base.SkillParticleOff();
            }
        }
    }
}