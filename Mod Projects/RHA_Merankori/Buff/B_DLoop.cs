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
    /// 丢弃“湮裂的燐焰晶”时，获得1点法力值和2层蓄力。
    /// </summary>
    public class B_DLoop : Buff, IP_Discard
    {
        public void Discard(bool Click, Skill skill, bool HandFullWaste)
        {
            if(skill.ExtendedFind<S_Attack_All>()!=null)
            {
                BattleSystem.instance.AllyTeam.AP += 1;
                this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, 2);
            }
        }
    }
}