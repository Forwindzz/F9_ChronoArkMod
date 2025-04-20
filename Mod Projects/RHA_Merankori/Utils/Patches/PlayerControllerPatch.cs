using ChronoArkMod;
using ChronoArkMod.ModData;
using GameDataEditor;
using HarmonyLib;
using Spine.Unity;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using static Library_SpriteStudio6.Data.Animation.Attribute;

namespace RHA_Merankori
{
    [HarmonyPatch]
    static class PlayerControllerPatch
    {

        [HarmonyPatch(
            typeof(FieldSystem), 
            nameof(FieldSystem.PartyAdd),
            new System.Type[] { 
                typeof(GDECharacterData),
                typeof(int)
            })]
        [HarmonyPostfix]
        static void FieldSystem_PartyAdd_Postfix(
            PlayerController __instance
            )
        {
            if(LucyReplaceBehavior.CheckIfShowSprite())
            {
                MeshRenderer meshRenderer = FindLucyMeshRender();
                if(meshRenderer!=null)
                {
                    LucyReplaceBehavior.CreateMonoBehavior(meshRenderer);
                }
            }
        }

        public static MeshRenderer FindLucyMeshRender()
        {
            MeshRenderer lucyMeshRender = null;
            if(FieldSystem.instance==null || 
                FieldSystem.instance.Playercontrol==null)
            {
                return null;
            }
            PlayerController playerController = FieldSystem.instance.Playercontrol;
            MeshRenderer[] meshRenderers = playerController.GetComponentsInChildren<MeshRenderer>();
            if(meshRenderers==null)
            {
                return null;
            }
            foreach (var meshRenderer in meshRenderers)
            {
                if (meshRenderer.gameObject.name == "Spine" && meshRenderer.gameObject.tag == "PlayerC")
                {
                    lucyMeshRender = meshRenderer;
                    Debug.Log($"Find Lucy Mesh Render: {meshRenderer.gameObject}");
                    break;
                }
            }
            return lucyMeshRender;
        }

    }
}
