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
            bool isCalm = IsCalm();
            if(!isCalm)
            {
                yield break;
            }
            // 冷静：“折射”效果延长1回合
            int turn = isCalm ? 1 : 0;
            foreach (BattleChar bc in Targets)
            {
                // 指向的队友获得2回合“折射”效果
                Buff buff = bc.GetBuffByID(ModItemKeys.Buff_B_Refraction);
                if (buff != null)
                {
                    if (buff.StackInfo.Count > 0)
                    {
                        buff.StackInfo[0].RemainTime += turn;
                        continue;
                    }
                }
                else
                {
                    Debug.Log("Cannot find refraction buff to be prolonged");
                }
                //bc.BuffAdd(IDs.ID_Buff_Refraction, this.BChar, false, 0, false, turn + 1);
                //Debug.Log("Add new buff IDs.ID_Buff_Refraction");
            }
            yield break;
        }

    }
}