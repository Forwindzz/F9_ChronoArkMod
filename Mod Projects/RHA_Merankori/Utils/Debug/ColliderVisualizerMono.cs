using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{

    using UnityEngine;

    public class ColliderVisualizerMono : MonoBehaviour
    {
        public Color boxColor = Color.green;
        private GUIStyle labelStyle;
        private Camera mainCam;

        private Collider col3D;  // 可能为 null
        private Collider2D col2D;      // 可能为 null
        private PolygonCollider2D polyCol2D; // 精确绘制用

        private int offsetY = Random.Range(-16, 16);
        void Start()
        {
            mainCam = Camera.main;

            labelStyle = new GUIStyle
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };

            col3D = GetComponent<Collider>();
            col2D = GetComponent<Collider2D>();
            polyCol2D = GetComponent<PolygonCollider2D>();

            if (col3D == null && col2D == null)
            {
                //Debug.LogWarning($"[ColliderVisualizerMono] 没有检测到 Collider：{gameObject.name}");
                enabled = false;
            }
        }

        void OnGUI()
        {
            if (mainCam == null) return;

            bool flag = true;
            if (polyCol2D != null && polyCol2D.enabled)
            {
                boxColor = polyCol2D.isTrigger ? Color.blue : Color.green;
                DrawPolygonCollider2D(polyCol2D);
            }
            else if (col3D != null && col3D.enabled)
            {
                boxColor = col3D.isTrigger ? Color.blue : Color.green;
                DrawBoundsBox(col3D.bounds);
            }
            else if (col2D != null && col2D.enabled)
            {
                boxColor = col2D.isTrigger ? Color.blue : Color.green;
                DrawBoundsBox(col2D.bounds);
            }
            else
            {
                flag = false;
            }

            // 显示名称
            if(flag)
            {
                Vector3 worldPos = transform.position;
                Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
                screenPos.y = Screen.height - screenPos.y;

                // add offset avoid stack together
                GUI.Label(new Rect(screenPos.x + 4, screenPos.y - 16+ offsetY, 200, 20), gameObject.name, labelStyle);
            }
            
        }

        void DrawBoundsBox(Bounds bounds)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3[] corners = new Vector3[8]
            {
            mainCam.WorldToScreenPoint(center + new Vector3(-extents.x, -extents.y, -extents.z)),
            mainCam.WorldToScreenPoint(center + new Vector3(-extents.x, -extents.y, extents.z)),
            mainCam.WorldToScreenPoint(center + new Vector3(-extents.x, extents.y, -extents.z)),
            mainCam.WorldToScreenPoint(center + new Vector3(-extents.x, extents.y, extents.z)),
            mainCam.WorldToScreenPoint(center + new Vector3(extents.x, -extents.y, -extents.z)),
            mainCam.WorldToScreenPoint(center + new Vector3(extents.x, -extents.y, extents.z)),
            mainCam.WorldToScreenPoint(center + new Vector3(extents.x, extents.y, -extents.z)),
            mainCam.WorldToScreenPoint(center + new Vector3(extents.x, extents.y, extents.z))
            };

            float minX = corners[0].x, maxX = corners[0].x;
            float minY = corners[0].y, maxY = corners[0].y;
            for (int i = 1; i < corners.Length; i++)
            {
                minX = Mathf.Min(minX, corners[i].x);
                maxX = Mathf.Max(maxX, corners[i].x);
                minY = Mathf.Min(minY, corners[i].y);
                maxY = Mathf.Max(maxY, corners[i].y);
            }

            Rect rect = new Rect(minX, Screen.height - maxY, maxX - minX, maxY - minY);
            DrawRect(rect, boxColor);
        }

        void DrawPolygonCollider2D(PolygonCollider2D poly)
        {
            for (int p = 0; p < poly.pathCount; p++)
            {
                Vector2[] points = poly.GetPath(p);
                Vector2 prevScreen = WorldToGUIScreen(poly.transform.TransformPoint(points[0]));

                for (int i = 1; i < points.Length; i++)
                {
                    Vector2 worldPos = poly.transform.TransformPoint(points[i]);
                    Vector2 screenPos = WorldToGUIScreen(worldPos);

                    DrawLine(prevScreen, screenPos, boxColor);
                    prevScreen = screenPos;
                }

                // 尾部连接回起点
                Vector2 firstScreen = WorldToGUIScreen(poly.transform.TransformPoint(points[0]));
                DrawLine(prevScreen, firstScreen, boxColor);
            }
        }

        Vector2 WorldToGUIScreen(Vector2 world)
        {
            Vector3 screen = mainCam.WorldToScreenPoint(world);
            return new Vector2(screen.x, Screen.height - screen.y);
        }

        void DrawRect(Rect rect, Color color)
        {
            Color oldColor = GUI.color;
            GUI.color = color;

            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1), Texture2D.whiteTexture); // Top
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height, rect.width, 1), Texture2D.whiteTexture); // Bottom
            GUI.DrawTexture(new Rect(rect.x, rect.y, 1, rect.height), Texture2D.whiteTexture); // Left
            GUI.DrawTexture(new Rect(rect.x + rect.width, rect.y, 1, rect.height), Texture2D.whiteTexture); // Right

            GUI.color = oldColor;
        }

        void DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            HandlesWrapper.DrawLine(p1, p2, color);
        }
    }
    public static class HandlesWrapper
    {
        private static Texture2D _lineTex;

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width = 1f)
        {
            if (_lineTex == null)
            {
                _lineTex = new Texture2D(1, 1);
                _lineTex.SetPixel(0, 0, Color.white);
                _lineTex.Apply();
            }

            Color oldColor = GUI.color;
            GUI.color = color;

            Vector2 delta = pointB - pointA;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            float length = delta.magnitude;

            Matrix4x4 matrixBackup = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, length, width), _lineTex);
            GUI.matrix = matrixBackup;

            GUI.color = oldColor;
        }
    }

}
