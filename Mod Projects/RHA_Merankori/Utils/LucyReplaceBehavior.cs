using ChronoArkMod.ModData;
using ChronoArkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace RHA_Merankori
{
    public class LucyReplaceBehavior : MonoMapBefore
    {
        // the main object
        public GameObject merankoriCharGO = null;
        public JsonSpriteLoader jsonSpriteLoader;
        public SS6AnimControl ss6AnimControl;
        public MeshRenderer mkRender;
        // lucy render
        public MeshRenderer lucyMeshRender = null;

        private bool isInited = false;

        public bool IsDisplayingSprite { get => merankoriCharGO.activeSelf; set => SetDisplayCustomSprite(value); }
    
        public static bool CheckIfShowSprite()
        {
            if (PlayData.TSavedata != null)
            {
                System.Collections.Generic.List<Character> party = PlayData.TSavedata.Party;
                bool flag = false;
                foreach (var c in party)
                {
                    if (c.KeyData == ModItemKeys.Character_C_Merankori) //check if our mod character is here 
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
            return false;
        }

        public static void CreateMonoBehavior(MeshRenderer lucyMeshRender)
        {
            if(lucyMeshRender==null||
                FieldSystem.instance==null ||
                FieldSystem.instance.Playercontrol==null)
            {
                return;
            }
            if (lucyMeshRender.gameObject.GetComponent<LucyReplaceBehavior>() != null)
            {
                return;
            }
            LucyReplaceBehavior lucyReplaceBehavior = lucyMeshRender.gameObject.AddComponent<LucyReplaceBehavior>();
            lucyReplaceBehavior.enabled = true;
            lucyReplaceBehavior.InitMerankoriCharacter(lucyMeshRender);
        }

        private void OnDestory()
        {
            if(merankoriCharGO!=null)
            {
                Destroy(merankoriCharGO);
            }
        }

        public void Update()
        {
            UpdateAnimation();
        }

        public void SetDisplayCustomSprite(bool flag)
        {
            if(lucyMeshRender==null || merankoriCharGO==null)
            {
                Debug.LogWarning("Try to set custom lucy sprite without inited!");
                return;
            }
            if (flag == IsDisplayingSprite)
            {
                return;
            }
            if (flag)
            {
                if (lucyMeshRender.enabled)
                {
                    merankoriCharGO.SetActive(true);
                    lucyMeshRender.enabled = false;
                    Debug.Log($"Show MerankoriCharacter");
                }
            }
            else
            {
                if (!lucyMeshRender.enabled)
                {
                    merankoriCharGO.SetActive(false);
                    lucyMeshRender.enabled = true;
                    Debug.Log($"Hide MerankoriCharacter");
                }
            }
            UpdateAnimation();
        }

        public void UpdateAnimation()
        {
            if (merankoriCharGO.activeSelf && isInited)
            {
                PlayerController playerController = FieldSystem.instance.Playercontrol;
                mkRender.sortingOrder = lucyMeshRender.sortingOrder; //绘制顺序，保证遮挡关系正确 
                if (playerController.RunToggle || playerController.Movevec.sqrMagnitude > 0.02f)
                {
                    if (playerController.Jumpval.JumpSpeed != 0)
                    {
                        ss6AnimControl.SwitchToAnimation(2, 0, true, 1.18f); //>.< run
                    }
                    else
                    {
                        ss6AnimControl.SwitchToAnimation(3, 0, true, 1.25f); //run
                    }
                }
                else
                {
                    ss6AnimControl.SwitchToAnimation(4); // idle
                }
                // 0 for attack
                // 1 for damaged
            }
        }

        private void InitMerankoriCharacter(MeshRenderer currentLucyRender)
        {
            PlayerController playerControl = FieldSystem.instance.Playercontrol;
            Debug.Log("Try init merankori character sprite");
            lucyMeshRender = currentLucyRender;

            ModInfo info = ModManager.getModInfo(IDs.ID_Mod);
            string path = info.assetInfo.ObjectFromAsset<GameObject>("rha_merankoriunityassetbundle", "Assets/ModAssets/Content/Prefabs/Character_Merankori.prefab");
            Debug.Log($"Try to load asset from {path}");
            GameObject character = AddressableLoadManager.Instantiate(path, AddressableLoadManager.ManageType.None);
            if (character == null)
            {
                merankoriCharGO = null;
                isInited = false;
                Debug.LogWarning($"InitMerankoriCharacter failed, cannot load Character_Merankori.prefab");
                return;
            }
            merankoriCharGO = character;

            jsonSpriteLoader = merankoriCharGO.GetComponentInChildren<JsonSpriteLoader>(true);
            ss6AnimControl = merankoriCharGO.GetComponentInChildren<SS6AnimControl>(true);
            mkRender = character.GetComponentInChildren<MeshRenderer>(true);

            merankoriCharGO.SetActive(false);

            MeshRenderer lucyRender = lucyMeshRender.GetComponentInChildren<MeshRenderer>();
            if (mkRender != null && lucyRender != null)
            {
                Debug.Log($"Render Get: {mkRender.gameObject.name} {lucyRender.gameObject.name}");

                mkRender.sortingLayerID = lucyMeshRender.sortingLayerID;
                mkRender.sortingOrder = lucyMeshRender.sortingOrder;
            }

            character.transform.parent = lucyMeshRender.transform;
            character.transform.localPosition = new Vector3(0, 1.2f, 0);
            character.transform.rotation = Quaternion.identity;
            character.transform.localScale = Vector3.one * 0.007f;
            character.layer = lucyMeshRender.gameObject.layer;
            merankoriCharGO.tag = lucyMeshRender.tag;

            character.gameObject.AddComponent<ObjectAngle>();

            Debug.Log($"InitMerankoriCharacter done");
            isInited = true;

            SetDisplayCustomSprite(true);
        }

    }
}
