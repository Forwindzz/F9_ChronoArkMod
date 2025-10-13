using UnityEditor;
using UnityEngine;

// 从 FBM 灰度图近似生成 RG FlowMap：flow = normalize(∇FBM) 取垂直方向（curl-like）
public class FlowMapFromFBM : EditorWindow
{
    Texture2D fbmTex;
    string outPath = "Assets/flow_from_fbm.png";
    float gradScale = 1.0f;   // 采样尺度（越大越平滑）
    float strength = 1.0f;   // 向量强度（会归一化）
    bool useCurl = true;   // true=用梯度的垂直方向（更像旋涡）
    bool invertY = false;  // UV坐标系差异可翻转Y

    [MenuItem("Tools/Noise/FlowMap From FBM")]
    static void Open() => GetWindow<FlowMapFromFBM>("FlowMap From FBM");

    void OnGUI()
    {
        fbmTex = (Texture2D)EditorGUILayout.ObjectField("FBM Texture", fbmTex, typeof(Texture2D), false);
        gradScale = EditorGUILayout.Slider("Gradient Scale", gradScale, 0.5f, 4f);
        strength = EditorGUILayout.Slider("Strength", strength, 0.1f, 2f);
        useCurl = EditorGUILayout.Toggle("Use Curl (perp grad)", useCurl);
        invertY = EditorGUILayout.Toggle("Invert Y", invertY);
        outPath = EditorGUILayout.TextField("Output Path", outPath);

        GUI.enabled = fbmTex != null;
        if (GUILayout.Button("Bake FlowMap PNG")) Bake();
        GUI.enabled = true;
    }

    void Bake()
    {
        int W = fbmTex.width, H = fbmTex.height;
        var src = fbmTex.GetPixels();
        float Sample(int x, int y)
        {
            x = (x % W + W) % W; y = (y % H + H) % H; // repeat
            Color c = src[y * W + x];
            return c.grayscale;
        }

        var dst = new Color32[W * H];
        int step = Mathf.Max(1, Mathf.RoundToInt(gradScale));

        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                float dx = Sample(x + step, y) - Sample(x - step, y);
                float dy = Sample(x, y + step) - Sample(x, y - step);

                // 梯度 → 方向
                Vector2 g = new Vector2(dx, dy);
                Vector2 v = useCurl ? new Vector2(+g.y, -g.x) : g; // 垂直于梯度，更有旋涡感
                if (invertY) v.y = -v.y;

                if (v.sqrMagnitude > 1e-8f) v.Normalize();
                v *= strength;

                // 映射到 [0,1]
                float r = Mathf.Clamp01(v.x * 0.5f + 0.5f);
                float gCh = Mathf.Clamp01(v.y * 0.5f + 0.5f);
                dst[y * W + x] = new Color(r, gCh, 0.5f, 1f);
            }

        var tex = new Texture2D(W, H, TextureFormat.RGBA32, false, true);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(dst); tex.Apply();

        System.IO.File.WriteAllBytes(outPath, tex.EncodeToPNG());
        AssetDatabase.Refresh();
        Debug.Log($"FlowMap saved: {outPath}");
    }
}
