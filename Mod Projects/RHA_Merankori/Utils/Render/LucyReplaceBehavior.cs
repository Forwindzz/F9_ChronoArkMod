using ChronoArkMod.ModData;
using ChronoArkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using ChronoArkMod.ModData.Settings;

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



        public static MeshRenderer FindLucyMeshRender()
        {
            MeshRenderer lucyMeshRender = null;
            if (FieldSystem.instance == null ||
                FieldSystem.instance.Playercontrol == null)
            {
                return null;
            }
            PlayerController playerController = FieldSystem.instance.Playercontrol;
            MeshRenderer[] meshRenderers = playerController.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers == null)
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

        // check if we need to load 
        public static void CheckUpdate()
        {
            if (LucyReplaceBehavior.CheckIfShowSprite())
            {
                if(LucyReplaceBehavior.instance==null)
                {
                    MeshRenderer meshRenderer = FindLucyMeshRender();
                    if (meshRenderer != null)
                    {
                        LucyReplaceBehavior.CreateMonoBehavior(meshRenderer);
                    }
                }
                else
                {
                    LucyReplaceBehavior.instance.SetDisplayCustomSprite(true);
                }
                
            }
            else
            {
                if(LucyReplaceBehavior.instance!=null)
                {
                    LucyReplaceBehavior.instance.SetDisplayCustomSprite(false);
                }

            }
        }
    
        //条件是小队里有梅朗柯莉
        public static bool CheckIfShowSprite()
        {
            if(IsDisabledGlobal()) //或者全局禁用
            {
                return false;
            }
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
            if(LucyReplaceBehavior.instance!=null)
            {
                Destroy(LucyReplaceBehavior.instance);
            }
            LucyReplaceBehavior.instance = lucyReplaceBehavior;
        }

        private static LucyReplaceBehavior instance = null;
        public static LucyReplaceBehavior Instance => instance;

        // instance part


        private void OnDestory()
        {
            LucyReplaceBehavior.instance = null;
            if (merankoriCharGO!=null)
            {
                Destroy(merankoriCharGO);
            }
        }


        private float DegreeDeltaAbs(float a, float b)
        {
            return Mathf.Abs((b - a + 180) % 360 - 180);
        }

        private float DegreeMoveToTarget(float cur, float target, float delta)
        {
            float diff = (target - cur + 180) % 360 - 180;

            if (Mathf.Abs(diff) <= delta)
                return target;

            return (cur + Mathf.Sign(diff) * delta + 360f) % 360f;
        }

        // lucy Rotation和camera的rotation在相差多少度的时候，强制同步两者的rotation
        public static float ForceCalibrateDegreeThres = 7;
        public static float carlibrateRotSpeed = 0.1f;

        public void Update()
        {
            UpdateAnimation();
            if (ss6AnimControl!=null && lucyMeshRender!=null)
            {
                Vector3 lucyRot = lucyMeshRender.transform.rotation.eulerAngles;
                Vector3 camRot = Camera.main.transform.rotation.eulerAngles;
                float delta = DegreeDeltaAbs(lucyRot.x, camRot.x);
                if(delta>=ForceCalibrateDegreeThres)
                {
                    lucyRot.x = DegreeMoveToTarget(lucyRot.x, camRot.x, delta * Time.deltaTime * carlibrateRotSpeed);
                    lucyMeshRender.transform.rotation = Quaternion.Euler(lucyRot);
                    ss6AnimControl.SpriteRoot.transform.rotation = Camera.main.transform.rotation;

                    //Debug.Log($"Force Rot player: delta={delta} | Lucy={lucyRot}, Camera={camRot}");
                }
                //Debug.Log($"Delta Time: {Time.deltaTime} -> {1.0f / Time.deltaTime} Ticks/s");
            }
        }

        private static bool IsDisabledGlobal()
        {
            ModInfo modInfo = ModManager.getModInfo(IDs.ID_Mod);
            if(modInfo==null)
            {
                return false;
            }
            ToggleSetting toggleSetting = modInfo.GetSetting<ToggleSetting>(IDs.Setting_UseCustomSpine);
            if(toggleSetting==null)
            {
                return false;
            }
            return toggleSetting.Value;
        }

        public void SetDisplayCustomSprite(bool flag)
        {
            if(lucyMeshRender==null || merankoriCharGO==null)
            {
                Debug.LogWarning("Try to set custom lucy sprite without inited!");
                return;
            }
            if(IsDisabledGlobal())
            {
                flag = false;
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
                    FieldSystem.DelayInput(PlayOnceAnimationCo(LucyReplaceBehavior.ANIM_DAMAGE, false));
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

        public const int ANIM_ATTACK = 0;
        public const int ANIM_DAMAGE = 1;
        public const int ANIM_ESCAPE = 2;
        public const int ANIM_RUN = 3;
        public const int ANIM_SPARE = 4;
        public const int ANIM_FADE = 5;

        private bool isPlayingOnce = false;

        public IEnumerator PlayOnceAnimationCo(int animationIndex, bool enableTransition = true)
        {
            PlayOnceAnimationCo(animationIndex, enableTransition);
            yield break;
        }

        public bool IsShowingCustomSpine
        {
            get => merankoriCharGO.activeSelf && isInited;
        }

        public void PlayOnceAnimation(int animationIndex, bool enableTransition=true)
        {
            //Debug.Log($"Play once {animationIndex} {enableTransition}");
            if(!IsShowingCustomSpine)
            {
                return;
            }
            isPlayingOnce = true;
            ss6AnimControl.SwitchToAnimation(
                animationIndex,
                1,
                enableTransition,
                1,
                (a, b) => { isPlayingOnce = false; return true; }
                );
        }

        //如果跑步时间很短，那就继续播放跑步动画，避免因为在站立和停止之间动画导致反复抽搐 （鼠标靠近人物时会发生这种情况）
        private float TOLERATE_RUN_TIME = 0.25f;
        private float totalRunAnimTime = 0.0f;
        private bool isRunningLastTime = false;

        //如果比较复杂的话，考虑包装成状态机模型
        public void UpdateAnimation()
        {
            if (IsShowingCustomSpine)
            {
                PlayerController playerController = FieldSystem.instance.Playercontrol;
                mkRender.sortingOrder = lucyMeshRender.sortingOrder; //绘制顺序，保证遮挡关系正确
                if(isPlayingOnce)
                {
                    return;
                }
                if (playerController.RunToggle || playerController.Movevec.sqrMagnitude > 1e-8f)
                {
                    //速度通常是240或者320
                    float playSpeedExtra = playerController.Movevec.magnitude * 0.00075f;
                    if (!isRunningLastTime) //如果上一次是站立，那么重新计时
                    {
                         //重新计算速度，实际速度会比这低...
                        totalRunAnimTime = 0;
                    }
                    playSpeedExtra = Mathf.Min(0.6f, playSpeedExtra);
                    if (playerController.Jumpval.JumpSpeed != 0)
                    {
                        ss6AnimControl.SwitchToAnimation(ANIM_ESCAPE); //>.< run
                        ss6AnimControl.SetPlaySpeed(1.0f + playSpeedExtra * 0.75f);
                    }
                    else
                    {
                        ss6AnimControl.SwitchToAnimation(ANIM_RUN); //run
                        ss6AnimControl.SetPlaySpeed(1.0f + playSpeedExtra);
                    }
                    totalRunAnimTime += Time.deltaTime;
                    isRunningLastTime = true;
                }
                else
                {
                    if(totalRunAnimTime>=TOLERATE_RUN_TIME)
                    {
                        ss6AnimControl.SwitchToAnimation(ANIM_SPARE); // idle
                        ss6AnimControl.SetPlaySpeed(1.0f);
                        totalRunAnimTime = 0.0f;
                    }
                    else
                    {
                        totalRunAnimTime += Time.deltaTime;
                    }
                    isRunningLastTime = false;
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

            GameObject character = ResUtils.LoadModPrefab("Assets/ModAssets/Content/Prefabs/Character_Merankori.prefab");
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
                //Debug.Log($"Render Get: {mkRender.gameObject.name} {lucyRender.gameObject.name}");

                mkRender.sortingLayerID = lucyMeshRender.sortingLayerID;
                mkRender.sortingOrder = lucyMeshRender.sortingOrder;
            }

            character.transform.parent = lucyMeshRender.transform;
            character.transform.localPosition = new Vector3(0, 1.2f, 0); // 这个offset可能会导致camera旋转时人物被看扁，因此我们需要在update中同步渲染SpriteSutdio gameobject的rotation
            character.transform.rotation = Quaternion.identity;
            character.transform.localScale = Vector3.one * 0.00634f;
            character.layer = lucyMeshRender.gameObject.layer;
            merankoriCharGO.tag = lucyMeshRender.tag;

            //character.gameObject.AddComponent<ObjectAngle>();

            Debug.Log($"InitMerankoriCharacter done");
            isInited = true;

            SetDisplayCustomSprite(true);
        }

    }
}
