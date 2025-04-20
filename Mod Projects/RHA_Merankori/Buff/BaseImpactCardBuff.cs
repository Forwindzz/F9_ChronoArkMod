using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHA_Merankori
{


    public abstract class BaseImpactCardBuff :
        Buff,
        IP_ParticleOut_Before,
        IP_Draw
    {
        //这里返回skill ext的Key
        protected abstract string ApplySkillExKey { get; }

        protected abstract bool CanApplyToSkill(Skill skill);

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            foreach (var skill in Utils.GetAllSkillsInBattle())
            {
                TryAddSkillExtend(skill);
            }
        }

        public IEnumerator Draw(Skill Drawskill, bool NotDraw)
        {
            TryAddSkillExtend(Drawskill);
            yield break;
        }

        public void ParticleOut_Before(Skill SkillD, List<BattleChar> Targets)
        {
            TryAddSkillExtend(SkillD);
        }

        public override void SelfdestroyPlus()
        {
            base.SelfdestroyPlus();
            foreach (var skill in Utils.GetAllSkillsInBattle())
            {
                if (CanApplyToSkill(skill))
                {
                    skill.ExtendedDelete(ApplySkillExKey);
                }

            }
        }

        private void TryAddSkillExtend(Skill skill)
        {
            if (CanApplyToSkill(skill))
            {
                skill.EnsureExtendSkill(ApplySkillExKey);
            }
        }
    }
}
