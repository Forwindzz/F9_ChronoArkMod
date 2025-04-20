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
    /// ÿ���ֿܵ�1�β���ս��Ч�����ض�����ս����Ч��Ҳ�ֿܵ���
    /// </summary>
    public class B_Shield :
        Buff,
        IP_DeadResist,
        IP_BattleEnd,
        IP_BattleEndField,
        IP_TurnEnd,
        IP_PlayerTurn,
        IP_BuffAddAfter,
        IP_BuffRemove,
        IMerankoriCanExtraStackBuff
    {
        public void BattleEnd()
        {
            Check();
        }

        public void BattleEndField()
        {
            Check();
        }

        public bool DeadResist()
        {
            if(this.BChar.BuffFind(ModItemKeys.Buff_B_NotDeadlyAtk))
            {
                return true;
            }
            this.SelfStackDestroy();
            return true;
        }

        public void Turn()
        {
            Check();
        }

        public void TurnEnd()
        {
            Check();
        }

        private void Check()
        {
            if (this.BChar.Recovery < 1)
            {
                this.BChar.Recovery = 1;
            }
        }

        public override void BuffStat()
        {
            base.BuffStat();
            if(this.BChar.BuffFind(GDEItemKeys.Buff_B_Neardeath))
            {
                this.PlusStat.HEALTaken = this.StackNum * 10;
            }
        }

        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            BuffStat();
        }

        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            BuffStat();
        }
    }
}