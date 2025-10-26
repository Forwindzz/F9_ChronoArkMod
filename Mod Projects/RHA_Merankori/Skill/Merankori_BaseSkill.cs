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

    //基础类，能够检测calm和panic并做出动画效果反应，不过有时候可能不起作用（卡牌的主人不是梅朗柯莉的时候，或者某些时点并没有将卡牌加入回调）
    public abstract class Merankori_BaseSkill :
        Skill_Extended,
        IP_BuffAddAfter,
        IP_ParticleOut_After_Global,
        IMerankoriStateInfo,
        ICanMerankoriRectification
    {
        private GameObject tinySkillViewEffect = null;
        private UIGasAnimator tinySkillViewAnimator = null;
        private const float tinyEffect_showFactor = 0.3f;
        private const float tinyEffect_moveInFactor = 0.7f;
        private const float tinyEffect_clickFactor = 1.0f;

        public UIGasAnimator TinySkillViewAnimator => tinySkillViewAnimator;
        private bool previousIsCalm = false;
        private bool previousIsPanic = false;

        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            //Debug.Log($"[MBaseSkill] @{this?.Name} BuffaddedAfter U={BuffUser?.Info?.Name} | T={BuffTaker?.Info?.Name} | {addedbuff?.BuffData?.Name}");

            if (BuffTaker != this.BChar)
            {
                return;
            }
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
            //Debug.Log($"[MBaseSkill] @{this?.Name} CheckEmotionState");

            if (IsCalm())
            {
                UpdateCalmState(true);
            }
            else if (IsPanic())
            {
                UpdateCalmState(false);
            }
        }

        private bool hasChecked = false;

        private void UpdateCalmState(bool isCalmNow)
        {
            if(previousIsCalm && isCalmNow)
            {
                return;
            }
            if(previousIsPanic && !isCalmNow)
            {
                return;
            }
            if(BattleSystem.instance==null)
            {
                return;
            }

            //Debug.Log($"[MBaseSkill] @{this?.Name} UpdateCalmState Checked:{hasChecked} | Cur={isCalmNow} | C={previousIsCalm} P={previousIsPanic}");

            if (!hasChecked)
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
                //Debug.Log($"Particle on for {this.Name}");
                SetTinySkillViewEffect(IDs.Res_TinySkillViewCalm, tinyEffect_showFactor);
                base.SkillParticleOn();
            }
            else if (!isCalmNow && effectSetting.HasFlag(StateForVisualEffect.Panic))
            {
                //Debug.Log($"Particle on for {this.Name}");
                SetTinySkillViewEffect(IDs.Res_TinySkillViewPanic, tinyEffect_showFactor);
                base.SkillParticleOn();
            }
            else
            {
                //Debug.Log($"Particle off for {this.Name}");
                RemoveTinySkillViewEffect();
                base.SkillParticleOff();
            }
            //Debug.Log($"base calm change {isCalmNow} {this.GetType().Name}");
            if (isCalmNow)
            {
                OnEmotionCalm();
                previousIsCalm = true;
                previousIsPanic = false;
            }
            else
            {
                OnEmotionPanic();
                previousIsCalm = false;
                previousIsPanic = true;
            }
            //Debug.Log($"[MBaseSkill] @{this?.Name} After Co_CheckEmotion Checked:{hasChecked} | Cur={isCalmNow} | C={previousIsCalm} P={previousIsPanic}");

            yield break;
        }

        [Flags]
        protected enum StateForVisualEffect
        {
            Panic = 1,
            Calm = 2
        }

        protected StateForVisualEffect effectSetting;

        public abstract bool CanApplyCalm { get; }
        public abstract bool CanApplyPanic { get; }
        public abstract bool UseParticleEffect { get; }

        protected virtual void OnEmotionCalm()
        {
            //Debug.Log("[MBaseSkill] OnCalm!");
        }

        protected virtual void OnEmotionPanic()
        {
            //Debug.Log("[MBaseSkill] OnPanic!");
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

        public override void Init()
        {
            base.Init();
            //Debug.Log($"[MBaseSkill] @{this?.Name} Init");
            hasChecked = false;
            previousIsCalm = false;
            previousIsPanic = false;
            if (CanApplyCalm)
            {
                effectSetting |= StateForVisualEffect.Calm;
            }
            if (CanApplyPanic)
            {
                effectSetting |= StateForVisualEffect.Panic;
            }
            if (UseParticleEffect)
            {
                this.SkillParticleObject = new GDESkillExtendedData(GDEItemKeys.SkillExtended_MissChain_Ex_P).Particle_Path;
            }
            CheckEmotionState();
        }

        public override void Special_SkillButtonPointerEnter()
        {
            base.Special_SkillButtonPointerEnter();
            SetTinySkillViewFactor(tinyEffect_moveInFactor);
            GameObject toolTip = ToolTipWindow.ToolTip;
            if (toolTip != null)
            {
                ToolTipWindow toolTipWindow = toolTip.GetComponentInChildren<ToolTipWindow>();
                if(toolTipWindow==null)
                {
                    return;
                }
                SkillToolTip skillToolTip = toolTipWindow.GetComponentInChildren<SkillToolTip>();
                if (skillToolTip != null && tinySkillViewEffect != null)
                {
                    GameObject effect = GameObject.Instantiate(tinySkillViewEffect, skillToolTip.Name.transform);
                    if (effect != null)
                    {
                        effect.transform.localPosition = new Vector3(-154.3f, 0, 0);
                    }
                }
            }
        }

        public override void SelfDestroy()
        {
            base.SelfDestroy();
            hasChecked = false;
        }

        public void SetTinySkillViewEffect(string effectPrefabPath, float factor = 0.5f)
        {
            RemoveTinySkillViewEffect();
            if (this.MySkill?.MyButton == null)
            {
                return;
            }
            tinySkillViewEffect = ResUtils.LoadModPrefab(effectPrefabPath);
            tinySkillViewEffect = GameObject.Instantiate(tinySkillViewEffect, this.MySkill.MyButton.transform);
            if (tinySkillViewEffect != null)
            {
                tinySkillViewAnimator = tinySkillViewEffect.GetComponentInChildren<UIGasAnimator>();
                if (tinySkillViewEffect != null)
                {
                    tinySkillViewAnimator.SetFactorSmooth(factor);
                }
                tinySkillViewEffect.transform.localPosition = new Vector3(-134.3f, 0, 0);
            }
        }

        public void RemoveTinySkillViewEffect()
        {
            if (tinySkillViewAnimator != null)
            {
                tinySkillViewAnimator.DestroySmooth();
                tinySkillViewAnimator = null;
            }
            else
            {
                GameObject.Destroy(tinySkillViewEffect);
            }
        }

        public void SetTinySkillViewFactor(float newFactor)
        {
            if (tinySkillViewAnimator != null)
            {
                tinySkillViewAnimator.SetFactorSmooth(newFactor);
            }
        }

        public override void Special_PointerExit()
        {
            base.Special_PointerExit();
            SetTinySkillViewFactor(tinyEffect_showFactor);
        }

        public override void Special_SkillButtonPointerExit()
        {
            base.Special_SkillButtonPointerExit();
            SetTinySkillViewFactor(tinyEffect_showFactor);
        }

        public override void Special_SkillButtonPointerClick()
        {
            base.Special_SkillButtonPointerClick();
            SetTinySkillViewFactor(tinyEffect_clickFactor);
        }

        public IEnumerator ParticleOut_After_Global(Skill SkillD, List<BattleChar> Targets)
        {
            //Debug.Log($"[MBaseSkill] @{this?.Name} ParticleOut_After_Global {SkillD?.AllExtendeds?.FirstOrDefault()?.Name}");
            CheckEmotionState();
            yield break;
        }
    }
}
