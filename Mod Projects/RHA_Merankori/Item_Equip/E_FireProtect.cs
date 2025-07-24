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
	/// 火之守护
	/// 回合开始时，若没有燐色存护增益，赋予1层燐色存护（抵抗无法战斗1次）。
	/// <color=#5061A4>——从可爱的草刺猬上掉落的，没有1只草刺猬牺牲，真的，刚刚那只是我不小心用点力....啊？...愈疗是什么，那不是圣术？</color>
	/// </summary>
    public class E_FireProtect:EquipBase, IP_BattleStart_Ones
    {
        public void BattleStart(BattleSystem Ins)
        {
            this.BChar.BuffAdd(ModItemKeys.Buff_B_EnsureShield, this.BChar);
        }

        public override void Init()
        {
            base.Init();
            this.PlusStat.def = 10;
            this.PlusStat.DeadImmune = 20;
        }
    }
}