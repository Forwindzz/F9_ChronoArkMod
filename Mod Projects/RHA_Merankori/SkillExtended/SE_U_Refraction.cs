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
using Spine;
namespace RHA_Merankori
{
	/// <summary>
	/// 折射
	/// 获得1回合折射
	/// </summary>
    public class SE_U_Refraction:Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            foreach (BattleChar b in Targets)
            {
                b.BuffAdd(ModItemKeys.Buff_B_Refraction, this.BChar);
            }
        }

        public override bool CanSkillEnforce(Skill MainSkill)
        {
            return !MainSkill.NotCount &&
            (
                MainSkill.MySkill.Target.Key == GDEItemKeys.s_targettype_ally ||
                MainSkill.MySkill.Target.Key == GDEItemKeys.s_targettype_otherally
            );
        }
    }
}