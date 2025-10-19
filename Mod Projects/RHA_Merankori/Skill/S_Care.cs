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

    public interface IMerankoriCanExtraStackBuff
    {

    }

    /// <summary>
    /// 悉心照料
    /// 冷静：目标持有“燐焰晶”相关的正面效果，再额外增加1层
    /// </summary>
    public class S_Care : Merankori_BaseSkill
    {

        public override bool CanApplyCalm => true;
        public override bool CanApplyPanic => false;
        public override bool UseParticleEffect => true;

        public override void Init()
        {
            base.Init();
            this.effectSetting = StateForVisualEffect.Calm;
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            foreach (BattleChar BattleChar in Targets)
            {
                var buffs = BattleChar.GetBuffs(BattleChar.GETBUFFTYPE.BUFF,false)
                    .Where<Buff>(b=>b is IMerankoriCanExtraStackBuff);

                foreach(var buff in buffs)
                {
                    BattleChar.BuffAdd(buff.BuffData.Key, this.BChar);
                }
            }
        }
    }
}