using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIGasController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Graphic target;   // Image / RawImage / etc.

    [Header("Drive (0..1)")]
    [Range(0f, 1f)] public float factor = 0f; // 0=无效果, 1=完全成型
    public bool useCurve = true;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Shader Property Names")]
    [SerializeField] private string prop_LUTGamma = "_LUTGamma";
    [SerializeField] private string prop_Tint = "_Color";
    [SerializeField] private string prop_Contrast = "_Contrast";

    [Header("From → To")]
    [SerializeField] private float gammaFrom = 4.23f;
    [SerializeField] private float gammaTo = 1.53f;

    [SerializeField] private Color tintFrom = Color.white;                 // (1,1,1,1)
    [SerializeField] private Color tintTo = new Color(1f, 0.7f, 0.7f, 1);// (1,0.7,0.7,1)

    [SerializeField] private float contrastFrom = 1.35f;
    [SerializeField] private float contrastTo = 0.37f;

    private Material runtimeMat;

    void Awake()
    {
        if (Application.isPlaying)
        {
            if (!target) target = GetComponent<Graphic>();
            if (target && target.material)
            {
                runtimeMat = Instantiate(target.material);
                runtimeMat.name = target.material.name + " (Runtime)";
                target.material = runtimeMat;
            }
        }
    }

    void OnDestroy()
    {
        if (runtimeMat)
        {
            if (Application.isPlaying)
                Destroy(runtimeMat);
            else
                DestroyImmediate(runtimeMat);
        }
    }

    void Update()
    {
        if (!Application.isPlaying || runtimeMat == null) return;
        Apply();
    }

    public void SetFactor(float v)
    {
        factor = Mathf.Clamp01(v);
        Apply();
    }

    private void Apply()
    {
        float t = Mathf.Clamp01(factor);
        if (useCurve && curve != null)
            t = Mathf.Clamp01(curve.Evaluate(t));

        float lutGamma = Mathf.Lerp(gammaFrom, gammaTo, t);
        float contrast = Mathf.Lerp(contrastFrom, contrastTo, t);
        Color tint = Color.Lerp(tintFrom, tintTo, t);

        if (runtimeMat.HasProperty(prop_LUTGamma))
            runtimeMat.SetFloat(prop_LUTGamma, lutGamma);
        if (runtimeMat.HasProperty(prop_Contrast))
            runtimeMat.SetFloat(prop_Contrast, contrast);
        if (runtimeMat.HasProperty(prop_Tint))
            runtimeMat.SetColor(prop_Tint, tint);
    }
}
