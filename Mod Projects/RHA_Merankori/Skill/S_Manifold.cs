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
    /// 晶体流形
    /// </summary>
    public class S_Manifold : 
        Merankori_BaseSkill
    {
        public override bool CanApplyCalm => false;
        public override bool CanApplyPanic => false;
    }
}