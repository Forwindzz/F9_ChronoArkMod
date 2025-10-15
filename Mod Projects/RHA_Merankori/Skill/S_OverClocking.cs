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
using NLog.Targets;
namespace RHA_Merankori
{
    /// <summary>
    /// 越频晶体
    /// 下1次“湮裂的燐焰晶”会附带“震荡冲击”的效果。
    /// 指向的队友获得1回合“折射”效果
    /// 冷静：“折射”效果延长1回合
    /// </summary>
    public class S_OverClocking : 
        Merankori_BaseSkill
    {
        public override bool CanApplyCalm => true;
        public override bool CanApplyPanic => false;

        public override void Init()
        {
            base.Init();
            this.effectSetting = StateForVisualEffect.Calm;
        }

        public override void SkillUseSingleAfter(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingleAfter(SkillD, Targets);
            BattleSystem.DelayInputAfter(CO_ExtendBuffTime(Targets));
        }

        private IEnumerator CO_ExtendBuffTime(List<BattleChar> Targets)
        {
            if (Targets == null)
            {
                yield break;
            }
            bool isCalm = IsCalm();
            if(!isCalm)
            {
                yield break;
            }
            // 冷静：“折射”效果延长1回合
            foreach (BattleChar bc in Targets)
            {
                // 指向的队友获得2回合“折射”效果
                bc.ProlongBuff(ModItemKeys.Buff_B_Refraction);
                //Debug.Log("Add new buff IDs.ID_Buff_Refraction");
            }
            yield break;
        }

    }
}