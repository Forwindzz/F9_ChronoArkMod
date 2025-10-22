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
    /// 频移微调-露西
    /// 露西会引导燐焰晶
    /// 仅用于实现，实际是隐藏的buff
    /// </summary>
    public class B_H_FreqShift_Lucy : Buff, IP_ChangeDamageState
    {
        public void ChangeDamageState(SkillParticle SP, BattleChar Target, int DMG, bool Cri, ref bool ToHeal, ref bool ToPain)
        {
            //Debug.Log($"[B_H_FreqShift_Lucy] @{this.BChar?.Info?.Name} | S={SP?.SkillKey} | T: {Target?.Info?.Name} | D={DMG} | C={Cri} | H={ToHeal} | P={ToPain}");
            Buff buff = Target.GetBuffByID(ModItemKeys.Buff_B_FreqShift);
            if (buff == null || ToHeal)
            {
                return;
            }
            if (SP.SkillKey == ModItemKeys.Skill_S_Attack_All || SP.SkillData.ExtendedFind<S_Attack_All>() != null)
            {
                //Debug.Log("[B_H_FreqShift_Lucy] Heal!");
                ToHeal = true;
                if (Target.HP != Target.GetStat.maxhp)
                {
                    buff.SelfStackDestroy();
                }
            }
        }
    }
}