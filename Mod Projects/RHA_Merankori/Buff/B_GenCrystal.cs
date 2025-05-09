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
	/// 焰晶生成
	/// 每当有人脱离濒死状态时，将“湮裂的燐焰晶”放入手中。
	/// </summary>
    public class B_GenCrystal: 
        Buff,
        //IP_NearDeath,
        IP_BuffRemove
    {
        
        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            if (buffMaster.Info.Ally && buff.IsKeyID(GDEItemKeys.Buff_B_Neardeath))
            {
                S_Attack_All.GenCardToHand(this.BChar);
            }
        }

        /*
        public void NearDeath(BattleAlly Ally)
        {
            S_Attack_All.GenCardToHand(this.BChar);
        }
        */

    }
}