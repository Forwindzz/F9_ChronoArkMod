using ChronoArkMod;
using ChronoArkMod.Plugin;
using ChronoArkMod.Template;
using DarkTonic.MasterAudio;
using GameDataEditor;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
namespace RHA_Merankori
{
    /// <summary>
    /// 蓄能
    /// 根据蓄能效果层数，每层增加10%的伤害。
    /// </summary>
    public class SE_Charge :
        Skill_Extended,
        IP_BuffAddAfter,
        IP_BuffRemove
    {

        private void UpdateData()
        {
            if (this.MySkill == null)
            {
                return;
            }
            if (this.MySkill.Master == null)
            {
                return;
            }
            Buff buff = this.MySkill?.Master?.GetBuffByID(ModItemKeys.Buff_B_Charge);
            int stackCount = buff?.StackNum ?? 0;
            this.PlusPerStat.Damage = stackCount * 10;

            Skill_Extended mainExtend = this.MySkill.AllExtendeds.FirstOrDefault();
            if (mainExtend != null && mainExtend is Merankori_BaseSkill baseMerankoriSkill)
            {
                int maxStack = buff?.BuffData?.MaxStack ?? 1;
                baseMerankoriSkill.SetTinySkillViewFactor(((float)stackCount) / maxStack);
            }
        }

        public override void Init()
        {
            base.Init();
            UpdateData();
        }

        public override string DescExtended(string desc)
        {
            UpdateData();
            return base.DescExtended(desc).Replace("&a", this.PlusPerStat.Damage.ToString() + "%");
        }

        public override string DescInit()
        {
            UpdateData();
            return base.DescInit().Replace("&a", this.PlusPerStat.Damage.ToString() + "%");
        }

        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            UpdateData();
        }

        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            UpdateData();
        }

        //bug!

        /*
        public override void Special_SkillButtonPointerEnter()
        {
            base.Special_SkillButtonPointerEnter();
            GameObject toolTip = ToolTipWindow.ToolTip;
            if (toolTip != null)
            {
                ToolTipWindow toolTipWindow = toolTip.GetComponentInChildren<ToolTipWindow>();
                SkillToolTip skillToolTip = toolTipWindow.GetComponentInChildren<SkillToolTip>();
                if (skillToolTip != null)
                {
                    GameObject spreadEffect = ResUtils.LoadModPrefab(IDs.Res_GasSpreadUIEffect);
                    spreadEffect = GameObject.Instantiate(spreadEffect, skillToolTip.SkillImage.gameObject.transform);
                    UIGasAnimator uIGasAnimator = spreadEffect.GetComponent<UIGasAnimator>();
                    if (uIGasAnimator != null)
                    {
                        uIGasAnimator.SetFactorSmooth(B_Charge.GetChargePercent(this.BChar));
                    }
                }
            }
        }*/
    }
}