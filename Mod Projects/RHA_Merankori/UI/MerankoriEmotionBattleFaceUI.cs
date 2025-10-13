using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    public class MerankoriEmotionBattleFaceUI : MonoBehaviour
    {
        public static readonly string ASSET_PATH_PANIC = "Assets/ModAssets/Content/Sprites/Emotions/battle_face_panic.png";
        public static readonly string ASSET_PATH_CALM = "Assets/ModAssets/Content/Sprites/Emotions/battle_face_calm.png";

        public static Sprite spritePanic;
        public static Sprite spriteCalm;

        public static void CheckInitRes()
        {
            if (spritePanic == null)
            {
                spritePanic = ResUtils.LoadModAsset<Sprite>(ASSET_PATH_PANIC);
            }
            if (spriteCalm == null)
            {
                spriteCalm = ResUtils.LoadModAsset<Sprite>(ASSET_PATH_CALM);
            }
        }

        public static bool TryToCreate(BattleChar battleChar)
        {
            if (!battleChar.IsCharacterKey(ModItemKeys.Character_C_Merankori))
            {
                // not merankori!
                return false;
            }

            CheckInitRes();

            if (spriteCalm == null || spritePanic == null)
            {
                Debug.LogWarning("[MerankoriEmotionVisual] create with null sprites!");
                return false;
            }

            UIComponent ui = battleChar.UI;
            if(ui==null)
            {
                Debug.LogWarning("[MerankoriEmotionVisual] create with null UI!");
                return false;
            }

            if(ui.gameObject.GetComponent<MerankoriEmotionBattleFaceUI>()!=null)
            {
                Debug.LogWarning($"[MerankoriEmotionVisual] duplicate UI! skip {battleChar?.Info?.KeyData}");
                return false;
            }

            MerankoriEmotionBattleFaceUI faceUI = ui.gameObject.AddComponent<MerankoriEmotionBattleFaceUI>();
            faceUI.battleChar = battleChar;
            faceUI.ui = ui;

            return true;
        }

        public UIComponent ui;
        public BattleChar battleChar;

        public void SetCalmFace()
        {
            if (ui.CharImage_Image.sprite != spriteCalm)
            {
                ui.CharImage_Image.sprite = spriteCalm;
            }

        }

        public void SetPanicFace()
        {
            if (ui.CharImage_Image.sprite != spritePanic)
            {
                ui.CharImage_Image.sprite = spritePanic;
            }
        }

        public void UpdateFace()
        {
            if (EmotionBuffSwitch.IsPanic(battleChar))
            {
                SetPanicFace();
            }
            else
            {
                SetCalmFace();
            }
        }

        public static void OnUpdateUI(BattleChar bChar)
        {
            MerankoriEmotionBattleFaceUI faceUI = bChar?.UI?.GetComponent<MerankoriEmotionBattleFaceUI>();
            if (faceUI != null)
            {
                faceUI.UpdateUI();
            }
        }

        public void UpdateUI()
        {
            UpdateFace();
        }
    }
}
