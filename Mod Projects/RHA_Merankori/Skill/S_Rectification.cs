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
    public interface ICanMerankoriRectification
    {

    }

    /// <summary>
    /// 整流
    /// 可以将梅朗柯莉指向单体队友的专属技能更改为对“全体友军”释放。
    /// 弃牌时，抽取2个梅朗柯莉的专属技能。
    /// 冷静：抽1张卡
    /// </summary>
    public class S_Rectification : Merankori_BaseSkill,
        ICanMerankoriRectification
    {
        public override bool CanApplyCalm => true;
        public override bool CanApplyPanic => false;

        public const int DISCARD_DRAW_COUNT = 2;

        public override void Init()
        {
            base.Init();
            this.effectSetting = StateForVisualEffect.Calm;
        }

        public override void SkillTargetSingle(List<Skill> Targets)
        {
            base.SkillTargetSingle(Targets);
            foreach (Skill skill in Targets)
            {
                if(skill!=null)
                {
                    skill.EnsureExtendSkill(ModItemKeys.SkillExtended_SE_Rectification);
                }
            }
            //S_Attack_All.GenCardToHand(this.BChar);
            if(this.IsCalm())
            {
                this.BChar.MyTeam.Draw();
            }
        }

        public override bool SkillTargetSelectExcept(Skill skill)
        {
            if (skill != null)
            {
                bool isMerankoriSkill = skill.Master.Info.KeyData == ModItemKeys.Character_C_Merankori;
                if (!isMerankoriSkill)
                {
                    return true;
                }

                bool isAllyTarget =
                    skill.MySkill.Target.Key == GDEItemKeys.s_targettype_ally ||
                    skill.MySkill.Target.Key == GDEItemKeys.s_targettype_otherally;
                if (!isAllyTarget)
                {
                    return true;
                }

                bool isMerankoriSpecificSkill = skill.AllExtendeds.Any(x => x is ICanMerankoriRectification);
                if (!isMerankoriSpecificSkill)
                {
                    return true;
                }
            }
            return base.SkillTargetSelectExcept(skill);
        }

        public override void DiscardSingle(bool Click)
        {
            base.DiscardSingle(Click);
            for (int i = 0; i < DISCARD_DRAW_COUNT; i++)
            {
                BattleSystem.DelayInputAfter(this.Draw());
            }
        }

        private IEnumerator Draw()
        {
            Skill skill2 = BattleSystem.instance.AllyTeam.Skills_Deck.Find((Skill skill) => skill.Master.IsCharacterKey(ModItemKeys.Character_C_Merankori));
            if (skill2 == null)
            {
                BattleSystem.instance.AllyTeam.Draw();
            }
            else
            {
                yield return BattleSystem.instance.StartCoroutine(BattleSystem.instance.AllyTeam._ForceDraw(skill2, null));
            }
            yield break;
        }

        public override string DescInit()
        {
            return base.DescInit().Replace("&a", DISCARD_DRAW_COUNT.ToString());
        }
    }
}