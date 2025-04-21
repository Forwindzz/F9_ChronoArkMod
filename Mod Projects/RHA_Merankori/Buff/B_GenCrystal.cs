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
	/// 每当有人进入濒死状态时，将“湮裂的燐焰晶”放入手中。
	/// </summary>
    public class B_GenCrystal: 
        Buff,
        IP_NearDeath//,
        //IP_BuffRemove
    {
        /*
        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            if (buffMaster.Info.Ally && buff.IsKeyID(GDEItemKeys.Buff_B_Neardeath))
            {
                GenCard();
            }
        }
        */

        public void NearDeath(BattleAlly Ally)
        {
            GenCard();
        }

        private void GenCard(int times = 1)
        {
            Skill skill = Skill.TempSkill(ModItemKeys.Skill_S_Attack_All, this.BChar, this.BChar.MyTeam);
            for (int i = 0; i < times; i++)
            {
                this.BChar.MyTeam.Add(skill.CloneSkill(), true);
            }
        }
    }
}