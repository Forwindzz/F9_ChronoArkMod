using GameDataEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RHA_Merankori.BlowUpAttr;

namespace RHA_Merankori
{
    public static class Utils
    {
        public static void RemoveTarget(string keyID, List<BattleChar> Targets)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                var candidate = Targets[i];
                if (candidate.Info.KeyData == keyID)
                {
                    Targets.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void InvokeAllIP<T>(Action<T> action) where T : class
        {
            foreach (var ip in BattleSystem.instance.IReturn<T>())
            {
                if (ip != null)
                {
                    action(ip);
                }
            }
        }

        public static Buff GetBuffByID(this BattleChar bc, string keyID)
        {
            /*
            foreach(var buff in bc.Buffs)
            {
                if(buff.BuffData.Key == keyID)
                {
                    return buff;
                }
            }
            return null;*/
            return bc.BuffReturn(keyID);
        }

        public static void BuffAddWithStacks(this BattleChar target, string buffKey, BattleChar source, int stack)
        {
            for (int i = 0; i < stack; i++)
            {
                target.BuffAdd(buffKey, source);
            }

        }

        public static List<T> FilterOutExtendSkillsInHand<T>()
        {
            if (BattleSystem.instance == null)
            {
                return new List<T>();
            }
            return FilterOutExtendSkills<T>(BattleSystem.instance.AllyTeam.Skills);
        }

        public static List<T> FilterOutExtendSkills<T>(List<Skill> skills)
        {
            var list = new List<T>();
            foreach (Skill skill in skills)
            {
                foreach (var skill_extend in skill.AllExtendeds)
                {
                    if (skill_extend is T skill_extend_typed)
                    {
                        list.Add(skill_extend_typed);
                    }
                }
            }
            return list;
        }

        public static bool ContainExtendSkillsInHand<T>()
        {
            if (BattleSystem.instance == null)
            {
                return false;
            }
            return ContainExtendSkills<T>(BattleSystem.instance.AllyTeam.Skills);
        }

        public static bool ContainExtendSkills<T>(List<Skill> skills)
        {
            foreach (Skill skill in skills)
            {
                foreach (var skill_extend in skill.AllExtendeds)
                {
                    if (skill_extend is T)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int GetAliveAlliesTotalLoseHP()
        {
            if(BattleSystem.instance ==null)
            {
                return 0;
            }
            return GetTotalLoseHP(BattleSystem.instance.AllyTeam.AliveChars_Vanish);
        }

        public static int GetTotalLoseHP(List<BattleChar> battleChars)
        {
            if(BattleSystem.instance==null)
            {
                return 0;
            }
            int totalLoseHP = 0;
            foreach (var bc in battleChars)
            {
                int hp = bc.HP;
                int maxHP = bc.GetStat.maxhp;
                int loseHP = Mathf.Max(maxHP - hp, 0);
                totalLoseHP += loseHP;
            }
            return totalLoseHP;
        }

        public static IEnumerable<Skill> GetAllSkillsInBattle()
        {
            if (BattleSystem.instance != null)
            {
                BattleTeam allyTeam = BattleSystem.instance.AllyTeam;
                if (allyTeam != null)
                {
                    return 
                        allyTeam.Skills
                        .Concat(allyTeam.Skills_Deck)
                        .Concat(allyTeam.Skills)
                        .Concat(
                            allyTeam.Chars.Select(
                                (c)=>
                                {
                                    if(c is BattleAlly ally)
                                    {
                                        return ally.MyBasicSkill.buttonData;
                                    }
                                    return null;
                                }
                                )
                            )
                        .Concat(
                            allyTeam.Chars.Select(
                                (c) =>
                                {
                                    if (c is BattleAlly ally)
                                    {
                                        return ally.BattleBasicskillRefill;
                                    }
                                    return null;
                                }
                                )
                            )
                        .Where(x => x != null)
                        ;
                }
            }
            return new List<Skill>();
        }

        public static void EnsureExtendSkill<T>(this Skill skill) where T : Skill_Extended, new()
        {
            foreach (var skill_extend in skill.AllExtendeds)
            {
                if (skill_extend is T)
                {
                    return;
                }
            }
            Skill_Extended skill_Extended = skill.ExtendedAdd_Battle(new T());
            //Debug.Log($"Add extend {typeof(T).Name} to skill {skill.MySkill.KeyID}");
        }

        public static void EnsureExtendSkill(this Skill skill, string keyID)
        {
            foreach (var skill_extend in skill.AllExtendeds)
            {
                if (skill_extend.Data!=null && skill_extend.Data.Key == keyID) 
                {
                    return;
                }
            }
            Skill_Extended skill_Extended = skill.ExtendedAdd(keyID);
            //Debug.Log($"Add extend {keyID} to skill {skill.MySkill.KeyID}");
        }

        public static bool IsAnyAllyDying()
        {
            if (BattleSystem.instance != null && BattleSystem.instance.AllyTeam != null)
            {
                List<BattleChar> aliveChars = BattleSystem.instance.AllyTeam.AliveChars_Vanish;
                foreach(var c in aliveChars)
                {
                    if(c.BuffFind(GDEItemKeys.Buff_B_Neardeath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsNearDeath(this BattleChar bchar)
        {
            return bchar.BuffFind(GDEItemKeys.Buff_B_Neardeath);
        }

        public static int CountBuffStack(this BattleChar battleChar, string buffKey)
        {
            if (battleChar == null || buffKey == null)
            {
                return 0;
            }
            Buff buff = battleChar.GetBuffByID(buffKey);
            if (buff == null)
            {
                return 0;
            }
            return buff.StackNum;
        }

        public static int CountBuffStack(IEnumerable<BattleChar> chars,string buffKey)
        {
            if(chars==null || buffKey==null)
            {
                return 0;
            }
            int total = 0;
            foreach(var c in chars)
            {
                Buff buff = c.GetBuffByID(buffKey);
                if (buff == null)
                {
                    continue;
                }
                total+= buff.StackNum;
            }
            return total;
        }

        public static int CountAliveAllyBuffStack(string buffKey)
        {
            if(BattleSystem.instance==null || BattleSystem.instance.AllyTeam==null)
            {
                return 0;
            }
            return CountBuffStack(BattleSystem.instance.AllyTeam.AliveChars_Vanish, buffKey);
        }

        public static int CountAliveAllyBuffTypes()
        {
            if (BattleSystem.instance == null || BattleSystem.instance.AllyTeam == null)
            {
                return 0;
            }
            List<BattleChar> aliveChars = BattleSystem.instance.AllyTeam.AliveChars_Vanish;
            HashSet<string> buffTypes = new HashSet<string>();
            foreach (BattleChar c in aliveChars)
            {
                List<Buff> buffs = c.GetBuffs(BattleChar.GETBUFFTYPE.BUFF, false);
                foreach (Buff buff in buffs)
                {
                    buffTypes.Add(buff.BuffData.Key);
                }
            }
            return buffTypes.Count;
        }

        public static bool IsKeyID(this Buff buff, string keyID)
        {
            return buff.BuffData.Key == keyID;
        }

        public static bool IsCharacterKey(this BattleChar bChar, string key)
        {
            return bChar?.Info?.GetData?.Key == key;
        }

        public static bool IsCharacterKey(this Character ch, string key)
        {
            return ch?.GetData?.Key == key;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            if (list == null || list.Count <= 1)
            {
                return;
            }
            int n = list.Count;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0,n);
                n--;
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        public static void AllyTeamEquipRunIfType<T>(Action<T> callback)
        {
            foreach (var info in PlayData.TSavedata.Party)
            {
                if (info == null || info?.Equip == null)
                {
                    continue;
                }
                foreach (var equip in info.Equip)
                {
                    if (equip == null)
                    {
                        continue;
                    }
                    Item_Equip equip_item = equip as Item_Equip;
                    if (equip_item == null)
                    {
                        continue;
                    }
                    EquipBase equipBase = equip_item.ItemScript;
                    if (equipBase == null)
                    {
                        equipBase = equip_item.ItemScript_Snapshot;
                        if (equipBase == null)
                        {
                            continue;
                        }
                    }
                    if (equipBase is T modifier)
                    {
                        if (modifier != null)
                        {
                            callback(modifier);
                        }
                    }
                }
            }
        }

        public static bool AllyTeamEquipFindType<T>()
        {
            foreach (var info in PlayData.TSavedata.Party)
            {
                if (info == null || info?.Equip == null)
                {
                    continue;
                }
                foreach (var equip in info.Equip)
                {
                    if (equip == null)
                    {
                        continue;
                    }
                    Item_Equip equip_item = equip as Item_Equip;
                    if (equip_item == null)
                    {
                        continue;
                    }
                    EquipBase equipBase = equip_item.ItemScript;
                    if (equipBase == null)
                    {
                        equipBase = equip_item.ItemScript_Snapshot;
                        if (equipBase == null)
                        {
                            continue;
                        }
                    }
                    if (equipBase is T modifier)
                    {
                        if (modifier != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void ProlongBuff(this BattleChar battleChar, string buffID, int turn=1)
        {
            Buff buff = battleChar.GetBuffByID(buffID);
            ProlongBuff(buff, turn);
        }

        public static void ProlongBuff(this Buff buff, int turn)
        {
            if (buff != null)
            {
                if (buff.StackInfo.Count > 0)
                {
                    buff.StackInfo[0].RemainTime += turn;
                }
            }
        }

    }
}
