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
using Spine;
namespace RHA_Merankori
{
    /// <summary>
    /// 逆流的燐焰晶
    /// “湮裂的燐焰晶”会根据队友已损失的体力值增加，每1点增加1%伤害。
    /// 回合结束时，若手中有“湮裂的燐焰晶”，移除这个效果。
    /// </summary>
    public class B_Backflow : 
        Buff, 
        IP_HPChange1, 
        IP_ParticleOut_Before,
        IP_Draw
    {

        public override string DescExtended()
        {
            return base.DescExtended();
        }

        public override string DescInit()
        {
            return base.DescInit()
                .Replace("&a", Utils.GetAliveAlliesTotalLoseHP().ToString() + "%");
        }

        public override void Init()
        {
            base.Init();
        }

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            foreach (var skill in Utils.GetAllSkillsInBattle())
            {
                AddSkillExtend(skill);
            }
        }

        public IEnumerator Draw(Skill Drawskill, bool NotDraw)
        {
            AddSkillExtend(Drawskill);
            yield break;
        }

        public void HPChange1(BattleChar Char, bool Healed, int PreHPNum, int NewHPNum)
        {
            //“湮裂的燐焰晶”会根据队友已损失的体力值增加，每1点增加1%伤害。

        }

        public void ParticleOut_Before(Skill SkillD, List<BattleChar> Targets)
        {
            AddSkillExtend(SkillD);
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            if (this.BChar.Info.Ally)
            {
                // 回合结束时，若手中有“湮裂的燐焰晶”，移除这个效果。
                if(Utils.ContainExtendSkillsInHand<S_Attack_All>())
                {
                    this.SelfDestroy();
                }
            }
        }

        public override void SelfdestroyPlus()
        {
            base.SelfdestroyPlus();
            foreach (var skill in Utils.GetAllSkillsInBattle())
            {
                if (skill.MySkill.Key == ModItemKeys.Skill_S_Attack_All)
                {
                    skill.ExtendedDelete(ModItemKeys.SkillExtended_SE_Backflow);
                }
                
            }
        }

        private void AddSkillExtend(Skill skill)
        {
            if(skill.MySkill.KeyID == ModItemKeys.Skill_S_Attack_All)
            {
                //skill.EnsureExtendSkill<SE_Backflow>();
                skill.EnsureExtendSkill(ModItemKeys.SkillExtended_SE_Backflow);
            }
        }
    }
}