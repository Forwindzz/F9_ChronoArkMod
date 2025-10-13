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
    /// 根据蓄能效果层数，每层增加10%的伤害。
    /// </summary>
    public class SE_Charge :
        Skill_Extended,
        IP_BuffAddAfter,
        IP_BuffRemove
    {

        private SkillGasVisualEffectInfo gasEffectInfo = null;

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
            if (gasEffectInfo == null)
            {
                return;
            }
            if (stackCount == 0)
            {
                gasEffectInfo.SetFactor(0);
            }
            else
            {
                int maxStack = buff?.BuffData?.MaxStack ?? 0;
                if (maxStack == 0)
                {
                    gasEffectInfo.SetFactor(0);
                }
                else
                {
                    gasEffectInfo.SetFactor(((float)stackCount) / maxStack);
                }
            }
        }

        public override void Init()
        {
            base.Init();
            gasEffectInfo = SkillGasVisualEffectInfo.CloneSKillGasEffectObj(this.MySkill);
            if(gasEffectInfo!=null)
            {
                Transform gasTransform = gasEffectInfo.skillGasEffect.transform;

                // 这个是作为卡牌背景的效果
                {
                    gasTransform.localPosition = new Vector3(-134.3f, 0, 0);
                }
                
                /*
                // 这个是作为卡牌延伸的效果
                {
                    gasTransform.localPosition = new Vector3(260, 0, 0);
                }
                */
                
            }
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

        public override void SelfDestroy()
        {
            SkillGasVisualEffectInfo.Destroy(ref gasEffectInfo);
            base.SelfDestroy();
        }
    }
}