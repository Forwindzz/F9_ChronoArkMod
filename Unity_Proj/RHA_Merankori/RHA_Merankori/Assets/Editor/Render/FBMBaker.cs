using UnityEngine;
using UnityEditor;

// 生成 tileable FBM（Perlin 混合）灰度贴图：供 NoiseTex 使用
public class FbmBaker : EditorWindow
{
    int size = 256;
    int octaves = 4;
    float baseFreq = 2.0f;
    float lacunarity = 2.0f;
    float gain = 0.5f;
    bool ridged = false;
    bool equalize = true;
    string outPath = "Assets/fbm_noise_256.png";
    int seed = 1337;

    [MenuItem("Tools/Noise/FBM Baker")]
    static void Open() => GetWindow<FbmBaker>("FBM Baker");

    void OnGUI()
    {
        size = EditorGUILayout.IntPopup("Size", size, new[] { "128", "256", "512" }, new[] { 128, 256, 512 });
        octaves = EditorGUILayout.IntSlider("Octaves", octaves, 1, 8);
        baseFreq = EditorGUILayout.Slider("Base Freq", baseFreq, 0.5f, 8f);
        lacunarity = EditorGUILayout.Slider("Lacunarity", lacunarity, 1.5f, 3.5f);
        gain = EditorGUILayout.Slider("Gain", gain, 0.3f, 0.8f);
        ridged = EditorGUILayout.Toggle("Ridged (turbulence)", ridged);
        equalize = EditorGUILayout.Toggle("Histogram Equalize", equalize);
        seed = EditorGUILayout.IntField("Seed", seed);
        outPath = EditorGUILayout.TextField("Output Path", outPath);

        if (GUILayout.Button("Bake FBM PNG")) Bake();
    }

    void Bake()
    {
        Random.InitState(seed);
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Bilinear;

        // 无缝 Perlin：四角混合法
        float SeamPerlin(float x, float y, float f, float ox, float oy)
        {
            float W = size, H = size;
            float u = x / W, v = y / H;
            float n00 = Mathf.PerlinNoise((x + ox) * f, (y + oy) * f);
            float n10 = Mathf.PerlinNoise((x + ox + W) * f, (y + oy) * f);
            float n01 = Mathf.PerlinNoise((x + ox) * f, (y + oy + H) * f);
            float n11 = Mathf.PerlinNoise((x + ox + W) * f, (y + oy + H) * f);
            float nx0 = Mathf.Lerp(n00, n10, u);
            float nx1 = Mathf.Lerp(n01, n11, u);
            return Mathf.Lerp(nx0, nx1, v);
        }

        // 随机偏移，避免同图同频率的重复感
        float offx = Random.Range(0f, 9999f);
        float offy = Random.Range(0f, 9999f);

        float[,] acc = new float[size, size];
        float minv = 1e9f, maxv = -1e9f;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float amp = 0.5f, f = baseFreq;
                float s = 0f;
                for (int o = 0; o < octaves; o++)
                {
                    float n = SeamPerlin(x, y, f, offx, offy);
                    if (ridged) n = 1f - Mathf.Abs(2f * n - 1f); // ridged/turbulence
                    s += amp * n;
                    f *= lacunarity;
                    amp *= gain;
                }
                acc[x, y] = s;
                minv = Mathf.Min(minv, s);
                maxv = Mathf.Max(maxv, s);
            }

        // 归一化到 [0,1]
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                acc[x, y] = (acc[x, y] - minv) / Mathf.Max(1e-6f, maxv - minv);

        // 简易直方图均衡（可关），提升动态范围
        if (equalize)
        {
            int bins = 256, N = size * size;
            int[] hist = new int[bins], cdf = new int[bins];
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++) hist[(int)(acc[x, y] * 255f)]++;
            cdf[0] = hist[0]; for (int i = 1; i < bins; i++) cdf[i] = cdf[i - 1] + hist[i];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    acc[x, y] = Mathf.Clamp01((cdf[(int)(acc[x, y] * 255f)] - cdf[0]) / (float)(N - 1));
        }

        // 写图
        var colors = new Color32[size * size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                byte v = (byte)Mathf.RoundToInt(acc[x, y] * 255f);
                colors[y * size + x] = new Color32(v, v, v, 255);
            }
        tex.SetPixels32(colors); tex.Apply();
        System.IO.File.WriteAllBytes(outPath, tex.EncodeToPNG());
        AssetDatabase.Refresh();
        Debug.Log($"FBM saved: {outPath}");
    }
}
