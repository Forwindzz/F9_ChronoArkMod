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
    /// 回合开始时，生成1张“湮裂的燐焰晶”并放入手牌中。
    /// </summary>
    public class B_Backup : Buff
    {
        public override void TurnUpdate()
        {
            base.TurnUpdate();
            S_Attack_All.GenCardToHand(this.BChar);
        }
    }
}