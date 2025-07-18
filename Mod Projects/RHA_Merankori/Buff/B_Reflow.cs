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
    /// 燐焰晶回流
    /// 回合开始时，赋予1层燐色存护。
    /// 溢出的燐色存护会转化为折射。
    /// </summary>
    public class B_Reflow :
        Buff,
        IP_BeforeBuffAdd,
        IP_PlayerTurn,
        IMerankoriCanExtraStackBuff//,
        //IP_BattleEnd
    {
        /*
        public void BattleEnd()
        {
            List<BattleChar> aliveChars_Vanish = BattleSystem.instance.AllyTeam.AliveChars_Vanish;
            foreach(var c in aliveChars_Vanish)
            {
                int maxhp = c.GetStat.maxhp;
                int recover = maxhp /5;
                if(c.Recovery<0)
                {
                    this.BChar.Recovery = 1;
                }
                c.Heal(this.BChar, recover, false, true);
            }
        }*/

        public void BeforeBuffAdd(BattleChar target, ref string key, ref BattleChar UseState, ref bool hide, ref int PlusTagPer, ref bool debuffnonuser, ref int RemainTime, ref bool StringHide, ref bool cancelbuff)
        {
            if(target==this.BChar && key == ModItemKeys.Buff_B_Shield)
            {
                Buff buff = this.BChar.GetBuffByID(ModItemKeys.Buff_B_Shield);
                if(buff==null)
                {
                    return;
                }
                if(buff.StackNum>=buff.BuffData.MaxStack)
                {
                    cancelbuff = true;
                    this.BChar.BuffAdd(ModItemKeys.Buff_B_Refraction, this.BChar);
                }
            }
        }

        public void Turn()
        {
            this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Shield, this.BChar, this.StackNum);
            //this.BChar.BuffAdd(ModItemKeys.Buff_B_Shield, this.BChar);
        }
    }
}