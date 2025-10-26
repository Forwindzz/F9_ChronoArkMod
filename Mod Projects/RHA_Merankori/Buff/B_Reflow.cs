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
    /// 回合开始时，若燐色存护少于&a层，赋予1层燐色存护。
    /// <color=#5061A4>（每&b%无法战斗抵抗会增加1层的阈值）</color>
    /// 进入<color=#BAC8FF>冷静</color>状态时，延长此效果1回合。
    /// 溢出的燐色存护会转化为5%无法战斗抵抗增益。
    /// </summary>
    public class B_Reflow :
        Buff,
        IP_BeforeBuffAdd,
        IP_PlayerTurn,
        IMerankoriCanExtraStackBuff
    {
        public const int baseThreshold = 1;
        public const int perDeathResist = 20;

        public void BeforeBuffAdd(BattleChar target, ref string key, ref BattleChar UseState, ref bool hide, ref int PlusTagPer, ref bool debuffnonuser, ref int RemainTime, ref bool StringHide, ref bool cancelbuff)
        {
            //Debug.Log($"BeforeBuff: owner={this.BChar?.Info?.Name} | target={target?.Info?.Name} | buff_key={key} | own calm={target.BuffFind(ModItemKeys.Buff_B_Calm)}");
            if (target.IsCharacterKey(ModItemKeys.Character_C_Merankori) && key == ModItemKeys.Buff_B_Calm && !target.BuffFind(ModItemKeys.Buff_B_Calm))
            {
                //Debug.Log("Prolonged!");
                this.ProlongBuff(1);
            }
            if (target==this.BChar && key == ModItemKeys.Buff_B_Shield)
            {
                Buff buff = this.BChar.GetBuffByID(ModItemKeys.Buff_B_Shield);
                if(buff==null)
                {
                    return;
                }
                if(buff.StackNum>=buff.BuffData.MaxStack)
                {
                    cancelbuff = true;
                    this.BChar.BuffAdd(ModItemKeys.Buff_B_DeathResist, this.BChar);
                }
            }
        }

        public int GetThreshold()
        {
            return baseThreshold + Mathf.Max(0, this.BChar.GetStat.DeadImmune / perDeathResist);
        }

        public void Turn()
        {
            Buff buff = this.BChar.GetBuffByID(ModItemKeys.Buff_B_Shield);
            int threshold = GetThreshold();
            if (buff.StackNum< threshold)
            {
                this.BChar.BuffAddWithStacks(ModItemKeys.Buff_B_Shield, this.BChar, this.StackNum);
            }
        }

        private string ProcessDesc(string s)
        {
            return s.Replace("&a", GetThreshold().ToString())
                .Replace("&b", perDeathResist.ToString());
        }

        public override string DescInit()
        {
            return ProcessDesc(base.DescInit());
        }

        public override string DescExtended(string desc)
        {
            return ProcessDesc(base.DescExtended(desc));
        }
    }
}