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
    /// </summary>
    public class B_Charge : BaseMAtkAllCardBuff,
        IMerankoriCanExtraStackBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Charge;

        public override void BuffStat()
        {
            base.BuffStat();
            this.PlusStat.DeadImmune = 5 * StackNum;
            this.PlusStat.AggroPer = 33 * StackNum;
        }

        protected override void OnTriggerMerankoriBuffAttackAll(S_Attack_All skill_Extended)
        {
            this.SelfDestroy();
        }

        public static float GetChargePercent(BattleChar battleChar)
        {
            Buff buff = battleChar.GetBuffByID(ModItemKeys.Buff_B_Charge);
            int stackCount = buff?.StackNum ?? 0;
            int maxStack = buff?.BuffData?.MaxStack ?? 1;
            return ((float)stackCount) / maxStack;
        }
    }
}