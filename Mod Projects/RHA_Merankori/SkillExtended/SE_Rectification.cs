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
    /// 整流
    /// 除了已指向的队友，技能也会应用给其他全体队友
    /// </summary>
    public class SE_Rectification :
        Skill_Extended,
        IP_Targeted,
        IP_ParticleOut_After
    {
        public IEnumerator ParticleOut_After(Skill SkillD, List<BattleChar> Targets)
        {
            this.SelfDestroy();
            yield break;
        }

        public void Targeted(Skill SkillD, List<BattleChar> Targets)
        {
            if(BattleSystem.instance==null)
            {
                return;
            }
            if(BattleSystem.instance.AllyTeam==null)
            {
                return;
            }
            List<BattleChar> candidateChars = BattleSystem.instance.AllyTeam.AliveChars_Vanish;
            foreach (BattleChar candidateChar in candidateChars)
            {
                bool isAssigned = false;
                foreach(BattleChar currentTarget in Targets)
                {
                    if(currentTarget.Info.KeyData==candidateChar.Info.KeyData)
                    {
                        isAssigned = true;
                        break;
                    }
                }
                if(isAssigned)
                {
                    continue;
                }

                Targets.Add(candidateChar);
            }
        }


    }
}