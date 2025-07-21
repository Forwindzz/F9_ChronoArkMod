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
    /// 蓄能
    /// 倒计时4
    /// 每行进1点倒计时，获得3层蓄能
    /// 慌张时，停止蓄能。
    /// 冷静时完成蓄能，将2张<color=#FF6767>湮裂的燐焰晶</color>放入手中
    /// 慌张：冷静下来，若完成蓄能，再抽取1个技能
    /// </summary>
    public class S_Charge : 
        Merankori_BaseSkill,
        IP_SkillCastingStart
    {
        // 方式：SkillCasting -> SkillUseSingle
        //                   -> SkillCastingQuit / SelfDestory
        public override bool CanApplyCalm => false;
        public override bool CanApplyPanic => true;

        private CastingSkill currentCastingSkill = null;
        private int initCountingDown = 0;
        private bool isPanicStart = false;

        private const int PER_TIME_STACK = 2;

        private bool IsInitedTempInfo => currentCastingSkill != null;

        public override void Init()
        {
            base.Init();
            ResetTempInfo();
        }

        private void ResetTempInfo()
        {
            //Debug.Log("Charge: reset temp!");
            currentCastingSkill = null;
            initCountingDown = 0;
            isPanicStart = false;
        }

        //start count down!
        private void RecordTempInfo(CastingSkill newCastingSkill)
        {
            //Debug.Log("Charge: record temp!");
            if (currentCastingSkill!=null)
            {
                Debug.Log("S_Charge casting not reset yet! But require to set new record");
            }
            currentCastingSkill = newCastingSkill;
            initCountingDown = currentCastingSkill.CastSpeed;
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            if(this.IsCalm())
            {
                FinishCastingReward();
            }
        }

        private void UpdateTempInfo()
        {
            if (currentCastingSkill==null)
            {
                return;
            }
            int currentCastingCount = Mathf.Max(0, currentCastingSkill.CastSpeed);
            int deltaCount = initCountingDown - currentCastingCount;
            /*
            if (deltaCount!=0)
            {
                Debug.Log($"Charge: update temp! {deltaCount}");
            }
            */
            
            if (deltaCount==0)
            {
                return;
            }
            else if(deltaCount<0)
            {
                //Debug.Log("S_Charge count is negative, counting is increase???");
                initCountingDown = currentCastingCount;
                return;
            }
            else
            {
                initCountingDown = currentCastingCount;
                this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, deltaCount * PER_TIME_STACK);
                return;
            }
        }

        private void PanicTempInfo()
        {
            if (currentCastingSkill == null)
            {
                return;
            }
            int currentCastingCount = Mathf.Max(0, currentCastingSkill.CastSpeed);
            int giveCount = currentCastingCount;
            if(giveCount==0)
            {
                return;
            }
            //this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, giveCount);
            initCountingDown = currentCastingCount;
        }

        private void FinishTempInfo()
        {
            if (currentCastingSkill == null)
            {
                return;
            }
            int deltaCount = Mathf.Max(currentCastingSkill.CastSpeed, initCountingDown);
            Debug.Log($"Charge: finish temp! {deltaCount}");
            if (deltaCount == 0)
            {
                ResetTempInfo();
                return;
            }
            else if (deltaCount < 0)
            {
                Debug.Log("S_Charge count is negative, counting is <0 ???");
                ResetTempInfo();
                return;
            }
            else
            {
                this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, deltaCount * PER_TIME_STACK);
                ResetTempInfo();
                return;
            }
        }

        private void UpdateInitCounting()
        {
            //this.Counting = 3;// 1 + Utils.CountAliveAllyBuffTypes() / 3;
        }

        
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (BattleSystem.instance == null)
            {
                return;
            }
            if (!IsInitedTempInfo)
            {
                UpdateInitCounting();
                return;
            }
            List<CastingSkill> castSkills = BattleSystem.instance.CastSkills;
            if (castSkills==null || castSkills.Count==0)
            {
                return;
            }
            if (castSkills.Contains(currentCastingSkill))
            {
                UpdateTempInfo();
                if(IsPanic())
                {
                    PanicTempInfo();
                    BattleSystem.instance.ActWindow.CastingWaste(currentCastingSkill);
                    BattleSystem.instance.CastSkills.Remove(currentCastingSkill);
                }
            }
            else
            {
                ResetTempInfo(); //the skill disappear!
            }
        }

        public void SkillCasting(CastingSkill ThisSkill)
        {
            if(this.IsPanic())
            {
                EmotionBuffSwitch.SwitchToCalm(this.BChar);
                isPanicStart = true;
            }
            else
            {
                isPanicStart = false;
            }
            RecordTempInfo(ThisSkill);
        }

        public void SkillCastingQuit(CastingSkill ThisSkill)
        {
            if(IsCalm())
            {
                FinishTempInfo();
                FinishCastingReward();
            }
            else
            {
                ResetTempInfo();
            }
        }

        private void FinishCastingReward()
        {
            S_Attack_All.GenCardToHand(this.BChar);
            S_Attack_All.GenCardToHand(this.BChar);
            if(isPanicStart)
            {
                this.BChar.MyTeam.Draw();
                isPanicStart = false;
            }
        }

        public override void SelfDestroy()
        {
            base.SelfDestroy();
            ResetTempInfo();
        }

        //时点并不会被调用，所以这里也不会被调用
        /*
        protected override void OnEmotionPanic()
        {
            base.OnEmotionPanic();
            Debug.Log($"detect panic! {IsInitedTempInfo}");
            if (IsInitedTempInfo)
            {
                if(BattleSystem.instance!=null)
                {
                    if(BattleSystem.instance.ActWindow!=null)
                    {
                        Debug.Log("waste current casting!");
                        BattleSystem.instance.ActWindow.CastingWaste(currentCastingSkill);
                    }
                    else
                    {
                        Debug.Log("remove current casting!");
                        BattleSystem.instance.CastSkills.Remove(currentCastingSkill);
                    }
                    
                }
                ResetTempInfo();
            }
        }*/


    }
}