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
    /// 受到<color=#FF6767>湮裂的燐焰晶</color>攻击时，将受到的伤害转化为等量的治疗，并移除1层此效果。满体力值时触发则不会移除层数。
    /// </summary>
    public class B_FreqShift : Buff,
        IMerankoriCanExtraStackBuff,
        IP_BattleStart_Ones
    {
        // 实现由B_H_FreqShift_Lucy实现，因为IP_ChangeDamageState只会调用技能释放者的buff列表
        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            if (BattleSystem.instance == null)
            {
                return;
            }
            if (BattleSystem.instance.AllyTeam == null)
            {
                return;
            }
            if (BattleSystem.instance.AllyTeam.LucyChar == null)
            {
                return;
            }
            //Debug.Log($"[B_FreqShift] @{this.BChar?.Info?.Name} | {this.StackNum} |> BuffOneAwake");
            EnsureComputationLogic();
        }

        public override void Init()
        {
            base.Init();
            EnsureComputationLogic();
            //Debug.Log($"[B_FreqShift] @{this.BChar?.Info?.Name} | {this.StackNum} |> Init");
        }

        public void EnsureComputationLogic()
        {
            if (BattleSystem.instance == null || BattleSystem.instance.AllyTeam == null)
            {
                return;
            }
            List<BattleChar> chars = BattleSystem.instance.AllyTeam.AliveChars_Vanish;
            foreach (BattleChar ch in chars)
            {
                if (ch == null)
                {
                    continue;
                }
                if (ch.BuffFind(ModItemKeys.Buff_B_H_FreqShift_Lucy))
                {
                    continue;
                }
                //Debug.Log($"[B_FreqShift] @{this.BChar?.Info?.Name} | {this.StackNum} |> Add H_ to {ch?.Info?.Name}");
                ch.BuffAdd(ModItemKeys.Buff_B_H_FreqShift_Lucy, this.BChar, false); // TODO: make it as true
            }
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            EnsureComputationLogic();
        }

        public void BattleStart(BattleSystem Ins)
        {
            EnsureComputationLogic();
        }
    }
}