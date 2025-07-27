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
using static Library_SpriteStudio6.Data.Animation.Attribute;
using HarmonyLib;
namespace RHA_Merankori
{
    /// <summary>
    /// 燐色存护
    /// 每层能抵抗1次不能战斗效果。
    /// 濒死状态时，无法战斗抗性（&a%）会增加受到的治疗量。
    /// 每20%全队的无法战斗抗性<color=#5061A4>（&b%）</color>增加1层上限。
    /// 
    /// 关于实现方式：
    /// Patch BattleChar.Dead
    /// - Prefix: 设置canJudgeReduceStack为true，表示此时的判定可以削减层数
    /// - Postfix：设置canJudgeReduceStack为false，表示此时判定结束
    /// 也就是只有在这个方法内可以缩减层数，在其他地方调用的IP_DeadResist则不会执行
    /// </summary>
    public class B_Shield :
        Buff,
        IP_DeadResist,
        IP_BattleEnd,
        IP_BattleEndField,
        IP_TurnEnd,
        IP_PlayerTurn,
        IP_BuffAddAfter,
        IP_BuffRemove,
        IP_ParticleOut_After_Global,
        IP_ParticleOut_Before,
        IP_DamageTake,
        IP_HealChange,
        IP_ParticleOut_After,
        IMerankoriCanExtraStackBuff
    {

        private bool canJudgeReduceStack = false;

        public void BattleEnd()
        {
            CheckRecover();
        }

        public void BattleEndField()
        {
            CheckRecover();
        }

        //因为DeadResist会在单次攻击内被多次调用，所以代码逻辑移动到了BuffPatch类里的BattleChar.Dead的Patch

        private bool isJudging = false;
        
        public bool DeadResist()
        {
            if(canJudgeReduceStack)
            {
                //如果别的地方也在通过遍历检查这个是否为true，那我们暂时返回false
                if(isJudging)
                {
                    Debug.Log(">>>> Detect Judge Loop! return false!");
                    return false;
                }
                isJudging = true;
                //先判断其他的DeadResist，如果有通过的，则跳过此次减层判定
                try
                {
                    foreach (IP_DeadResist ip_DeadResist in this.BChar.IReturn<IP_DeadResist>(null))
                    {
                        if (ip_DeadResist != null &&
                            !(ip_DeadResist is B_Shield) &&
                            ip_DeadResist.DeadResist())
                        {
                            canJudgeReduceStack = false;
                            isJudging = false;
                            Debug.Log($">>>> DeadResist {this.BChar.Info.KeyData}: other IP_DeadResist <{ip_DeadResist.GetType().FullName}> return true, skip judging.");
                            return true;
                        }
                    }
                }
                catch 
                { 
                }
                isJudging = false;

                Debug.Log($">>>> DeadResist {this.BChar.Info.KeyData}: reduce B_Shield!");
                canJudgeReduceStack = false;
                this.SelfStackDestroy();
            }
            else
            {
                Debug.Log($">>>> DeadResist {this.BChar.Info.KeyData}: just check");
            }
            return true;
        }

        public static bool Judge(BattleChar __instance)
        {
            /*
            if(__instance.BuffFind(ModItemKeys.Buff_B_NotDeadlyAtk) || 
                __instance.BuffFind(GDEItemKeys.Buff_B_Momori_P_NoDead))
            {
                return false;
            }
            */
            Buff shieldBuff = __instance.GetBuffByID(ModItemKeys.Buff_B_Shield);
            if (shieldBuff != null && shieldBuff.StackNum > 0)
            {
                if(shieldBuff is B_Shield bShield)
                {
                    bShield.canJudgeReduceStack = true;
                    //Debug.Log(">> Set B_Shield canJudgeReduceStack True!");
                    return true;
                }
                else
                {
                    Debug.LogWarning($">> Try to convert {shieldBuff.GetType().FullName} to B_Shield failed!");
                }
            }
            //Debug.Log(">> Cannot find B_Shield, Skip!");
            return true;
        }

        public static void JudgeEnd(BattleChar __instance)
        {
            Buff shieldBuff = __instance.GetBuffByID(ModItemKeys.Buff_B_Shield);
            if (shieldBuff != null && shieldBuff.StackNum > 0)
            {
                if (shieldBuff is B_Shield bShield)
                {
                    bShield.canJudgeReduceStack = false;
                    Debug.Log(">> Set B_Shield canJudgeReduceStack False!");
                    return;
                }
                else
                {
                    Debug.LogWarning($">> Try to convert {shieldBuff.GetType().FullName} to B_Shield failed!");
                }
            }
            Debug.Log(">> Cannot find B_Shield, Skip!");
        }

        public void Turn()
        {
            CheckRecover();
            BuffStat();
        }

        public void TurnEnd()
        {
            CheckRecover();
            BuffStat();
        }

        private void CheckRecover()
        {
            if (this.BChar.Recovery < 1)
            {
                this.BChar.Recovery = 1;
            }
        }

        
        public override void BuffStat()
        {
            base.BuffStat();
            if(this.BChar.BuffFind(GDEItemKeys.Buff_B_Neardeath))
            {
                this.PlusStat.HEALTaken = this.BChar.GetStat.DeadImmune;
            }
            else
            {
                this.PlusStat.HEALTaken = 0;
            }
        }

        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            BuffStat();
        }

        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            BuffStat();
        }

        public IEnumerator ParticleOut_After_Global(Skill SkillD, List<BattleChar> Targets)
        {
            CheckRecover();
            BuffStat();
            yield break;
        }

        public void ParticleOut_Before(Skill SkillD, List<BattleChar> Targets)
        {
            CheckRecover();
        }

        public void DamageTake(BattleChar User, int Dmg, bool Cri, ref bool resist, bool NODEF = false, bool NOEFFECT = false, BattleChar Target = null)
        {
            CheckRecover();
        }

        public void HealChange(BattleChar Healer, BattleChar HealedChar, ref int HealNum, bool Cri, ref bool Force)
        {
            CheckRecover();
        }

        public IEnumerator ParticleOut_After(Skill SkillD, List<BattleChar> Targets)
        {
            CheckRecover();
            yield break;
        }
        
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            CheckRecover(); //暴力解决各种边边角角的奇怪的死亡判定
            this.BuffData.MaxStack = GetAllDeathImmune() / 20 + 4;
        }


        public override string DescInit()
        {
            return base.DescInit()
                .Replace("&a", this.BChar.GetStat.DeadImmune.ToString())
                .Replace("&b", GetAllDeathImmune().ToString());

        }

        public int GetAllDeathImmune()
        {
            int total = 0;
            foreach(var b in this.BChar.MyTeam.AliveChars_Vanish)
            {
                total += b.GetStat.DeadImmune;
            }
            return total;
        }

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            // 彩蛋：凤凰获得不死效果时，会表示不屑，除非它的被动失效了...
            if(this.BChar.IsCharacterKey(GDEItemKeys.Character_Phoenix))
            {
                if(this.BChar.Info.Passive!=null)
                {
                    this.BChar.ShowAllyBattleTextRandom(ModLocalization.TSB_Phoenix_FirstShield);
                }
                else
                {
                    this.BChar.ShowAllyBattleTextRandom(ModLocalization.TSB_Phoenix_NoPassive_Shield);
                }
            }
        }
    }
}