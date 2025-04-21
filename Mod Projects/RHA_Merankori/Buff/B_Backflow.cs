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
    /// “湮裂的燐焰晶”会根据队友已损失的体力值增加，每2点增加1%伤害。
    /// </summary>
    public class B_Backflow : BaseMAtkAllCardBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Backflow;

        public override string DescInit()
        {
            return base.DescInit()
                .Replace("&a", Utils.GetAliveAlliesTotalLoseHP().ToString() + "%");
        }

        public override void Init()
        {
            base.Init();
        }
        /*

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            if (this.BChar.Info.Ally)
            {
                // 回合结束时，若手中有“湮裂的燐焰晶”，移除这个效果。
                if(Utils.ContainExtendSkillsInHand<S_Attack_All>())
                {
                    this.SelfDestroy();
                }
            }
        }*/
    }
}