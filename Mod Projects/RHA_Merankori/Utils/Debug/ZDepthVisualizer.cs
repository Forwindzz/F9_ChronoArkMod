using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    public class ZDepthVisualizer : MonoBehaviour
    {
        public GameObject target; 
        public float minZ = -10;
        public float maxZ = 10;
        public float cubeSize = 0.3f;
        public float step = 0.5f;

        private GameObject temp;

        public void StartInit()
        {
            if (target == null) return;

            Vector3 basePos = target.transform.position;

            int cubeCount = 0;

            for (float x = minZ; x <= maxZ; x += step)
                for (float z = minZ; z <= maxZ; z += step)
                {
                    float t2 = Mathf.InverseLerp(minZ, maxZ, x);
                    float t = Mathf.InverseLerp(minZ, maxZ, z);
                    Color color = Color.Lerp(Color.blue, Color.red, t * x);  // 渐变：蓝 → 红


                    Vector3 pos = new Vector3(basePos.x + x, basePos.y, basePos.z + z);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = target.transform;
                    cube.transform.localScale = Vector3.one * cubeSize;
                    cube.transform.position = pos;
                    Renderer renderer = cube.GetComponent<Renderer>();
                    renderer.material = new Material(Shader.Find("Standard"));
                    renderer.material.color = color;
                    temp = cube;
                    cubeCount++;
                }

            Debug.Log($"Draw cubes done! {cubeCount}");
        }

        float time = 0.0f;
        float lastHintTime = 0.0f;
        public void Update()
        {/*
            time+= Time.deltaTime;
            float scale = Mathf.Pow(1.02f, time);
            temp.transform.localScale = scale * Vector3.one;
            if(time > lastHintTime+10)
            {
                lastHintTime = time;
                Debug.Log($"Debug growing cube {time} >>> {scale}");
            }*/
        }

        public static ZDepthVisualizer AddDepthDebugVis(
            GameObject gameObject,
            float minZ=-1,
            float maxZ=1,
            float cubeSize=0.2f
            )
        {
            ZDepthVisualizer zDepthVisualizer = gameObject.AddComponent<ZDepthVisualizer>();
            zDepthVisualizer.target = gameObject;
            zDepthVisualizer.minZ = minZ;
            zDepthVisualizer.maxZ = maxZ;
            zDepthVisualizer.cubeSize = cubeSize;
            zDepthVisualizer.StartInit();
            return zDepthVisualizer;
        }
    }

}
