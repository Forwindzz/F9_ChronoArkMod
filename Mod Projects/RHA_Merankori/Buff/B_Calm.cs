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
    /// 冷静
    /// 照料可能因自己受伤的队友...
    /// </summary>
    public class B_Calm : 
        BaseImpactCardBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Calm;

        protected override bool CanApplyToSkill(Skill skill)
        {
            return skill.AllExtendeds.Any(
                x => x is IMerankoriStateInfo info 
                && info.CanApplyCalm
                && x.BChar==this.BChar);
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            BattleChar finalCandidate = null;
            int minHP = int.MaxValue;
            foreach(var candidate in BChar.MyTeam.AliveChars_Vanish)
            {
                if(candidate.HP<minHP)
                {
                    minHP = candidate.HP;
                    finalCandidate = candidate;
                }
            }
            if(finalCandidate!=null)
            {
                finalCandidate.BuffAdd(ModItemKeys.Buff_B_Shield, this.BChar);
            }
        }
    }
}