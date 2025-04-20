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
        IP_BeforeBuffAdd, 
        IMerankoriCanExtraStackBuff
    {
        private bool hasCanceledForAction = false;

        public void BeforeBuffAdd(BattleChar battleChar, ref string key, ref BattleChar UseState, ref bool hide, ref int PlusTagPer, ref bool debuffnonuser, ref int RemainTime, ref bool StringHide, ref bool cancelbuff)
        {
            if (key == ModItemKeys.Buff_B_Panic)
            {
                cancelbuff = true;
                if(!hasCanceledForAction)
                {
                    hasCanceledForAction=true;
                    this.StackDestroy();
                    BattleSystem.DelayInput(Co_ResetFlag());
                    Debug.Log("Cancel stack!");
                }
                Debug.Log("Cancel panic!");
            }
        }

        private IEnumerator Co_ResetFlag()
        {
            hasCanceledForAction = false;
            Debug.Log("Cancel panic flag reset!");
            yield break;
        }

        public override void Init()
        {
            base.Init();
            isStackDestroy = true;
        }
    }
}