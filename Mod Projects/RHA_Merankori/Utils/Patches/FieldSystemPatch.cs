using ChronoArkMod;
using ChronoArkMod.ModData;
using GameDataEditor;
using HarmonyLib;
using Spine.Unity;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Library_SpriteStudio6.Data.Animation.Attribute;

namespace RHA_Merankori
{
    [HarmonyPatch]
    static class FieldSystemPatch
    {

        //用于判定是否加载梅朗柯莉的sprite，并替换掉lucy
        [HarmonyPatch(
            typeof(FieldSystem), 
            nameof(FieldSystem.PartyAdd),
            new System.Type[] { 
                typeof(GDECharacterData),
                typeof(int)
            })]
        [HarmonyPostfix]
        static void FieldSystem_PartyAdd_Postfix(
            FieldSystem __instance
            )
        {
            LucyReplaceBehavior.CheckUpdate();
        }

        private static List<GameObject> holdGameObjects = new List<GameObject>();
        
        [HarmonyPatch(
            typeof(FieldSystem),
            nameof(FieldSystem.NextStage),
            new System.Type[] {
            })]
        [HarmonyPostfix]
        static void FieldSystem_NextStage_Postfix(
            FieldSystem __instance
            )
        {
            ClearFieldGameObjects();
        }

        [HarmonyPatch(
            typeof(FieldSystem),
            nameof(FieldSystem.StageStart),
            new System.Type[] {
                typeof(string)
            })]
        [HarmonyPostfix]
        static void FieldSystem_StageStart_Postfix(
            FieldSystem __instance
            )
        {
            ClearFieldGameObjects();
        }

        public static void ClearFieldGameObjects()
        {
            foreach (var obj in holdGameObjects)
            {
                if (obj != null)
                {
                    GameObject.Destroy(obj);
                }
            }
            holdGameObjects.Clear();
        }

        //挂载到Field场景中，确保切换关卡时能够及时销毁
        public static void AddFieldGameObject(GameObject gameObject)
        {
            if(holdGameObjects.Count>=100) //如果数量太多直接销毁
            {
                Debug.Log("Too many held objects for field, remove them!");
                ClearFieldGameObjects();
            }
            holdGameObjects.Add(gameObject);
        }

    }
}
