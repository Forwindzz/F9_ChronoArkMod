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
    public abstract class BaseMAtkAllCardBuff :
        BaseImpactCardBuff,
        IP_AfterMerankoriAttackAll
    {

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

        protected override bool CanApplyToSkill(Skill skill)
        {
            return skill.MySkill.KeyID == ModItemKeys.Skill_S_Attack_All;
        }
    }
}
