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
    /// 解除无法战斗上限
    /// 不再有80%的上限限制。
    /// </summary>
    public class B_DeadImmuneNoLimit : Buff
    {
        public override string DescInit()
        {
            return base.DescInit().Replace("&a", this.BChar.GetStat.DeadImmune.ToString());
        }
    }
}