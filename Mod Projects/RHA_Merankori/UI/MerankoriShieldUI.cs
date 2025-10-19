using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RHA_Merankori
{
    public class MerankoriShieldUI : MonoBehaviour
    {
        // 原游戏中的Ally 这个prefab里包含了UIComponent的详细信息，也就是人物的battle UI

        private static GameObject shieldUITemplate = null;

        private static void CheckInitRes()
        {
            if(shieldUITemplate==null)
            {
                shieldUITemplate = ResUtils.LoadModPrefab("Assets/ModAssets/Content/Prefabs/RHA_Shield_Bar_UI.prefab");
                if(shieldUITemplate==null)
                {
                    Debug.LogError("Failed to load shieldUITemplate!");
                }
            }
        }

        //这个会通过patch在ui初始化的时候自动调用。
        public static void CreateUIForAlly(BattleChar bChar)
        {
            if(bChar==null)
            {
                return;
            }
            UIComponent uiComp = bChar.UI;
            if (uiComp == null)
            {
                //Debug.LogWarning($"Try to use null ally UI!");
                return;
            }

            CheckInitRes();
            if (shieldUITemplate == null)
            {
                return;
            }

            MerankoriShieldUI shieldUI = uiComp.GetComponent<MerankoriShieldUI>();
            if(shieldUI==null)
            {
                shieldUI = uiComp.gameObject.AddComponent<MerankoriShieldUI>();
                shieldUI.shieldUIObj = Instantiate(shieldUITemplate, uiComp.transform);
                shieldUI.shieldUI = shieldUI.shieldUIObj.GetComponentInChildren<ShieldBarControl>();
                if (shieldUI.shieldUI==null)
                {
                    Debug.LogError("Failed to get shield UI component!");
                }
            }
            else
            {
                Debug.LogWarning($"Duplicate Shield UI @{bChar?.Info?.KeyData}");
                return;
            }
            if (shieldUI != null)
            {
                shieldUI.allyUI = uiComp;
                shieldUI.bchar = bChar;
                //Debug.Log("Succeed in creating shield UI");
                return;
            }

        }

        public static void OnUIUpdate(BattleChar bChar)
        {
            MerankoriShieldUI shieldUI = bChar?.UI?.GetComponent<MerankoriShieldUI>();
            if(shieldUI==null)
            {
                //Debug.LogError($"Cannot find shield ui: {bChar?.name}");
                return;
            }
            shieldUI.UpdateUI();
        }


        private BattleChar bchar = null;
        private UIComponent allyUI = null;
        private ShieldBarControl shieldUI = null;
        private GameObject shieldUIObj = null;
        private bool firstInit = true;
        
        private bool IsValid()
        {
            return allyUI != null && shieldUI != null && bchar!=null;
        }

        private void UpdateUI()
        {
            if(!IsValid())
            {
                return;
            }
            if (allyUI.HP == null) 
            {
                return;
            }
            if(firstInit)
            {
                shieldUIObj.transform.SetParent(allyUI.HP.transform);
                shieldUIObj.transform.localPosition = new Vector3(-165, 0.0f, 0.0f);
                shieldUIObj.transform.localScale = Vector3.one * 0.6f;
                firstInit = false;
                //Debug.Log("init shield UI > " + bchar.name);
            }
            Buff shieldBuff = bchar.GetBuffByID(ModItemKeys.Buff_B_Shield);
            if(shieldBuff==null)
            {
                shieldUI.SetCount(0);
                shieldUI.SetAlphaMult(0.25f);
            }
            else
            {
                int currentCount = shieldBuff.StackNum;
                int maxCount = shieldBuff.BuffData.MaxStack;
                shieldUI.SetCount(currentCount, $"{currentCount}/{maxCount}");
                if(maxCount<=currentCount)
                {
                    Color orgColor = shieldUI.textAnim.text.color;
                    shieldUI.textAnim.text.color = new Color(1.0f, 0.7461056f, 0.4470588f, orgColor.a);
                }
                else
                {
                    Color orgColor = shieldUI.textAnim.text.color;
                    shieldUI.textAnim.text.color = new Color(0.4481132f, 0.7127607f, 1.0f, orgColor.a);
                }
                if (bchar.IsNearDeath())
                {
                    shieldUI.SetAlphaMult(1.0f);
                }
                else
                {
                    shieldUI.SetAlphaMult(0.25f);
                }
            }
        }
    }
}
