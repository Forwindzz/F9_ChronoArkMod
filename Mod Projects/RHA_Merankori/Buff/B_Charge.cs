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
    /// 释放“湮裂的燐焰晶”时，每层增加其10%的伤害。
    /// 释放后移除一半层数
    /// </summary>
    public class B_Charge : BaseMAtkAllCardBuff,
        IMerankoriCanExtraStackBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Charge;

        public override void BuffStat()
        {
            base.BuffStat();
            this.PlusStat.DeadImmune = 2 * StackNum;
            this.PlusStat.AggroPer = 33 * StackNum;
        }

        protected override void OnTriggerMerankoriBuffAttackAll(S_Attack_All skill_Extended)
        {
            int removeCount = this.StackNum / 2 + 1;
            for(int i=0;i<removeCount;i++)
            {
                this.SelfStackDestroy();
            }
        }
    }
}