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
    /// 无法使用任何攻击技能。
    /// 回合结束时，解除1层。换牌则会移除此效果。
    /// </summary>
    public class B_Shock : Buff, IP_Discard
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(this.BChar.Info.Ally)
            {
                // 无法使用任何攻击技能。
                foreach (Skill skill in BattleSystem.instance.AllyTeam.Skills)
                {
                    if (skill.IsDamage && skill.Master == this.BChar)
                    {
                        skill.NotAvailable = true;
                    }
                }
            }
            else
            {
                this.PlusStat.Stun = true;
            }
        }

        public override void SelfdestroyPlus()
        {
            base.SelfdestroyPlus();
            if (this.BChar.Info.Ally)
            {
                // 解除无法使用任何攻击技能。
                foreach (Skill skill in BattleSystem.instance.AllyTeam.Skills)
                {
                    if (skill.IsDamage)
                    {
                        skill.NotAvailable = false;
                    }
                }
            }
            else
            {
                this.PlusStat.Stun = false;
            }
               
        }

        // 回合结束时，解除1层
        public override void TurnUpdate()
        {
            this.SelfStackDestroy();
            if(this.StackNum<=0)
            {
                this.Destr();
            }
        }

        public override void Init()
        {
            base.Init();
        }

        public void Destr()
        {
            this.SelfDestroy();
            this.BChar.BuffAddWithStacks(GDEItemKeys.Buff_B_Common_CCRsis, this.BChar, 6);
        }

        // 换牌则会移除此效果。
        public void Discard(bool Click, Skill skill, bool HandFullWaste)
        {
            if (skill.Master == this.BChar)
            {
                this.Destr();
            }
        }
    }
}