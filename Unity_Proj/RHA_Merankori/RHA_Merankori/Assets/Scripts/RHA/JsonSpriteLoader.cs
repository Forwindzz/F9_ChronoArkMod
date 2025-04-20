using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JsonSpriteLoader : MonoBehaviour
{
    public TextAsset dataCellMapJson;
    public TextAsset dataAnimationJson;
    public TextAsset ss6RootJson;
    public TextAsset ssBlendAnimJson;
    private bool init = false;
    public SS6AnimControl control;
    public BlendAnimatorSpriteStudio blend;
    public Script_SpriteStudio6_Root root;
    public Script_SpriteStudio6_DataCellMap dataCellMapAsset;
    public Script_SpriteStudio6_DataAnimation dataAnimationMapAsset;

    public bool alwaysLoad = false;

    private static readonly JsonSerializerSettings cacheSettings = new JsonSerializerSettings
    {
        Converters = new List<JsonConverter>
        {
                new RectConverter(),
                new Vector3Converter(),
                new Vector2Converter(),
                new UnityColorConverter(),

                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation>(),
                //new Library_SpriteStudio6_Data_Animation_Label_Converter(),

                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Parts>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Attribute.Cell>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Attribute.Effect>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Attribute.Instance>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Attribute.PartsColor>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Attribute.Status>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Attribute.UserData>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.Label>(),

                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.CapacityContainer>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerCell>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerEffect>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInstance>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt>(),

                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerStatus>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerUserData>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection>(),
                
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.CellMap>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.CellMap.Cell>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.CellMap.Cell.DataMesh>(),
                
                new RecursiveTypeConverter<Library_SpriteStudio6.Data.Parts.Animation>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Parts.Animation.BindMesh>(),

                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Parts.Animation.ColorLabel>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Data.Parts.Effect>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Control.Animation.Parts>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Control.Animation.Track>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Control.Effect>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Control.Effect.Emitter>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Control.Effect.Particle>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Control.Effect.Particle.Activity>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Draw.Cluster>(),
                //new RecursiveTypeConverter<Library_SpriteStudio6.Draw.Cluster.Chain>(),

                new ScriptSpriteStudio6RootConverter(),
                new UnityObjectIgnoreJsonConverter()
        },
        Formatting = Formatting.Indented
    };


    public static JsonSerializerSettings GetJsonSetting()
    {
        return cacheSettings;
    }
    public void Init()
    {
        if (init)
        {
            return;
        }
        Debug.Log("Init Json Sprite!");
        init = true;

        EnsureComponent(ref control);
        EnsureComponent(ref blend);
        EnsureComponent(ref root);

        Script_SpriteStudio6_DataCellMap dataCellMap = root.DataCellMap;
        if (dataCellMap == null)
        {
            Debug.Log("Detect null for DataCellMap");
            dataCellMap = dataCellMapAsset;
            if (dataCellMap == null)
            {
                Debug.Log("Detect null for DataCellMap Asset... this is impossible!");
                dataCellMap = ScriptableObject.CreateInstance<Script_SpriteStudio6_DataCellMap>();
                dataCellMap.Version = Script_SpriteStudio6_DataCellMap.KindVersion.CODE_010001;
            }
            root.DataCellMap = dataCellMap;
        }
        if (alwaysLoad || dataCellMap.TableCellMap == null)
        {
            // In the game, this will become null, the JsonUtility cannot load dataCellMap.TableCellMap (return null)
            // So we need a wrapper to process this data.
            
            Debug.Log("Detect null for DataCellMap.TableCellMap");
            CellMapListWrapper cellMapListWrapper = JsonConvert.DeserializeObject<CellMapListWrapper>(dataCellMapJson.text);

            dataCellMap.TableCellMap = cellMapListWrapper.TableCellMap;
            Check(dataCellMap.TableCellMap);
        }

        // Same as datacell map... but we have too much data, I use converter to resolve it
        Script_SpriteStudio6_DataAnimation dataAnimation = root.DataAnimation;
        if(dataAnimation==null)
        {
            Debug.Log("Detect null for DataAnimation");
            root.DataAnimation = dataAnimation = dataAnimationMapAsset;
        }
        if (alwaysLoad || dataAnimation.TableParts == null || dataAnimation.TableAnimation == null)
        {
            Debug.Log("Detect null for DataAnimation TableParts");
            Stopwatch sw = Stopwatch.StartNew();
            JsonConvert.PopulateObject(dataAnimationJson.text, dataAnimation, GetJsonSetting());
            sw.Stop();
            Debug.Log($"Json Load time {sw.ElapsedMilliseconds} ms | {sw.ElapsedTicks} ticks");
            Debug.Log("Check content:");
            Check(dataAnimation.TableAnimation);
            Check(dataAnimation.CatalogParts);
            Check(dataAnimation.TableParts);
            Check(dataAnimation.TableMaterial);
            Check(dataAnimation.TableAnimationPartsSetup);
            Check(dataAnimation.Dictionary_CPE_Flyweight.TableValueFloatVector);
        }
        if (alwaysLoad || 
            blend.depthInterpInfos == null || 
            blend.depthInterpInfos.interpList==null || 
            blend.depthInterpInfos.interpList.Length == 0)
        {
            Debug.Log("Detect null for blend.depthInterpInfo");
            JsonConvert.PopulateObject(ssBlendAnimJson.text, blend.depthInterpInfos, GetJsonSetting());
            Check(blend.depthInterpInfos.interpList);
        }
        if (alwaysLoad || root.TableInformationPlay == null)
        {
            Debug.Log("Detect null for root.TableInformationPlay");
            JsonConvert.PopulateObject(ss6RootJson.text, root, GetJsonSetting());
            Check(root.TableInformationPlay);
        }

        if (alwaysLoad || blend.root == null)
        {
            Debug.Log("blend.root is null");
            blend.root = root;
        }
        if (alwaysLoad || blend.bodyTransform == null)
        {
            Debug.Log("blend.bodyTransform is null");
            blend.bodyTransform = root.transform;
        }

        Debug.Log("Check control init");
        if (alwaysLoad || control.SpriteRoot == null || control.BlendHelper == null)
        {
            Debug.Log($"control has null {control.SpriteRoot} {control.BlendHelper}");
            control.Init(blend, root);
        }

        if(alwaysLoad || root.TableControlParts==null)
        {
            Debug.Log("Detect oot.TableControlParts null");
            InitTableControlParts();
        }

        if (!root.gameObject.activeSelf)
        {
            root.gameObject.SetActive(true);
        }
    }

    private void EnsureComponent<T>(ref T comp) where T : Component
    {
        if (comp == null)
        {
            comp = this.GetComponent<T>();
            if (comp == null)
            {
                comp = this.GetComponentInChildren<T>();
                if (comp == null)
                {
                    Debug.Log($"Cannot find {typeof(T).Name}, Create a new one");
                    comp = this.gameObject.AddComponent<T>();
                }
            }
        }
    }

    private void InitTableControlParts()
    {
        var scriptRoot = root;
        int countParts = dataAnimationMapAsset.TableParts.Length;
        //var informationSSAE = dataAnimationMapAsset;
        GameObject gameObjectRoot = root.gameObject;
        Library_SpriteStudio6.Control.Animation.Parts[] tableControlParts = new Library_SpriteStudio6.Control.Animation.Parts[countParts];
        tableControlParts[0].InstanceGameObject = gameObjectRoot;

        GameObject gameObjectParent = null;
        GameObject gameObjectParts = null;
        Script_SpriteStudio6_Collider scriptCollider = null;
        int indexPartsParent;
        bool flagAttachCollider;
        for (int i = 0; i < countParts; i++)
        {
            if (0 >= i)
            {   /* "Root" */
                indexPartsParent = -1;
                gameObjectParent = null;
                gameObjectParts = gameObjectRoot;
                //							gameObjectParts.name = 
                //							tableControlParts[0].InstanceGameObject = gameObjectParts;
            }
            else
            {   /* Not "Root" */
                //indexPartsParent = informationSSAE.TableParts[i].Data.IDParent;
                indexPartsParent = dataAnimationMapAsset.TableParts[i].IDParent;
                gameObjectParent = (0 <= indexPartsParent) ? tableControlParts[indexPartsParent].InstanceGameObject : null;
                //gameObjectParts = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(informationSSAE.TableParts[i].Data.Name, true, gameObjectParent);
                gameObjectParts = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(dataAnimationMapAsset.TableParts[i].Name, true, gameObjectParent);

                // 跳过名称设置：
                //gameObjectParts.name = informationSSAE.TableParts[i].Data.Name;
                //gameObjectParts.name = dataAnimationMapAsset.TableParts[i].Name;
                tableControlParts[i].InstanceGameObject = gameObjectParts;
            }

            scriptCollider = null;
            flagAttachCollider = false;
            if (null != gameObjectParts)
            {
                //switch (informationSSAE.TableParts[i].Data.ShapeCollision)
                switch (dataAnimationMapAsset.TableParts[i].ShapeCollision)
                {
                    case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.NON:
                        break;

                    case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.SQUARE:
                        /* Attach Script */
                        scriptCollider = gameObjectParts.GetComponent<Script_SpriteStudio6_Collider>();
                        //scriptCollider = gameObjectParts.AddComponent<Script_SpriteStudio6_Collider>();
                        tableControlParts[i].InstanceScriptCollider = scriptCollider;
                        if (null != scriptCollider)
                        {
                            scriptCollider.InstanceRoot = scriptRoot;
                            scriptCollider.IDParts = i;

                            BoxCollider collider = gameObjectParts.AddComponent<BoxCollider>();
                            if (null != collider)
                            {
                                collider.enabled = true;
                                //collider.size = new Vector3(1.0f, 1.0f, setting.Collider.SizeZ);
                                collider.size = new Vector3(1.0f, 1.0f, 1.0f);
                                collider.center = Vector2.zero;
                                collider.isTrigger = false;

                                scriptCollider.InstanceColliderBox = collider;

                                flagAttachCollider = true;
                            }
                        }
                        break;

                    case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.AABB:
                        /* MEMO: Not Supported */
                        break;

                    case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE:
                        //scriptCollider = gameObjectParts.AddComponent<Script_SpriteStudio6_Collider>();
                        scriptCollider = gameObjectParts.GetComponent<Script_SpriteStudio6_Collider>();
                        tableControlParts[i].InstanceScriptCollider = scriptCollider;
                        if (null != scriptCollider)
                        {
                            scriptCollider.InstanceRoot = scriptRoot;
                            scriptCollider.IDParts = i;

                            //CapsuleCollider collider = gameObjectParts.AddComponent<CapsuleCollider>();
                            CapsuleCollider collider = gameObjectParts.GetComponent<CapsuleCollider>();
                            if (null != collider)
                            {
                                collider.enabled = true;
                                collider.radius = 1.0f;
                                //collider.height = setting.Collider.SizeZ;
                                collider.height = 1.0f;
                                collider.direction = 2;
                                collider.isTrigger = false;

                                scriptCollider.InstanceColliderCapsule = collider;

                                flagAttachCollider = true;
                            }
                        }
                        break;

                    case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE_SCALEMINIMUM:
                        /* MEMO: Not Supported */
                        break;

                    case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE_SCALEMAXIMUM:
                        /* MEMO: Not Supported */
                        break;
                }
                if (true == flagAttachCollider)
                {
                    //if (true == setting.Collider.FlagAttachRigidBody)
                    if (true)
                    {
                        /* Attach Rigid-Body */
                        //Rigidbody rigidbody = gameObjectParts.AddComponent<Rigidbody>();
                        Rigidbody rigidbody = gameObjectParts.GetComponent<Rigidbody>();
                        rigidbody.isKinematic = false;
                        rigidbody.useGravity = false;
                    }
                }
                else
                {
                    if (null != scriptCollider)
                    {
                        /* Remove Script */
                        UnityEngine.Object.Destroy(scriptCollider);
                    }
                }
            }
        }

        /* Datas Set */
        //scriptRoot.DataCellMap = informationSSPJ.DataCellMapSS6PU.TableData[0];
        //scriptRoot.DataAnimation = informationSSAE.DataAnimationSS6PU.TableData[0];
        //scriptRoot.TableMaterial = informationSSPJ.TableMaterialAnimationSS6PU;
        //scriptRoot.LimitTrack = limitTrack;
        scriptRoot.TableControlParts = tableControlParts;
    }

    private void Check<T>(T v)
    {
        string len = "no_len";
        if (v is Array array)
        {
            len = array?.Length.ToString();
        }
        Debug.Log($"{typeof(T).Name} = {v} |> {len}");
    }
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        Init();
    }
}
