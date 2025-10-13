using UnityEngine;
using UnityEditor;

public class NoiseTexGen
{
    [MenuItem("Tools/Generate Tileable Noise")]
    static void Generate()
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float nx = (float)x / size, ny = (float)y / size;
                float v = Mathf.PerlinNoise(nx * 8f, ny * 8f);
                tex.SetPixel(x, y, new Color(v, v, v, 1));
            }
        }
        tex.Apply();
        System.IO.File.WriteAllBytes("Assets/noise_tex.png", tex.EncodeToPNG());
        AssetDatabase.Refresh();
    }
}
