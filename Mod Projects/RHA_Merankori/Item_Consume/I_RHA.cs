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
            if(StageSystem.instance==null)
            {
                return false;
            }
            if (FieldSystem.instance ==null || FieldSystem.instance.MiniMap == null || !FieldSystem.instance.MiniMap.isActiveAndEnabled)
            {
                return false;
            }
            int range = 1;
            if(MapChange.BlowUpTiles(StageSystem.instance.PlayerPos, range))
            {
                MapChange.BlowUpParticleEffect(FieldSystem.instance.Playercontrol.transform.position, range);
                return true;
            }
            return false;
        }

    }
}