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
    /// 频移微调
    /// 受到“湮裂的燐焰晶”的攻击时，将伤害转化为等量的治疗，然后移除1层。
    /// </summary>
    public class B_FreqShift : Buff
    {
        // 实现由B_H_FreqShift_Lucy实现，因为IP_ChangeDamageState只会调用技能释放者的buff列表
        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            if (BattleSystem.instance==null)
            {
                return;
            }
            if(BattleSystem.instance.AllyTeam==null)
            {
                return;
            }
            if(BattleSystem.instance.AllyTeam.LucyChar==null)
            {
                return;
            }
            //BattleSystem.instance.AllyTeam.LucyChar.BuffAdd(ModItemKeys.Buff_B_H_FreqShift_Lucy, this.BChar);
            List<BattleChar> chars = BattleSystem.instance.AllyTeam.AliveChars_Vanish;
            foreach (BattleChar ch in chars)
            {
                ch.BuffAdd(ModItemKeys.Buff_B_H_FreqShift_Lucy, this.BChar, true);
            }
        }
    }
}