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
    /// 镇定
    /// 阻止进入慌张状态，每次阻止会减少1层。
    /// </summary>
    public class B_CalmDown :
        Buff,
        IP_BuffAddAfter
    //IP_BeforeEmotionSwitch
    {
        /*
        public void BeforeEmotionSwitch(BattleChar battleChar, string planToSwitchID, ref bool cancelSwitch)
        {
            if(planToSwitchID == IDs.ID_Buff_Panic)
            {
                cancelSwitch = true;
                battleChar.BuffAdd(IDs.ID_Buff_Calm, battleChar);
                this.StackDestroy();
                Debug.Log($"Destory one stack calm down! {this.StackNum}");
            }
        }*/




        public void BuffaddedAfter(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff, StackBuff stackBuff)
        {
            if(addedbuff.BuffData.Key == ModItemKeys.Buff_B_Panic)
            {
                if(this.BChar.Info.KeyData==BuffTaker.Info.KeyData)
                {
                    BattleSystem.DelayInputAfter(Co_RecoverCalm());
                }
            }
        }

        public override void Init()
        {
            base.Init();
            isStackDestroy = true;
        }

        private IEnumerator Co_RecoverCalm()
        {
            EmotionBuffSwitch.SwitchToCalm(this.BChar);
            this.StackDestroy();
            yield break;
        }
    }
}