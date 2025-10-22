using ChronoArkMod;
using ChronoArkMod.Plugin;
using ChronoArkMod.Template;
using DarkTonic.MasterAudio;
using GameDataEditor;
using I2.Loc;
using NLog.Targets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
namespace RHA_Merankori
{

    public interface IP_AfterMerankoriAttackAll
    {
        void AfterMerankoriAttackAll(S_Attack_All skill_Extended);
    }

    /// <summary>
    /// 湮裂的燐焰晶
    /// 除了梅朗柯莉自身，对所有人（包括敌人和其他友军）造成300%攻击力的伤害。
    /// 这个伤害不会导致队友不能战斗。
    /// 不作为固定技能使用时，赋予放逐。
    /// 慌张：技能附加迅速。
    /// </summary>
    public class S_Attack_All : 
        Merankori_BaseSkill
    {
        public override bool CanApplyCalm => false;
        public override bool CanApplyPanic => true;
        public override bool UseParticleEffect => true;

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            // 除了梅朗柯莉自身
            Utils.RemoveTarget(ModItemKeys.Character_C_Merankori, Targets);
            //Debug.Log("----------\nMerankori Attack All!");

            // 效果：反射，将伤害转移给敌方，只有队友可以转移，敌人也可以转移的话，会死循环！
            List<BattleChar> refractionOwner = new List<BattleChar>();
            List<BattleChar> nonRefractionOwner = new List<BattleChar>();
            if(!IsRefractAttack())
            {
                //Debug.Log($"Check Refract!");
                foreach (BattleChar target in Targets)
                {
                    if (target.BuffFind(ModItemKeys.Buff_B_Refraction) && target.Info.Ally)
                    {
                        refractionOwner.Add(target);
                    }
                    else
                    {
                        nonRefractionOwner.Add(target);
                    }
                }
            }

            // remove targets, since they are refracted!
            foreach (BattleChar target in refractionOwner)
            {
                Targets.Remove(target);
            }

            // 赋予不死效果，这个其实也行，敌人不会通过这个效果判定
            foreach (BattleChar battleChar in Targets)
            {
                battleChar.BuffAdd(ModItemKeys.Buff_B_NotDeadlyAtk, this.BChar);
            }
            
            BattleSystem.DelayInput(Co_RefractDamage(refractionOwner));

            if (refractionOwner.Count == 0)
            {
                BattleSystem.DelayInputAfter(Co_AfterAttackAll());
            }

            BattleSystem.DelayInputAfter(Co_CheckChat(nonRefractionOwner));
        }

        private IEnumerator Co_CheckChat(List<BattleChar> nonRefractionOwner)
        {
            CheckChatOnAttack(nonRefractionOwner);
            yield break;
        }

        public override void HandInit()
        {
            base.HandInit();
            // 不作为固定技能使用时，赋予放逐
            if(this.MySkill!=null)
            {
                this.MySkill.isExcept = true;
            }
        }

        public override void Init()
        {
            base.Init();
            //Debug.Log($"[S_Attack_All] Init! {this.NotCount}");
            this.effectSetting = StateForVisualEffect.Panic;
        }

        public override void SelfDestroy()
        {
            //Debug.Log("[S_Attack_All] SelfDestroy!");
            base.SelfDestroy();
        }

        protected override void OnEmotionCalm()
        {
            //Debug.Log("[S_Attack_All] OnCalm!");
            base.OnEmotionCalm();
            base.SkillParticleOff();
            this.NotCount = false;
        }

        protected override void OnEmotionPanic()
        {
            //Debug.Log("[S_Attack_All] OnPanic!");
            base.OnEmotionPanic();
            base.SkillParticleOn();
            //Debug.Log($"[S_Attack_All] OnPanic before: NotCount = {this.NotCount} | Skill: {this.MySkill?.NotCount}");
            this.NotCount = true;
        }

        //==========================
        // 特殊效果附加

        private bool IsRefractAttack()
        {
            return this is S_D_RefractionAtk;
        }

        //折射伤害执行序列
        private IEnumerator Co_RefractDamage(List<BattleChar> refractionTargets)
        {
            yield return new WaitForSeconds(1.0f);
            foreach (BattleChar battleChar in refractionTargets)
            {
                List<BattleEnemy> enemyList = this.BChar.BattleInfo.EnemyList;
                if (enemyList.Count == 0)
                {
                    continue;// no target!
                }
                // 选择最低血量的敌人
                BattleEnemy targetEnemy = enemyList[0];
                int minHP = enemyList[0].HP;
                for (int i = 1; i > enemyList.Count; i++)
                {
                    BattleEnemy candidateEnemy = enemyList[i];
                    if (candidateEnemy.HP < minHP)
                    {
                        targetEnemy = candidateEnemy;
                        minHP = candidateEnemy.HP;
                    }
                }
                Skill newSkill = Skill.TempSkill(ModItemKeys.Skill_S_D_RefractionAtk, this.BChar);
                newSkill.FreeUse = true;
                newSkill.AP = 0;
                newSkill.NotCount = true;
                newSkill.PlusHit = true;
                newSkill.Disposable = true;
                //Debug.Log($"Emit refract attack!");
                this.BChar.UseSkill(newSkill, targetEnemy);
                yield return new WaitForSeconds(1.0f);
            }
            //Debug.Log($"Emit refract attack finished!");
            yield break;
        }

        private IEnumerator Co_AfterAttackAll()
        {
            yield return new WaitForSeconds(0.1f);
            Utils.InvokeAllIP<IP_AfterMerankoriAttackAll>(
                ip => ip.AfterMerankoriAttackAll(this));
            yield break;
        }

        public static void GenCardToHand(BattleChar BChar, int times = 1)
        {
            if(BChar==null || BChar.MyTeam==null)
            {
                return;
            }
            Skill skill = Skill.TempSkill(ModItemKeys.Skill_S_Attack_All, BChar, BChar.MyTeam);
            for (int i = 0; i < times; i++)
            {
                BChar.MyTeam.Add(skill.CloneSkill(), true);
            }
        }

        // UIPatch里有额外的逻辑代码
        public override void Special_SkillButtonPointerEnter()
        {
            base.Special_SkillButtonPointerEnter();
            GameObject toolTip = ToolTipWindow.ToolTip;
            Material shiningMaterial = ResUtils.LoadModAssetCached<Material>(IDs.Res_GlitterEffectMat);
            if (toolTip != null)
            {
                ToolTipWindow toolTipWindow = toolTip.GetComponentInChildren<ToolTipWindow>();
                SkillToolTip skillToolTip = toolTipWindow.GetComponentInChildren<SkillToolTip>();
                if (skillToolTip != null && skillToolTip.SkillImage != null) 
                {
                    skillToolTip.SkillImage.material = shiningMaterial;
                }
            }
            if(this.MySkill!=null)
            {
                SkillButton skillButton = this.MySkill.MyButton;
                if (skillButton != null)
                {
                    Image skillImage = skillButton.SkillImage;
                    if (skillImage != null)
                    {
                        skillImage.material = shiningMaterial;
                    }
                }
                BasicSkill basicSkillButton = this.MySkill.BasicSkillButton;
                if(basicSkillButton!=null)
                {
                    Image skillImage = basicSkillButton.SkillImage;
                    if (skillImage != null)
                    {
                        skillImage.material = shiningMaterial;
                    }
                }
            }
            

        }

        public override void Special_SkillButtonPointerExit()
        {
            if (this.MySkill != null)
            {
                base.Special_SkillButtonPointerExit();
                SkillButton skillButton = this.MySkill?.MyButton;
                if (skillButton != null)
                {
                    Image skillImage = skillButton.SkillImage;
                    if (skillImage != null)
                    {
                        skillImage.material = null;
                    }
                }

                BasicSkill basicSkillButton = this.MySkill.BasicSkillButton;
                if (basicSkillButton != null)
                {
                    Image skillImage = basicSkillButton.SkillImage;
                    if (skillImage != null)
                    {
                        skillImage.material = null;
                    }
                }
            }
        }





        //对话彩蛋==========

        public static void CheckChatOnAttack(List<BattleChar> beAttackedChar)
        {
            foreach(BattleChar bChar in beAttackedChar)
            {
                // 凤凰
                if (bChar.IsCharacterKey(GDEItemKeys.Character_Phoenix))
                {
                    int hp = bChar.HP;
                    if(hp<=-2000)
                    {
                        bChar.ShowAllyBattleTextRandom(
                            ModLocalization.TSB_Phoenix_LowHP_2000_Shield,
                            Mathf.Max(0.9f,(-2000 - hp) / 5000.0f + 0.5f));
                    }
                    else if(hp <= -500)
                    {
                        bChar.ShowAllyBattleTextRandom(
                            ModLocalization.TSB_Phoenix_LowHP_500_Shield,0.5f);
                    }
                    else if (hp <= 0)
                    {
                        bChar.ShowAllyBattleTextRandom(
                            ModLocalization.TSB_Phoenix_LowHP_0_Shield, 
                            Mathf.Max(0.5f, 0.25f + (-hp) / 500.0f * 0.25f));
                    }

                }
            }
            
        }
    }
}