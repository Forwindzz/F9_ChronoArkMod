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
    /// 导流回路
    /// 锁定当前的<color=#BAC8FF>冷静</color>或<color=#FFC5BA>慌张</color>状态，每次阻碍状态切换会获得1点法力值。
    /// 
    /// 有的代码逻辑在EmotionBuffSwitch，避免lock的时候移除原状态....
    /// </summary>
    public class B_DLoop : Buff,
        IP_BeforeBuffAdd,
        IP_ParticleOut_After_Global
    {
        private bool alreadySupplyThisTurn = false;
        public void BeforeBuffAdd(BattleChar target, ref string key, ref BattleChar UseState, ref bool hide, ref int PlusTagPer, ref bool debuffnonuser, ref int RemainTime, ref bool StringHide, ref bool cancelbuff)
        {
            if (target != this.BChar)
            {
                return;
            }
            bool willPanic = key == ModItemKeys.Buff_B_Panic;
            bool willCalm = key == ModItemKeys.Buff_B_Calm;
            if (willCalm || willPanic)
            {
                cancelbuff = true;
                if (alreadySupplyThisTurn)
                {
                    return;
                }
                if (
                    EmotionBuffSwitch.IsCalm(this.BChar) && willPanic ||
                    EmotionBuffSwitch.IsPanic(this.BChar) && willCalm
                    )
                {
                    IncreaseStack();
                    alreadySupplyThisTurn = true;

                }
            }
        }


        public override string DescInit()
        {
            return base.DescInit()
                .Replace("&a", this.BChar.GetStat.DeadImmune.ToString() + "%");

        }

        public void IncreaseStack()
        {
            int deadImmune = this.BChar.GetStat.DeadImmune;
            RandomClass randomClass = null;
            BattleRandom.CharRandom.TryGetValue(this.BChar, out BattleCharRandom randomChar);
            if(randomChar != null)
            {
                randomClass =
                    randomChar.Main ??
                    randomChar.Target ??
                    randomChar.BaseRandomClass ??
                    randomChar.SkillSelect;
            }

            if (randomClass == null) // this should not happen...
            {
                randomClass =
                    BattleRandom.AllyBase ??
                    BattleRandom.NullCharRandom?.Main ??
                    BattleRandom.Main;
            }

            if (randomClass == null)
            {
                // this should not happen!
                if (UnityEngine.Random.Range(0.0f, 1.0f) < deadImmune)
                {
                    ApplyReward();
                    return;
                }
            }

            if (RandomManager.RandomFloat(randomClass, 0.0f, 1.0f) < deadImmune)
            {
                ApplyReward();
                return;
            }


        }

        private void ApplyReward()
        {
            this.BChar.MyTeam.AP += 1;
        }

        public IEnumerator ParticleOut_After_Global(Skill SkillD, List<BattleChar> Targets)
        {
            alreadySupplyThisTurn = false;
            yield break;
        }
    }
}