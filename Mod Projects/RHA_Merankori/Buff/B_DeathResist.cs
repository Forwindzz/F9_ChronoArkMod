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
    /// 不能战斗抵抗
    /// 增加不能战斗抵抗
    /// </summary>
    public class B_DeathResist : Buff,
        IMerankoriCanExtraStackBuff
    {
        public override void BuffStat()
        {
            this.PlusStat.DeadImmune = 5 * base.StackNum;
        }
    }
}