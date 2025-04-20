using GameDataEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{

    public interface IMerankoriStateInfo
    {
        bool CanApplyCalm { get; }
        bool CanApplyPanic { get; }
    }

    public abstract class Merankori_BaseSkill : 
        Skill_Extended, 
        IP_BuffAdd,
        IP_BuffAddAfter,
        IMerankoriStateInfo,
        ICanMerankoriRectification
    {

        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            if (addedbuff.BuffData.Key == ModItemKeys.Buff_B_Panic)
            {
                UpdateCalmState(false);
            }
            else if (addedbuff.BuffData.Key == ModItemKeys.Buff_B_Calm)
            {
                UpdateCalmState(true);
            }
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            CheckEmotionState();
        }

        private void CheckEmotionState()
        {
            if (EmotionBuffSwitch.IsCalm(this.BChar))
            {
                UpdateCalmState(true);
            }
            else if (EmotionBuffSwitch.IsPanic(this.BChar))
            {
                UpdateCalmState(false);
            }
        }

        private bool hasChecked = false;

        private void UpdateCalmState(bool isCalmNow)
        {
            if(!hasChecked && BattleSystem.instance!=null)
            {
                hasChecked = true;
                BattleSystem.DelayInput(Co_CheckEmotion(isCalmNow));
            }
        }

        private IEnumerator Co_CheckEmotion(bool isCalmNow)
        {
            hasChecked = false;
            if (isCalmNow && effectSetting.HasFlag(StateForVisualEffect.Calm))
            {
                Debug.Log($"Particle on for {this.Name}");
                base.SkillParticleOn();
            }
            else if (effectSetting.HasFlag(StateForVisualEffect.Panic))
            {
                Debug.Log($"Particle on for {this.Name}");
                base.SkillParticleOn();
            }
            else
            {
                Debug.Log($"Particle off for {this.Name}");
                base.SkillParticleOff();
            }
            Debug.Log($"base calm change {isCalmNow} {this.GetType().Name}");
            if (isCalmNow)
            {
                OnEmotionCalm();
            }
            else
            {
                OnEmotionPanic();
            }
            yield break;
        }

        [Flags]
        protected enum StateForVisualEffect
        {
            Panic=1,
            Calm=2
        }

        protected StateForVisualEffect effectSetting;

        public abstract bool CanApplyCalm { get; }
        public abstract bool CanApplyPanic { get; }

        protected virtual void OnEmotionCalm()
        {

        }

        protected virtual void OnEmotionPanic()
        {

        }

        public override void HandInit()
        {
            base.HandInit();
            CheckEmotionState();
        }

        public override string DescInit()
        {
            CheckEmotionState();
            return base.DescInit();
        }

        protected bool IsCalm()
        {
            return EmotionBuffSwitch.IsCalm(this.BChar);
        }

        protected bool IsPanic()
        {
            return EmotionBuffSwitch.IsPanic(this.BChar);
        }

        public void Buffadded(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff)
        {
            if (addedbuff.BuffData.Key == ModItemKeys.Buff_B_Panic)
            {
                UpdateCalmState(false);
            }
            else if (addedbuff.BuffData.Key == ModItemKeys.Buff_B_Calm)
            {
                UpdateCalmState(true);
            }
        }

        public override void Init()
        {
            base.Init();
            hasChecked = false;
            //this.SkillParticleObject = new GDESkillExtendedData(GDEItemKeys.SkillExtended_Public_1_Ex).Particle_Path;
            this.SkillParticleObject = new GDESkillExtendedData(GDEItemKeys.SkillExtended_MissChain_Ex_P).Particle_Path;
        }

        public override void SelfDestroy()
        {
            base.SelfDestroy();
            hasChecked = false;
        }
    }
}
