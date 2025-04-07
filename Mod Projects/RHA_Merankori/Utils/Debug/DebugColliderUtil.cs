using RHA_Merankori;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    internal class DebugColliderUtil
    {
        public static void EnableDebugVisCollider()
        {
            Rigidbody[] rigidbodies = GameObject.FindObjectsOfType<Rigidbody>();

            Rigidbody2D[] rigidbody2Ds = GameObject.FindObjectsOfType<Rigidbody2D>();
            foreach (Rigidbody b in rigidbodies)
            {
                if (b.GetComponent<RigidbodyOverlay>() == null)
                {
                    b.gameObject.AddComponent<RigidbodyOverlay>();
                    Debug.Log($"RigidBody add for {b.name} @{b.position} S={Camera.main.WorldToScreenPoint(b.position)}");
                }
            }
            foreach (Rigidbody2D b in rigidbody2Ds)
            {
                if (b.GetComponent<RigidbodyOverlay>() == null)
                {
                    b.gameObject.AddComponent<RigidbodyOverlay>();
                    Debug.Log($"RigidBody2D add for {b.name} @{b.position} S={Camera.main.WorldToScreenPoint(b.position)}");
                }
            }
            foreach (var b in GameObject.FindObjectsOfType<Collider>())
            {
                if (b.GetComponent<ColliderVisualizerMono>() == null)
                {

                    b.gameObject.AddComponent<ColliderVisualizerMono>();
                    Debug.Log($"Collider add for {b.name} @{b.transform.position} S={Camera.main.WorldToScreenPoint(b.transform.position)}");
                }
            }

            foreach (var b in GameObject.FindObjectsOfType<Collider2D>())
            {
                if (b.GetComponent<ColliderVisualizerMono>() == null)
                {
                    b.gameObject.AddComponent<ColliderVisualizerMono>();
                    Debug.Log($"Collider2D add for {b.name} @{b.transform.position} S={Camera.main.WorldToScreenPoint(b.transform.position)}");
                }
            }
        }
    }
}
