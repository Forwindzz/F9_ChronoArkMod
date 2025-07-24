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
	/// 火之存护
	/// 若回合开始时，没有燐色存护，施加1层燐色存护。
	/// </summary>
    public class B_EnsureShield:Buff
    {
        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            EnsureBuff();
        }

        public void Turn()
        {
            EnsureBuff();
        }

        public void EnsureBuff()
        {
            if (!this.BChar.BuffFind(ModItemKeys.Buff_B_Shield))
            {
                this.BChar.BuffAdd(ModItemKeys.Buff_B_Shield, this.BChar);
            }
        }
    }
}