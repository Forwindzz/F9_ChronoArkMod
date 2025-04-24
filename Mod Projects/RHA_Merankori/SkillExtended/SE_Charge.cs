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
	/// 蓄能
	/// 根据蓄能效果层数，每层增加10%的伤害。
	/// </summary>
    public class SE_Charge:
		Skill_Extended,
		IP_BuffAddAfter,
        IP_BuffRemove
    {
		private void UpdateData()
		{
			if(this.MySkill==null)
			{
				return;
			}
			if(this.MySkill.Master==null)
			{
				return;
			}

			this.PlusPerStat.Damage = this.MySkill.Master.CountBuffStack(ModItemKeys.Buff_B_Charge) * 10;
        }

        public override void Init()
        {
            base.Init();
			UpdateData();
        }

        public override string DescExtended(string desc)
        {
            UpdateData();
            return base.DescExtended(desc).Replace("&a", this.PlusPerStat.Damage.ToString() + "%");
        }

        public override string DescInit()
        {
			UpdateData();
            return base.DescInit().Replace("&a", this.PlusPerStat.Damage.ToString()+"%");
        }

        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            UpdateData();
        }

        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            UpdateData();
        }
    }
}