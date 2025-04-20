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
    /// 抽1张牌，获得燐焰晶物品1个
    /// </summary>
    public class S_ItemRHA : Skill_Extended,
        ICanMerankoriRectification
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            this.BChar.MyTeam.Draw(1);
            InventoryManager.Reward(ItemBase.GetItem(ModItemKeys.Item_Consume_I_RHA));
        }
    }
}