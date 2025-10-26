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
using TileTypes;
namespace RHA_Merankori
{
    /// <summary>
    /// 燐焰晶
    /// 右击可以让梅朗柯莉摧毁周边的墙体，可能发现小型物资，在篝火处会充能3次。
    /// 赤红的晶石上透射着淡淡的回路，似乎很危险的样子。
    /// </summary>
    public class I_RHA : UseitemBase
    {

        public override bool Use()
        {
            return BlowUpTilesWithEffectChat();
        }

        public static bool BlowUpTilesWithEffectChat()
        {
            if (StageSystem.instance == null)
            {
                return false;
            }
            if (FieldSystem.instance == null || FieldSystem.instance.MiniMap == null || !FieldSystem.instance.MiniMap.isActiveAndEnabled)
            {
                return false;
            }

            BlowUpAttr.BlowUpAttributes blowUpAttributes = BlowUpAttr.GetBlowUpAttr();
            int range = blowUpAttributes.range;

            MapChange.BlowUpResult blowUpResult = MapChange.BlowUpTiles(StageSystem.instance.PlayerPos, range);
            if (blowUpResult.CanBlowUP)
            {
                if (LucyReplaceBehavior.Instance != null)
                {
                    LucyReplaceBehavior.Instance.PlayOnceAnimation(LucyReplaceBehavior.ANIM_DAMAGE, false);
                }
                MapChange.BlowUpParticleEffect(FieldSystem.instance.Playercontrol.transform.position, range);
                BlowUpChat(blowUpResult);
                return true;
            }
            else
            {
                BlowUpChat(blowUpResult);
                return false;
            }
        }

        public static void BlowUpChat(MapChange.BlowUpResult result)
        {
            foreach(var ch in PlayData.TSavedata.Party)
            {
                if(result.CanBlowUP)
                {
                    if (ch.IsCharacterKey(GDEItemKeys.Character_Phoenix))
                    {
                        ch.ShowAllyFieldTextRandom(ModLocalization.TS_Phoenix_UseRHA, 0.5f);
                    }

                    if (ch.IsCharacterKey(ModItemKeys.Character_C_Merankori))
                    {
                        //如果有隐藏
                        if(result.hiddenTiles.Count>0)
                        {
                            ch.ShowAllyFieldTextRandom(ModLocalization.TS_Merankori_RHA_FindHidden);
                        }
                        else if(result.rewardTiles.Count>0) //如果有奖励
                        {
                            int rewardCount = result.rewardTiles.Count;
                            if(FieldSystem.instance!=null)
                            {
                                FieldScriptOut fieldScript = FieldSystem.instance.FieldScript;
                                if(fieldScript!=null && TextUtils.ShowTextChance(0.1f* rewardCount* rewardCount))
                                {
                                    fieldScript.Text_GetItem();
                                }
                            }
                        }
                    }
                }
                else//无法使用
                {
                    if(ch.IsCharacterKey(ModItemKeys.Character_C_Merankori))
                    {
                        if (result.hiddenTiles.Count > 0)
                        {
                            //如果有隐藏
                            ch.ShowAllyFieldTextRandom(ModLocalization.TS_Merankori_RHA_CannotUseHidden);
                        }
                        else
                        {
                            ch.ShowAllyFieldTextRandom(ModLocalization.TS_Merankori_RHA_CannotUse);
                        }
                        
                    }
                }
                
                
            }
        }
    }
}