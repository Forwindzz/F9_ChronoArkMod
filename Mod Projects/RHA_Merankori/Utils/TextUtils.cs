﻿using ChronoArkMod;
using ChronoArkMod.ModData;
using GameDataEditor;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Library_SpriteStudio6.Utility;
using Random = System.Random;

namespace RHA_Merankori
{
    public static class TextUtils
    {

        public const string SPLIT = "\n---\n";
        public static readonly string[] SPLIT_ARRAY = new string[1] { "\n---\n" };

        public static void ShowAllyBattleText(this BattleChar bChar, string text)
        {
            BattleSystem.DelayInput(BattleText.InstBattleTextAlly_Co(bChar, text, true));
        }

        private static Random random = new Random();

        /// <summary>
        /// 战斗时队友说话
        /// </summary>
        /// <param name="bChar">队友</param>
        /// <param name="text">文本，用\n---\n分割</param>
        /// <param name="chance">说话的概率</param>
        /// <param name="isEvent">为true时，会阻碍用户交互进行</param>
        public static void ShowAllyBattleTextRandom(this BattleChar bChar, string text, float chance=1.0f, bool isEvent = false)
        {
            if (text == null || random.NextDouble() > chance) 
            {
                return;
            }
            BattleSystem.DelayInput(BattleText.InstBattleTextAlly_Co(bChar, GetRandomText(text), isEvent));
        }

        private static string GetRandomText(string text)
        {
            string[] strings = text.Split(SPLIT_ARRAY, StringSplitOptions.RemoveEmptyEntries);
            return strings[random.Next(strings.Length)];
        }
        /// <summary>
        /// 地图界面时的简短说话
        /// </summary>
        /// <param name="character"></param>
        /// <param name="text"></param>
        /// <param name="chance"></param>
        public static void ShowAllyFieldTextRandom(this Character character, string text, float chance=1.0f)
        {
            if (text == null || random.NextDouble() > chance)
            {
                return;
            }
            BattleText.InstFieldText(character.GetAllyWindow, GetRandomText(text));
        }

        public static bool ShowTextChance(float chance)
        {
            return random.NextDouble() > chance;
        }

        /*
        public static void ShowAllyBattleTextKey(this BattleChar bChar, string key)
        {
            //Debug.Log($"Try to show {key} for {bChar?.Info?.Name}");
            string s = GetRandomTextByKey(key);
            if(s == null)
            {
                Debug.LogError($"String key [{key}] is null!");
                return;
            }
            BattleSystem.DelayInput(BattleText.InstBattleText_Co(bChar, GetRandomTextByKey(key), true));
        }*/

        public static void ShowEnemyBattleText(this BattleEnemy bChar, string text)
        {
            if(bChar?.MyUIObject?.custom?.Head == null)
            {
                return;
            }
            BattleSystem.instance.StartCoroutine(BattleText.InstBattleText_Co(bChar.MyUIObject.custom.Head.transform.position, text));
        }

        /*
        public static void ShowEnemyBattleTextKey(this BattleEnemy bChar, string key)
        {
            if (bChar?.MyUIObject?.custom?.Head == null)
            {
                return;
            }
            BattleSystem.instance.StartCoroutine(BattleText.InstBattleText_Co(bChar.MyUIObject.custom.Head.transform.position, GetRandomTextByKey(key)));
        }*/

        public static void ShowBattleText(this BattleChar bChar, string text)
        {
            if(bChar.Info.Ally)
            {
                bChar.ShowAllyBattleText(text);
                return;
            }
            else if (bChar is BattleEnemy battleEnemy)
            {
                battleEnemy.ShowEnemyBattleText(text);
                return;
            }
            Debug.LogWarning($"Unclear how to display text for {bChar.Info.Name} -> {text}");
        }

        /*
        public static void ShowBattleTextKey(this BattleChar bChar, string key)
        {
            if (bChar.Info.Ally)
            {
                bChar.ShowAllyBattleTextKey(key);
                return;
            }
            else if (bChar is BattleEnemy battleEnemy)
            {
                battleEnemy.ShowEnemyBattleTextKey(key);
                return;
            }
            Debug.LogWarning($"Unclear how to display text key for {bChar.Info.Name} -> {key}");
        }*/

        /*
        public static string GetTextByKey(string key)
        {
            return LocalizationManager.GetTranslation(key);
        }
        public static string[] GetTextsByKey(string key)
        {
            return GetTextByKey(key)?.Split(SPLIT_ARRAY, StringSplitOptions.RemoveEmptyEntries);
        }

        private static Random random = new Random();

        public static string GetRandomTextByKey(string key)
        {
            string[] strings = GetTextsByKey(key);
            if(strings==null || strings.Length==0)
            {
                Debug.Log($"key {key} is {null} or 0!");
                return null;
            }
            return strings[random.Next(strings.Length)];
        }*/


    }
}
