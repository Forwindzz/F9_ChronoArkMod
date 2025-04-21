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
    /// 不致命
    /// 燐焰晶的伤害不会致命
    /// 
    /// 
    /// </summary>
    public class B_NotDeadlyAtk :
        Buff,
        IP_ParticleOut_After_Global,
        IP_DeadResist
    {
        //燐焰晶的伤害不会致命
        public bool DeadResist()
        {
            Check();
            return true;
        }

        public IEnumerator ParticleOut_After_Global(Skill SkillD, List<BattleChar> Targets)
        {
            //动作完成后立刻解除此效果
            Check();
            this.SetDestroyBuffTrue(true);
            yield break;
        }

        private void Check()
        {
            if (this.BChar.Recovery < 1)
            {
                this.BChar.Recovery = 1;
            }
        }
    }
}