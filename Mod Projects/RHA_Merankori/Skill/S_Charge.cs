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
    /// 队友每拥有2层“燐色存护”，倒计时增加1点。每行进1点倒计时，获得1层蓄能。
    /// 进入慌张状态时，停止蓄能。
    /// </summary>
    public class S_Charge : 
        Merankori_BaseSkill,
        IP_SkillCastingStart,
        IP_SkillCastingQuit
    {
        // 方式：SkillCasting -> SkillUseSingle
        //                   -> SkillCastingQuit / SelfDestory


        private CastingSkill currentCastingSkill = null;
        private int initCountingDown = 0;

        private bool IsInitedTempInfo => currentCastingSkill != null;

        public override void Init()
        {
            base.Init();
            ResetTempInfo();
        }

        private void ResetTempInfo()
        {
            Debug.Log("Charge: reset temp!");
            currentCastingSkill = null;
            initCountingDown = 0;
        }

        private void RecordTempInfo(CastingSkill newCastingSkill)
        {
            Debug.Log("Charge: record temp!");
            if (currentCastingSkill!=null)
            {
                Debug.Log("S_Charge casting not reset yet! But require to set new record");
            }
            currentCastingSkill = newCastingSkill;
            initCountingDown = currentCastingSkill.CastSpeed;
        }

        private void UpdateTempInfo()
        {
            if (currentCastingSkill==null)
            {
                return;
            }
            int currentCastingCount = Mathf.Max(0, currentCastingSkill.CastSpeed);
            int deltaCount = initCountingDown - currentCastingCount;
            if (deltaCount!=0)
            {
                Debug.Log($"Charge: update temp! {deltaCount}");
            }
            
            if (deltaCount==0)
            {
                return;
            }
            else if(deltaCount<0)
            {
                Debug.Log("S_Charge count is negative, counting is increase???");
                initCountingDown = currentCastingCount;
                return;
            }
            else
            {
                initCountingDown = currentCastingCount;
                this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, deltaCount);
                return;
            }
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
                this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, deltaCount);
                ResetTempInfo();
                return;
            }
        }

        private void UpdateInitCounting()
        {
            if(IsCalm())
            {
                this.Counting = 2 + Utils.CountAliveAllyBuffStack(ModItemKeys.Buff_B_Shield) / 2;
            }
            else
            {
                this.Counting = 0;
            }
            
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
                    BattleSystem.instance.ActWindow.CastingWaste(currentCastingSkill);
                }
            }
            else
            {
                ResetTempInfo(); //the skill disappear!
            }
        }

        public void SkillCasting(CastingSkill ThisSkill)
        {
            RecordTempInfo(ThisSkill);
        }

        public void SkillCastingQuit(CastingSkill ThisSkill)
        {
            if(IsCalm())
            {
                FinishTempInfo();
            }
            else
            {
                ResetTempInfo();
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