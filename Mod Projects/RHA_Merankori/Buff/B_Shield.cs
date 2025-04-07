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
        IP_DeadResist
    {
        public bool DeadResist()
        {
            if(this.BChar.BuffFind(ModItemKeys.Buff_B_NotDeadlyAtk))
            {
                return true;
            }
            this.SelfStackDestroy();
            if(this.BChar.Recovery<1)
            {
                this.BChar.Recovery = 1;
            }
            return true;
        }
    }
}