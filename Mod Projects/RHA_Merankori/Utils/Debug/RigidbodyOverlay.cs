using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{

    public class RigidbodyOverlay : MonoBehaviour
    {
        void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.yellow;
            style.fontSize = 14;

            foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(rb.position);
                if (screenPos.z > 0)
                {
                    GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 20, 20), rb.name, style);
                }
            }

            foreach (Rigidbody2D rb in FindObjectsOfType<Rigidbody2D>())
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(rb.position);
                if (screenPos.z > 0)
                {
                    GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 20, 20), rb.name, style);
                }
            }
        }
    }

}
