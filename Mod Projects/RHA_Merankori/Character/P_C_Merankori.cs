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
    /// 梅朗柯莉
    /// Passive:
    /// 情绪高涨的梅朗柯莉会激化身边的红色结晶，会对周围不分敌我地产生毁灭性的伤害，甚至摧毁地形。
    /// 但冷静下来的梅朗柯莉又会悉心照料因自己而受伤的队友。
    /// 任何攻击都会让她变得情绪慌张，但在回合开始时，梅朗柯莉会冷静下来。
    /// 固定技能始终为“湮裂的燐焰晶”
    /// 获得物品“活化燐焰晶”
    /// </summary>
    public class P_C_Merankori : 
        Passive_Char,
        IP_ParticleOut_After_Global,
        IP_LevelUp,
        IP_CampFire
    {

        //TODO: 情绪高涨的梅朗柯莉会激化身边的红色结晶，会对周围不分敌我地产生毁灭性的伤害，甚至摧毁地形。

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            //在回合开始时，梅朗柯莉会冷静下来。
            EmotionBuffSwitch.SwitchToCalm(this.BChar);
        }

        public override void Init()
        {
            base.Init();
        }

        public IEnumerator ParticleOut_After_Global(Skill SkillD, List<BattleChar> Targets)
        {
            //任何攻击都会让她变得情绪慌张
            if (SkillD.IsDamage && Targets.Count>0)
            {
                EmotionBuffSwitch.SwitchToPanic(this.BChar);
                Debug.Log($"Add Panic! {Targets.Count}");
            }
            yield break;
        }

        public void LevelUp()
        {
            FieldSystem.DelayInput(this.Delay());
        }

        private IEnumerator Delay()
        {
            InventoryManager.Reward(ItemBase.GetItem(ModItemKeys.Item_Consume_I_RHA, 2));
            yield break;
        }

        public void Camp()
        {
            InventoryManager.Reward(ItemBase.GetItem(ModItemKeys.Item_Consume_I_RHA, 3));
        }
    }
}