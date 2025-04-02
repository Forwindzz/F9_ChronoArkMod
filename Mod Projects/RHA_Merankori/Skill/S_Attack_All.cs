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
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            // 除了梅朗柯莉自身
            Utils.RemoveTarget(IDs.ID_Character_Merankori, Targets);
            Debug.Log("----------\nMerankori Attack All!");

            // 效果：反射，将伤害转移给敌方，只有队友可以转移，敌人也可以转移的话，会死循环！
            List<BattleChar> refractionOwner = new List<BattleChar>();
            if(!IsRefractAttack())
            {
                Debug.Log($"Check Refract!");
                foreach (BattleChar target in Targets)
                {
                    if (target.BuffFind(ModItemKeys.Buff_B_Refraction) && target.Info.Ally)
                    {
                        refractionOwner.Add(target);
                    }
                }
            }
            else
            {
                Debug.Log($"Check non - Refract!");
            }


            // TODO: add other effect


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

            //TODO： 判定震荡冲击效果
            /*
            if (this.BChar.BuffFind(ModItemKeys.Buff_B_OverClocking))
            {
                foreach(BattleChar battleChar in Targets)
                {
                    battleChar.BuffAdd(ModItemKeys.Buff_B_Shock, this.BChar);
                }
                //如果当前没有新的追击产生，延迟移除buff，（即移除buff是最后一件事情）
                //一般情况下不会出现追击触发追击的情况
                Debug.Log($"Overclocking: Refraction count:{refractionOwner.Count}");
                if (refractionOwner.Count == 0)
                {
                    Debug.Log($"Remove IDs.ID_Buff_OverClocking!");
                    BattleSystem.DelayInputAfter(Co_DestoryBuffOverClocking(this.BChar));
                }
            }*/
            if (refractionOwner.Count == 0)
            {
                BattleSystem.DelayInputAfter(Co_AfterAttackAll());
            }
            /*
            // 给队友赋予不死效果
            var allyBC = BattleSystem.instance.AllyTeam.AliveChars_Vanish.Select(x=>x.Info.KeyData).ToList();
            foreach (BattleChar battleChar in Targets)
            {
                if(allyBC.Contains(battleChar.Info.KeyData))
                {
                    battleChar.BuffAdd(IDs.ID_Buff_NotDeadlyAtk, this.BChar);
                    //这个效果将会在一次SkillParticle动作后消失
                }
            }*/
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
            this.effectSetting = StateForVisualEffect.Panic;
        }

        protected override void OnEmotionCalm()
        {
            base.OnEmotionCalm();
            base.SkillParticleOff();
            this.NotCount = false;
        }

        protected override void OnEmotionPanic()
        {
            base.OnEmotionPanic();
            base.SkillParticleOn();
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
                //Skill newSkill = Skill.TempSkill(IDs.ID_Skill_AttackAll, this.BChar);
                //Skill newSkill = SkillD.CloneSkill(this.BChar);
                newSkill.FreeUse = true;
                newSkill.AP = 0;
                newSkill.NotCount = true;
                newSkill.PlusHit = true;
                newSkill.Disposable = true;
                Debug.Log($"Emit refract attack!");
                battleChar.UseSkill(newSkill, targetEnemy);
                //battleChar.ParticleOut(this.MySkill, newSkill, targetEnemy); 
                yield return new WaitForSeconds(1.0f);
            }
            Debug.Log($"Emit refract attack finished!");
            yield break;
        }
        /*
        private IEnumerator Co_DestoryBuffOverClocking(BattleChar bchar)
        {
            yield return new WaitForSeconds(0.1f);
            bchar.BuffRemove(ModItemKeys.Buff_B_OverClocking);
            Debug.Log($"Removed IDs.ID_Buff_OverClocking!");
            yield break;
        }*/

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
    }
}