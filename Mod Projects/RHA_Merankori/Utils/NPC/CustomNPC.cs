using Dialogical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine.Events;
using UnityEngine;

namespace RHA_Merankori
{


    public class CustomNPC: MonoBehaviour
    {
        // EventObject.TargetEvent，会在交互时，通过UnityEvent调用方法
        public EventObject eventObjectComp;
        // 然后Dialogue里有个LocalEvent，也是通过一个字符串（方法名，参数，用空格隔开）SendMessage来调用该对话节点触发的额外调用
        public Dialogue dialogue;
        public CircleCollider2D collider;

        public GameObject eventRootObjectGO;
        public GameObject eventObjectGO;

        // render relevant
        public ObjectAngle objectAngle;
        public GameObject renderObject;
        public ObjectSort objectSort;
        //public SpriteRenderer spriteRenderer;

        public Dialogue dialogueAgain;

        public static CustomNPC Create(Vector3 localPos)
        {
            if(FieldSystem.instance == null)
            {
                return null;
            }
            GameObject eventRootObjectGO = new GameObject("EventRootObject");
            eventRootObjectGO.transform.localPosition = localPos;
            GameObject eventObjectGO = new GameObject("EventObject");
            eventObjectGO.transform.SetParent(eventRootObjectGO.transform);

            CustomNPC customNPC = eventObjectGO.AddComponent<CustomNPC>();
            customNPC.eventRootObjectGO = eventObjectGO;
            customNPC.eventObjectGO = eventObjectGO;
            customNPC.eventObjectComp = eventObjectGO.AddComponent<EventObject>();
            customNPC.dialogue = eventObjectGO.AddComponent<Dialogue>();
            customNPC.collider = eventObjectGO.AddComponent<CircleCollider2D>();
            customNPC.collider.radius = 0.8f;
            customNPC.objectAngle = eventObjectGO.AddComponent<ObjectAngle>();
            //customNPC.eventObjectComp.TargetEvent;

            //TODO: render should load from pack.
            GameObject render = new GameObject("Render");
            render.transform.parent.SetParent(eventObjectGO.transform);
            //customNPC.spriteRenderer = render.AddComponent<SpriteRenderer>();
            //customNPC.spriteRenderer.sortingOrder = - 1000;
            //customNPC.spriteRenderer.sortingLayerID = SortingLayer.GetLayerValueFromName("Object");
            customNPC.renderObject = render;
            customNPC.objectSort = render.AddComponent<ObjectSort>();
            //customNPC.objectSort.Spine = true;
            //render.transform.localPosition = new Vector3(0, 0.75f, 0);
            //render.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            GameObject dialogueGO = new GameObject("Dialogue Again");
            dialogueGO.transform.parent.SetParent(eventObjectGO.transform);
            customNPC.dialogueAgain = dialogueGO.AddComponent<Dialogue>();

            return customNPC;
        }
    }

    /*
    //copy from basic methods
    public class CustomNPC : MonoBehaviour
    {
        // Token: 0x17000014 RID: 20
        // (get) Token: 0x06000034 RID: 52 RVA: 0x00002A28 File Offset: 0x00000C28
        [XmlIgnore]
        [SerializeField]
        public GameObject EventObject
        {
            get
            {
                return base.gameObject;
            }
        }

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000035 RID: 53 RVA: 0x00002A40 File Offset: 0x00000C40
        public int ChildCount
        {
            get
            {
                bool flag = this.MainObject == null;
                int num;
                if (flag)
                {
                    num = 0;
                }
                else
                {
                    num = this.MainObject.transform.childCount;
                }
                return num;
            }
        }

        public bool Active
        {
            get
            {
                bool flag = this.MainObject == null;
                return !flag && this.MainObject.activeSelf;
            }
            set
            {
                bool flag = this.MainObject != null;
                if (flag)
                {
                    this.MainObject.SetActive(value);
                }
            }
        }

        public static T Creat<T>(Vector3 position, bool localPosition = true) where T : CustomNPC
        {
            bool flag = FieldSystem.instance == null;
            T t;
            if (flag)
            {
                t = default(T);
            }
            else
            {
                GameObject gameObject = AddressableLoadManager.Instantiate(BasicMethodsPlugin.ModInfo.assetInfo.ObjectFromAsset<GameObject>("basicmethodsunityassetbundle", "Assets/ChronoArkModUnity/Effect/EventRootObject.prefab"), AddressableLoadManager.ManageType.Stage, FieldSystem.instance.NowMap.transform);
                Transform transform = gameObject.transform.Find("EventObject");
                T t2 = transform.gameObject.AddComponent(typeof(T)) as T;
                EventObject component = transform.GetComponent<EventObject>();
                if (localPosition)
                {
                    gameObject.transform.localPosition = position;
                }
                else
                {
                    gameObject.transform.position = position;
                }
                t2.MainObject = gameObject;
                t2.EventScript = component;
                t2.MainDialogue = transform.GetComponent<Dialogue>();
                t2.MainSprites = transform.Find("New SkeletonRenderer").GetComponent<SpriteRenderer>();
                t2.MainObject.SetActive(true);
                t = t2;
            }
            return t;
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00002BF8 File Offset: 0x00000DF8
        public virtual T AddComponent<T>(Vector3 localPosition) where T : MonoBehaviour
        {
            bool flag = this.MainObject != null;
            T t2;
            if (flag)
            {
                GameObject gameObject = new GameObject("TempObejct" + (this.ChildCount + 1).ToString());
                GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, this.MainObject.transform);
                T t = gameObject2.AddComponent<T>();
                gameObject2.transform.localPosition = localPosition;
                gameObject2.SetActive(true);
                t2 = t;
            }
            else
            {
                t2 = default(T);
            }
            return t2;
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00002C80 File Offset: 0x00000E80
        public virtual void AddGameObject(GameObject gameObject, Vector3 position, bool local = true)
        {
            bool flag = gameObject == null || this.MainObject == null;
            if (!flag)
            {
                gameObject.transform.parent = this.MainObject.transform;
                if (local)
                {
                    gameObject.transform.localPosition = position;
                }
                else
                {
                    gameObject.transform.position = position;
                }
            }
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00002CEC File Offset: 0x00000EEC
        public virtual void AddEventDialogue(DialogueTree dialogueTree)
        {
            bool flag = dialogueTree != null;
            if (flag)
            {
                this.MainDialogue.tree = dialogueTree;
                this.EventScript.TargetEvent.RemoveListener(new UnityAction(this.MainDialogue.Activate));
                this.EventScript.TargetEvent.AddListener(new UnityAction(this.MainDialogue.Activate));
            }
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00002D58 File Offset: 0x00000F58
        public virtual void AddTargetEvent(UnityAction call)
        {
            bool flag = this.EventScript == null;
            if (!flag)
            {
                this.EventScript.TargetEvent.AddListener(call);
            }
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00002D8C File Offset: 0x00000F8C
        public virtual void RemoveTargetEvent(UnityAction call)
        {
            bool flag = this.EventScript == null;
            if (!flag)
            {
                this.EventScript.TargetEvent.RemoveListener(call);
            }
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00002DC0 File Offset: 0x00000FC0
        public virtual void ClearEvents()
        {
            bool flag = this.EventScript == null;
            if (!flag)
            {
                this.EventScript.TargetEvent.RemoveAllListeners();
                this.EventScript.TargetEvent_FastMode.RemoveAllListeners();
            }
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00002E04 File Offset: 0x00001004
        public virtual void AddSprites(Sprite sprite)
        {
            bool flag = this.MainSprites == null;
            if (!flag)
            {
                this.MainSprites.sprite = sprite;
                this.MainSprites.size = sprite.rect.size;
                this.MainSprites.color = Color.white;
            }
        }

        // Token: 0x04000018 RID: 24
        [XmlIgnore]
        [SerializeField]
        public GameObject MainObject;

        // Token: 0x04000019 RID: 25
        [XmlIgnore]
        [SerializeField]
        public EventObject EventScript;

        // Token: 0x0400001A RID: 26
        [XmlIgnore]
        [SerializeField]
        public Dialogue MainDialogue;

        // Token: 0x0400001B RID: 27
        [XmlIgnore]
        [SerializeField]
        public SpriteRenderer MainSprites;
    }*/
}