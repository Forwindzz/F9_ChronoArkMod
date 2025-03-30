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
    /// 释放“湮裂的燐焰晶”时，每层增加其25%的伤害。
    /// 释放后移除此效果。
    /// </summary>
    public class B_Charge : BaseImpactAllCardBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Charge;

        public override void BuffStat()
        {
            base.BuffStat();
            this.PlusStat.DeadImmune = 2 * StackNum;
            this.PlusStat.AggroPer = 300;
        }
    }
}