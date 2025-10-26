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
    /// 蓄能
    /// 释放“湮裂的燐焰晶”时，每层增加其10%的伤害。
    /// </summary>
    public class B_Charge : BaseMAtkAllCardBuff,
        IMerankoriCanExtraStackBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Charge;

        public override void BuffStat()
        {
            base.BuffStat();
            this.PlusStat.DeadImmune = 5 * actualStackNum;
        }

        protected override void OnTriggerMerankoriBuffAttackAll(S_Attack_All skill_Extended)
        {
            this.SelfDestroy();
        }

        public static float GetChargePercent(BattleChar battleChar)
        {
            Buff buff = battleChar.GetBuffByID(ModItemKeys.Buff_B_Charge);
            int stackCount = buff?.StackNum ?? 0;
            int maxStack = buff?.BuffData?.MaxStack ?? 1;
            return ((float)stackCount) / maxStack;
        }

        public const int maxChargeStack = 1000000;

        private int actualStackNum = 0;
        public int ActualStackNum => actualStackNum;

        
        public static void AddChargeStack(BattleChar battleChar, BattleChar source, int num)
        {
            Buff buff = battleChar.GetBuffByID(ModItemKeys.Buff_B_Charge);
            if(buff==null)
            {
                battleChar.BuffAdd(ModItemKeys.Buff_B_Charge, source);
            }
            BattleSystem.DelayInputAfter(Co_AddChargeStack(battleChar, source, num));
        }

        private static IEnumerator Co_AddChargeStack(BattleChar battleChar, BattleChar source, int num)
        {
            B_Charge buff = null;
            int count = 0;
            while (buff == null && count < 200)
            {
                yield return null;
                buff = battleChar.GetBuffByID(ModItemKeys.Buff_B_Charge) as B_Charge;
                count++;
            }
            if (buff != null)
            {
                buff.actualStackNum += num;
                buff.BuffStatUpdate();
                /*
                foreach (var skill in Utils.GetAllSkillsInBattle())
                {
                    SE_Charge se_charge = skill.ExtendedFind<SE_Charge>();
                    if(se_charge!=null)
                    {
                        se_charge.
                    }
                }*/
            }
            else
            {
                Debug.LogWarning("Cannot find B_Charge!");
            }
            yield break;
        }

        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc).Replace("&a", actualStackNum.ToString());
        }

        public override string DescInit()
        {
            return base.DescInit().Replace("&a", actualStackNum.ToString());
        }


    }
}