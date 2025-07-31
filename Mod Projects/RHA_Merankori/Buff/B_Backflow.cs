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
using Spine;
namespace RHA_Merankori
{
    /// <summary>
    /// 逆流的燐焰晶
    /// “湮裂的燐焰晶”会根据队友已损失的体力值增加，每?点增加?%伤害。
    /// </summary>
    public class B_Backflow : BaseMAtkAllCardBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Backflow;

        public const int PER_LOSS_HP = 5;
        public const int PER_INCREASE_PERCENT = 3;

        public static int ComputeFinalIncreasePercent()
        {
            return Utils.GetAliveAlliesTotalLoseHP() / PER_LOSS_HP * PER_INCREASE_PERCENT;
        }

        public override string DescInit()
        {
            return base.DescInit()
                .Replace("&a", PER_LOSS_HP.ToString())
                .Replace("&b", PER_INCREASE_PERCENT.ToString())
                .Replace("&c", ComputeFinalIncreasePercent().ToString() + "%");
        }

        public override void Init()
        {
            base.Init();
        }
    }
}