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
    /// </summary>
    public class S_WarmFire : Merankori_BaseSkill
    {
        public override void Init()
        {
            base.Init();
            //this.effectSetting = StateForVisualEffect.Calm;
        }
        /*
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            float reg = this.BChar.GetStat.reg;
            if(IsCalm())
            {
                foreach(BattleChar BattleChar in Targets)
                {
                    int recoverHP = (int)(reg * GetHealPercent(BattleChar) * 0.01f);
                    this.BChar.Heal(BattleChar, recoverHP, false, false, null);
                }
            }
        }

        private int GetHealPercent(BattleChar b)
        {
            if(b==null)
            {
                return 0;
            }
            return b.GetStat.DeadImmune / 5 * 5;
        }

        public override string DescInit()
        {
            return ReplaceDesc(base.DescInit());
        }

        private string ReplaceDesc(string result)
        {
            if (BattleSystem.instance == null)
            {
                return result;
            }
            if (BattleSystem.instance.AllyTeam == null || this.BChar == null)
            {
                return result;
            }
            float reg = this.BChar.GetStat.reg;
            List<BattleChar> aliveChars = BattleSystem.instance.AllyTeam.AliveChars_Vanish;
            string s = "\n";
            foreach (BattleChar bc in aliveChars)
            {
                int recoverHP = (int)(reg * GetHealPercent(bc) * 0.01f);
                s += bc.Info.Name + ": " + recoverHP + "\n";
            }
            return result.Replace("&a", s);
        }*/
    }
}