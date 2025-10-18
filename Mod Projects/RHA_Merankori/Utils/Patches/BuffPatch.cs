using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{


    public interface IP_BeforeBuffAdd
    {
        void BeforeBuffAdd(
            BattleChar target,
            ref string key,
            ref BattleChar UseState,
            ref bool hide,
            ref int PlusTagPer,
            ref bool debuffnonuser,
            ref int RemainTime,
            ref bool StringHide,
            ref bool cancelbuff);
    }

    public interface IP_Healed_Anyone
    {
        void AfterHealedAnyone(
            BattleChar __instance,
            int __result,
            BattleChar User, float Hel, bool Cri, bool Force, BattleChar.ChineHeal heal);
    }

    [HarmonyPatch]
    static class BuffPatch
    {

        [HarmonyPatch(typeof(BattleChar), nameof(BattleChar.BuffAdd))]
        [HarmonyPrefix]
        static bool BattleChar_BuffAdd_Prefix(
            BattleChar __instance,
            ref string key,
            ref BattleChar UseState,
            ref bool hide,
            ref int PlusTagPer,
            ref bool debuffnonuser,
            ref int RemainTime,
            ref bool StringHide)
        {
            if (BattleSystem.instance == null)
            {
                return true;
            }
            bool cancelBuff = false;
            foreach (var ip in BattleSystem.instance.IReturn<IP_BeforeBuffAdd>())
            {
                if (ip != null)
                {
                    //Debug.Log("Check buff cancel");
                    ip.BeforeBuffAdd(
                        __instance,
                        ref key,
                        ref UseState,
                        ref hide,
                        ref PlusTagPer,
                        ref debuffnonuser,
                        ref RemainTime,
                        ref StringHide,
                        ref cancelBuff);
                }
            }
            return !cancelBuff;
        }

        [HarmonyPatch(typeof(BattleChar), nameof(BattleChar.Dead))]
        [HarmonyPrefix]
        static bool BattleChar_Dead_Prefix(
            BattleChar __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"Judge BattleChar.Dead {__instance.Info.KeyData}");
            bool result = B_Shield.Judge(__instance);
            return result;
        }

        [HarmonyPatch(typeof(BattleAlly), nameof(BattleAlly.Dead))]
        [HarmonyPrefix]
        static bool BattleAlly_Dead_Prefix(
            BattleAlly __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"Judge BattleChar.Dead {__instance.Info.KeyData}");
            bool result = B_Shield.Judge(__instance);
            return B_Shield.Judge(__instance);
        }

        //-------------------

        [HarmonyPatch(typeof(BattleAlly), nameof(BattleAlly.Dead))]
        [HarmonyPostfix]
        static void BattleAlly_Dead_Postfix(
            BattleAlly __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"JudgeEnd BattleAlly.Dead {__instance.Info.KeyData}");
            B_Shield.JudgeEnd(__instance);
        }

        [HarmonyPatch(typeof(BattleChar), nameof(BattleAlly.Dead))]
        [HarmonyPostfix]
        static void BattleChar_Dead_Postfix(
            BattleAlly __instance,
            bool notdeadeffect,
            bool NoTimeSlow)
        {
            //Debug.Log($"JudgeEnd BattleAlly.Dead {__instance.Info.KeyData}");
            B_Shield.JudgeEnd(__instance);
        }

        [HarmonyPatch(
            typeof(BattleChar), 
            nameof(BattleAlly.Heal),
             new Type[] {
                typeof(BattleChar),
                typeof(float),
                typeof(bool),
                typeof(bool),
                typeof(BattleChar.ChineHeal)
                }
             )]
        [HarmonyPostfix]
        static void BattleChar_Heal_Postfix(
            BattleChar __instance,
            int __result,
            BattleChar User, float Hel, bool Cri, bool Force, BattleChar.ChineHeal heal)
        {
            foreach (var ip in IAllReturn<IP_Healed_Anyone>())
            {
                if (ip != null)
                {
                    ip.AfterHealedAnyone(__instance, __result, User, Hel, Cri, Force, heal);
                }
            }

        }

        public static List<T> IAllReturn<T>(Skill UseSkill = null) where T : class
        {
            List<T> list = new List<T>();

            foreach (Item_Passive item_Passive in PlayData.Passive)
            {
                if (!item_Passive.ItemScript.OnePassive)
                {
                    list.Add(item_Passive.ItemScript as T);
                }
            }

            var allyChars = BattleSystem.instance?.AllyTeam?.Chars;

            if (allyChars != null)
            {

                BattleChar bChar = allyChars.FirstOrDefault();
                if (bChar != null)
                {

                    if (bChar.MyTeam != null)
                    {
                        foreach (Buff buff in bChar.MyTeam.LucyChar.Buffs)
                        {
                            if (buff is LucyBuff && (buff as LucyBuff).AllBuff && !buff.DestroyBuff)
                            {
                                list.Add(buff as T);
                            }
                        }
                        foreach (Skill skill in bChar.MyTeam.Skills)
                        {
                            foreach (Skill_Extended skill_Extended in skill.AllExtendeds)
                            {
                                if (!skill_Extended.OnePassive)
                                {
                                    list.Add(skill_Extended as T);
                                }
                            }
                        }
                        foreach (CastingSkill castingSkill in bChar.BattleInfo.CastSkills)
                        {
                            foreach (Skill_Extended skill_Extended2 in castingSkill.skill.AllExtendeds)
                            {
                                if (skill_Extended2.CountingExtedned && !skill_Extended2.OnePassive)
                                {
                                    list.Add(skill_Extended2 as T);
                                }
                            }
                        }
                        foreach (CastingSkill castingSkill2 in bChar.BattleInfo.SaveSkill)
                        {
                            Skill skill2 = castingSkill2.skill;
                            bool? flag;
                            if (skill2 == null)
                            {
                                flag = null;
                            }
                            else
                            {
                                BattleChar master = skill2.Master;
                                if (master == null)
                                {
                                    flag = null;
                                }
                                else
                                {
                                    Character info = master.Info;
                                    flag = ((info != null) ? new bool?(info.Ally) : null);
                                }
                            }
                            if (flag ?? false)
                            {
                                foreach (Skill_Extended skill_Extended3 in castingSkill2.skill.AllExtendeds)
                                {
                                    if (skill_Extended3.CountingExtedned && !skill_Extended3.OnePassive)
                                    {
                                        list.Add(skill_Extended3 as T);
                                    }
                                }
                            }
                        }
                        foreach (BattleChar battleChar in bChar.MyTeam.AliveChars)
                        {
                            if ((battleChar as BattleAlly).MyBasicSkill != null && (battleChar as BattleAlly).MyBasicSkill.buttonData != null)
                            {
                                foreach (Skill_Extended skill_Extended4 in (battleChar as BattleAlly).MyBasicSkill.buttonData.AllExtendeds)
                                {
                                    if (!skill_Extended4.OnePassive)
                                    {
                                        list.Add(skill_Extended4 as T);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (allyChars != null)
            {
                foreach (var bChar in allyChars)
                {
                    if (bChar == null)
                    {
                        continue;
                    }
                    foreach (BattleAlly battleAlly in bChar.BattleInfo.AllyList)
                    {
                        if (battleAlly.Info.Passive != null)
                        {
                            if (battleAlly.Info.Passive.OnePassive && bChar == battleAlly)
                            {
                                list.Add(battleAlly.Info.Passive as T);
                            }
                            else if (!battleAlly.Info.Passive.OnePassive)
                            {
                                list.Add(battleAlly.Info.Passive as T);
                            }
                        }
                    }
                    foreach (ItemBase itemBase in bChar.Info.Equip)
                    {
                        if (itemBase != null)
                        {
                            Item_Equip item_Equip = itemBase as Item_Equip;
                            list.Add(item_Equip.ItemScript as T);
                            list.Add(item_Equip.Curse as T);
                        }
                    }

                    foreach (Buff buff2 in bChar.Buffs)
                    {
                        if (!buff2.DestroyBuff)
                        {
                            list.Add(buff2 as T);
                        }
                    }

                    foreach (object obj in bChar.ModIReturn_Type(typeof(T)))
                    {
                        list.Add(obj as T);
                    }
                }
            }
            var emenyChar = BattleSystem.instance?.EnemyTeam?.Chars;
            if (emenyChar != null)
            {

                foreach (BattleEnemy bChar in emenyChar)
                {
                    if (bChar == null)
                    {
                        continue;
                    }
                    list.Add(bChar.Info.Passive as T);

                    foreach (Buff buff2 in bChar.Buffs)
                    {
                        if (!buff2.DestroyBuff)
                        {
                            list.Add(buff2 as T);
                        }
                    }

                    foreach (object obj in bChar.ModIReturn_Type(typeof(T)))
                    {
                        list.Add(obj as T);
                    }
                }
                {
                    BattleEnemy bChar = emenyChar?.FirstOrDefault() as BattleEnemy;
                    if (bChar != null)
                    {
                        foreach (CastingSkill castingSkill3 in (bChar as BattleEnemy).SkillQueue)
                        {
                            foreach (Skill_Extended skill_Extended5 in castingSkill3.skill.AllExtendeds)
                            {
                                if (skill_Extended5.CountingExtedned)
                                {
                                    list.Add(skill_Extended5 as T);
                                }
                            }
                        }
                    }
                }
            }
            if (UseSkill != null)
            {
                foreach (Skill_Extended skill_Extended6 in UseSkill.AllExtendeds)
                {
                    list.Add(skill_Extended6 as T);
                }
            }
            return list.Distinct<T>().ToList<T>();
        }

    }
}
