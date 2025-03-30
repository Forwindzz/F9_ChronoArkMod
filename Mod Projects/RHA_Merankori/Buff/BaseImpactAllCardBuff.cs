using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHA_Merankori
{
    /// <summary>
    /// 当获得buff时，给所有“湮裂的燐焰晶”加上指定的Skill_Extend
    /// 这个extend每张卡只能添加1次，不能添加多次
    /// 当有1张卡使用后，移除这个效果1层
    /// </summary>
    public abstract class BaseImpactAllCardBuff :
        Buff,
        IP_ParticleOut_Before,
        IP_Draw,
        IP_AfterMerankoriAttackAll
    {
        //这里返回skill ext的Key
        protected abstract string ApplySkillExKey { get; }

        public void AfterMerankoriAttackAll(S_Attack_All skill_Extended)
        {
            if(skill_Extended.MySkill==null)
            {
                return;
            }
            if(skill_Extended.MySkill.Master==this.BChar)
            {
                this.SelfStackDestroy();
            }
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

        public void ParticleOut_Before(Skill SkillD, List<BattleChar> Targets)
        {
            AddSkillExtend(SkillD);
        }

        public override void SelfdestroyPlus()
        {
            base.SelfdestroyPlus();
            foreach (var skill in Utils.GetAllSkillsInBattle())
            {
                if (skill.MySkill.Key == ModItemKeys.Skill_S_Attack_All)
                {
                    skill.ExtendedDelete(ApplySkillExKey);
                }

            }
        }

        private void AddSkillExtend(Skill skill)
        {
            if (skill.MySkill.KeyID == ModItemKeys.Skill_S_Attack_All)
            {
                skill.EnsureExtendSkill(ApplySkillExKey);
            }
        }
    }
}
