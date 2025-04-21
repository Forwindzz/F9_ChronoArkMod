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
    /// 燐色存护
    /// 获得1层燐色存护
    /// </summary>
    public class SE_U_Shield : Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            foreach (BattleChar b in Targets)
            {
                b.BuffAdd(ModItemKeys.Buff_B_Shield, this.BChar);
            }
        }

        public override bool CanSkillEnforce(Skill MainSkill)
        {
            return MainSkill.AP > 0;
        }
    }
}