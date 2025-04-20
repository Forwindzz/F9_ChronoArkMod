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
	/// 慌张
	/// 燐焰晶逐渐炽烈...
	/// </summary>
    public class B_Panic:BaseImpactCardBuff
    {
        protected override string ApplySkillExKey => ModItemKeys.SkillExtended_SE_Panic;

        protected override bool CanApplyToSkill(Skill skill)
        {
            return skill.AllExtendeds.Any(
                x => x is IMerankoriStateInfo info 
                && info.CanApplyPanic
                && x.BChar == this.BChar);
        }
    }
}