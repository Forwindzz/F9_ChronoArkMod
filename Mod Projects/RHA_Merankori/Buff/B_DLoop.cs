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
    /// 导流回路
    /// 释放<color=#FF6767>湮裂的燐焰晶</color>时，获得1点法力值。
    /// </summary>
    public class B_DLoop : Buff, IP_AfterMerankoriAttackAll
    {
        public void AfterMerankoriAttackAll(S_Attack_All skill_Extended)
        {
            this.BChar.MyTeam.AP += 1;
        }
    }
}