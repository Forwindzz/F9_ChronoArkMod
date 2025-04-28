using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    public class AutoSelfDestoryComponent : MonoBehaviour
    {
        public float destoryTime = 10.0f;
        public GameObject targetGameObject = null;
        private Coroutine coroutine;
        
        public void BeginDestory()
        {
            if(targetGameObject==null)
            {
                targetGameObject = transform.gameObject;
            }

            coroutine = StartCoroutine(DestoryGameObject(destoryTime));
        }

        public void CancelDestory()
        {
            StopCoroutine(coroutine);
        }

        private IEnumerator DestoryGameObject(float destoryTime)
        {
            yield return new WaitForSeconds(destoryTime);
            GameObject.Destroy(targetGameObject);
            yield break;
        }

        public static void AutoDestoryGameObject(GameObject gameObject, float delayTime)
        {
            AutoSelfDestoryComponent autoSelfDestoryComponent = gameObject.AddComponent<AutoSelfDestoryComponent>();
            autoSelfDestoryComponent.destoryTime = delayTime;
            autoSelfDestoryComponent.targetGameObject = gameObject;
            autoSelfDestoryComponent.BeginDestory();
        }
    }
}
