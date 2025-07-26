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
    /// 精密调石台
    /// 解除队伍无法战斗抵抗的上限，
    /// 使用燐晶核时，摧毁地图的半径增加1格。
    /// </summary>
    public class E_AdjustCrystal : 
        EquipBase, 
        IP_BattleStart_Ones, 
        BlowUpAttr.IModifyBlowUpAttr,
        IRemoveDeadImmuneLimitForTeam
    {
        public void BattleStart(BattleSystem Ins)
        {
            EnsureAddBuff();
        }

        public override void EquipEffect()
        {
            base.EquipEffect();
        }

        public override void EquipPlusStat_Team(Character Char, ref Stat stat, ref PerStat perStat)
        {
            base.EquipPlusStat_Team(Char, ref stat, ref perStat);
        }

        public override void Init()
        {
            base.Init();
            this.PlusStat.DeadImmune = 50;
        }

        public void ModifyBlowUpAttr(ref BlowUpAttr.BlowUpAttributes attr)
        {
            attr.range += 1;
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            EnsureAddBuff();
        }

        private void EnsureAddBuff()
        {
            List<BattleChar> chars = this.BChar.MyTeam.Chars;
            foreach (BattleChar chr in chars)
            {
                if(!chr.BuffFind(ModItemKeys.Buff_B_DeadImmuneNoLimit))
                {
                    chr.BuffAdd(ModItemKeys.Buff_B_DeadImmuneNoLimit, this.BChar);
                }
            }
        }
    }
}