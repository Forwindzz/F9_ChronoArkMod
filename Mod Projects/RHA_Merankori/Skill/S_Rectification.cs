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
    /// 整流
    /// 指向梅朗柯莉“指向队友”或“其他友军”的技能时，改成“全体友军” 。
    /// 指向不符合条件的卡时，将一张“湮裂的燐焰晶”放入手中。
    /// </summary>
    public class S_Rectification : Skill_Extended
    {
        public override void SkillTargetSingle(List<Skill> Targets)
        {
            base.SkillTargetSingle(Targets);
            foreach (Skill skill in Targets)
            {
                if(skill!=null)
                {
                    bool isMerankoriSkill = skill.Master.Info.KeyData == ModItemKeys.Character_C_Merankori;
                    if(!isMerankoriSkill)
                    {
                        S_Attack_All.GenCardToHand(this.BChar);
                        continue;
                    }

                    bool isAllyTarget =
                        skill.MySkill.Target.Key == GDEItemKeys.s_targettype_ally ||
                        skill.MySkill.Target.Key == GDEItemKeys.s_targettype_otherally;
                    if(!isAllyTarget)
                    {
                        S_Attack_All.GenCardToHand(this.BChar);
                        continue;
                    }

                    bool isMerankoriSpecificSkill = IDs.List_MerankoriSkills.Contains(skill.MySkill.KeyID);
                    if (isMerankoriSpecificSkill) 
                    {
                        skill.EnsureExtendSkill(ModItemKeys.SkillExtended_SE_Rectification);
                    }
                    else
                    {
                        S_Attack_All.GenCardToHand(this.BChar);
                    }
                }
            }
        }
    }
}