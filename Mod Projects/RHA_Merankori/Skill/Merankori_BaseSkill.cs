using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    public class Merankori_BaseSkill : 
        Skill_Extended, 
        IP_BuffAdd,
        IP_BuffAddAfter
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
            if (EmotionBuffSwitch.IsCalm(this.BChar))
            {
                UpdateCalmState(true);
            }
            else if (EmotionBuffSwitch.IsPanic(this.BChar))
            {
                UpdateCalmState(false);
            }
        }

        private void UpdateCalmState(bool isCalmNow)
        {
            if (isCalmNow && effectSetting.HasFlag(StateForVisualEffect.Calm))
            {
                base.SkillParticleOn();
            }
            else if(effectSetting.HasFlag(StateForVisualEffect.Panic))
            {
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
        }

        [Flags]
        protected enum StateForVisualEffect
        {
            Panic=0,
            Calm=1
        }

        protected StateForVisualEffect effectSetting;

        protected virtual void OnEmotionCalm()
        {

        }

        protected virtual void OnEmotionPanic()
        {

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
    }
}
