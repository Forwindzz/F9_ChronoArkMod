using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHA_Merankori
{
    using UnityEngine;

    public static class ParticleSystemUtility
    {
        /// <summary>
        /// 修改给定物体及其所有子物体上粒子系统的材质渲染队列。
        /// </summary>
        /// <param name="parentObject">父物体</param>
        /// <param name="renderQueue">目标渲染队列值</param>
        public static void SetParticleSystemRenderQueue(GameObject parentObject, int renderQueue)
        {
            ParticleSystem[] particleSystems = parentObject.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem ps in particleSystems)
            {
                ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();

                if (psRenderer != null)
                {
                    Material psMaterial = psRenderer.material;
                    if (psMaterial != null)
                    {
                        psMaterial.renderQueue = renderQueue; 
                        //Debug.Log($"Updated RenderQueue for {ps.gameObject.name} to {renderQueue}");
                    }
                }
            }
        }

        /// <summary>
        /// 修改给定物体及其所有子物体上粒子系统的材质渲染队列。
        /// </summary>
        /// <param name="parentObject">父物体</param>
        /// <param name="renderQueue">目标渲染队列值</param>
        /// <param name="sortingLayerName">目标Sorting Layer的名字</param>
        /// <param name="sortingOrder">目标Sorting Order</param>
        public static void SetParticleSystemSorting(GameObject parentObject, string sortingLayerName, int sortingOrder)
        {
            ParticleSystem[] particleSystems = parentObject.GetComponentsInChildren<ParticleSystem>();

            int sortingLayerID = SortingLayer.NameToID(sortingLayerName);

            foreach (ParticleSystem ps in particleSystems)
            {
                ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
                //Debug.Log($"Set up sorting for {psRenderer.gameObject.name}");
                if (psRenderer != null)
                {
                    Material psMaterial = psRenderer.material;
                    if (psMaterial != null)
                    {
                        psRenderer.sortingLayerID = sortingLayerID;
                        psRenderer.sortingOrder = sortingOrder; 
                        
                    }
                }
            }
        }

        public static void SetParticleSystemNeverCareDepth(GameObject parentObject)
        {
            ParticleSystem[] particleSystems = parentObject.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem ps in particleSystems)
            {
                ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
                //Debug.Log($"Set up sorting for {psRenderer.gameObject.name}");
                if (psRenderer != null)
                {
                    Material psMaterial = psRenderer.material;
                    if (psMaterial != null)
                    {
                        psMaterial.SetInt("_ZWrite", 0);
                        psMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always); // 始终通过深度测试

                    }
                }
            }
        }
    }

}
