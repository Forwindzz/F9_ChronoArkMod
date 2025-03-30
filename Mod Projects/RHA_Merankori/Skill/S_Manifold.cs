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
    /// 晶体流形
    /// 冷静：手中每个“湮裂的燐焰晶”会额外提供2层蓄力
    /// </summary>
    public class S_Manifold : 
        Merankori_BaseSkill,
        IP_Draw,
        IP_Discard
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            List<Skill> skills = BattleSystem.instance.AllyTeam.Skills;
            foreach (Skill skill in skills)
            {
                if(skill.ExtendedFind<S_Attack_All>()!= null)
                {
                    this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, 2);
                }
            }
        }

        public override string DescInit()
        {
            Check();
            return base.DescInit().Replace("&a", GetBonusStacks().ToString());
        }

        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc).Replace("&a", GetBonusStacks().ToString());
        }

        private int GetBonusStacks()
        {
            if(BattleSystem.instance==null || BattleSystem.instance.AllyTeam==null || BattleSystem.instance.AllyTeam.Skills==null)
            {
                return 0 ;
            }
            if(IsPanic())
            {
                return 0;
            }
            List<Skill> skills = BattleSystem.instance.AllyTeam.Skills;
            int total = 0;
            foreach (Skill skill in skills)
            {
                if (skill.ExtendedFind<S_Attack_All>() != null)
                {
                    total += 2;
                }
            }
            return total;
        }

        private void Check()
        {
            if(GetBonusStacks()>0)
            {
                this.SkillParticleOn();
            }
            else
            {
                this.SkillParticleOff();
            }
        }

        public IEnumerator Draw(Skill Drawskill, bool NotDraw)
        {
            Check();
            yield break;
        }

        public void Discard(bool Click, Skill skill, bool HandFullWaste)
        {
            Check();
        }
    }
}