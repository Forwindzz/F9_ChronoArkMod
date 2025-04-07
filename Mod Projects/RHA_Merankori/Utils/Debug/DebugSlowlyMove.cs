using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    public class DebugSlowlyMove : MonoBehaviour
    {

        public Vector3 moveDir = new Vector3(0, 0, 1);

        public float moveSpeed = 1.0f;

        private float lastTime = -5.0f;
        private float curTime = 0.0f;
        public void Update()
        {
            curTime+= Time.deltaTime;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            if(lastTime+3.0f<curTime)
            {
                lastTime = curTime;
                Debug.Log($"{gameObject.name} move {moveDir * moveSpeed * curTime} nowPos {transform.position}");
            }
        }
        public void Reset()
        {
            lastTime = -5f;
            curTime = 0.0f;
        }

        public static DebugSlowlyMove AddDebugSlowlyMove(
            GameObject gameObject,
            Vector3 moveDir,
            float moveSpeed = 1.0f
            )
        {
            var comp = gameObject.GetComponent<DebugSlowlyMove>();
            if (comp!=null)
            {
                comp.moveDir = moveDir;
                comp.moveSpeed = moveSpeed;
                comp.Reset();
                return comp;
            }
            DebugSlowlyMove debugSlowlyMove = gameObject.AddComponent<DebugSlowlyMove>();
            debugSlowlyMove.moveDir = moveDir;
            debugSlowlyMove.moveSpeed = moveSpeed;
            return debugSlowlyMove;
        }
    }
}
