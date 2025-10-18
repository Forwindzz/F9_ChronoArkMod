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
    /// 越频晶石
    /// 恢复<color=#BAC8FF>冷静</color>时移除这个效果，每次从手中使用技能时获得1蓄能。
    /// </summary>
    public class B_PanicFire : Buff,
        IP_SkillUse_User_After,
        IP_BuffAddAfter
    {
        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            if(this.BChar == BuffTaker && addedbuff.IsKeyID(ModItemKeys.Buff_B_Calm))
            {
                this.SelfDestroy();
            }
        }

        public void SkillUseAfter(Skill SkillD)
        {
            this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Charge, this.BChar, 1);
        }
    }
}