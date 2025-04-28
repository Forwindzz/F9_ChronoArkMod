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
    /// 选择的技能会被丢弃，并额外抽1个技能，丢弃技能的拥有者获得1层折射。
    /// 如果有队友无法战斗抗性达到60%，则生成1张<color=#FF6767>湮裂的燐焰晶</color>到手中。
    /// </summary>
    public class S_ConvertCard : Skill_Extended,
        ICanMerankoriRectification,
        IP_BuffAddAfter,
        IP_BuffRemove,
        IP_TurnEnd
    {
        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            CheckParticle();
        }

        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            CheckParticle();
        }

        public void TurnEnd()
        {
            CheckParticle();
        }

        private void CheckParticle()
        {
            if(CanGenCard())
            {
                this.SkillParticleOn();
            }
            else
            {
                this.SkillParticleOff();
            }
        }

        public override void Init()
        {
            base.Init();
            this.SkillParticleObject = new GDESkillExtendedData(GDEItemKeys.SkillExtended_MissChain_Ex_P).Particle_Path;
        }

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
                    master.BuffAdd(ModItemKeys.Buff_B_Refraction, this.BChar);
                }
                targetSkill.Delete(false);
                this.BChar.MyTeam.Draw(1);

                if(CanGenCard())
                {
                    S_Attack_All.GenCardToHand(this.BChar, 1);
                }
            }
        }

        private bool CanGenCard()
        {
            var allies = this.BChar.MyTeam.AliveChars_Vanish;
            foreach (var ally in allies)
            {
                if (ally.GetStat.DeadImmune >= 15)
                {
                    return true;
                }
            }
            return false;
        }
    }
}