using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIGasAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UIGasController controller;   // 自动获取
    [SerializeField] private RectTransform rect;           // 自动获取

    [Header("Factor Tween (manual only)")]
    [SerializeField] private float defaultFactorDuration = 0.6f;
    [SerializeField] private AnimationCurve defaultFactorCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool ignoreTimeScale = false;

    [Header("Size Axes")]
    [Tooltip("是否在入/出场动画中改变宽度")]
    [SerializeField] private bool animateWidth = true;
    [Tooltip("是否在入/出场动画中改变高度")]
    [SerializeField] private bool animateHeight = true;

    [Header("Size In (OnEnable)")]
    [SerializeField] private bool animateSizeOnEnable = true;
    [SerializeField] private float sizeInDuration = 0.45f;
    [SerializeField] private AnimationCurve sizeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Size Out (Destroy)")]
    [SerializeField] private float sizeOutFactorDuration = 0.3f;       // 先让 factor→0
    [SerializeField] private float sizeOutShrinkDuration = 0.35f;      // 再把宽/高→0
    [SerializeField] private AnimationCurve sizeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine factorCo;
    private Coroutine sizeCo;
    private bool isDestroying = false;
    public bool IsDestorying => isDestroying;
    private float currentFactorTarget = -1f; // 正在前往的目标 factor
    private bool CanRunCoroutines => isActiveAndEnabled && gameObject.activeInHierarchy;

    void Awake()
    {
        if (!controller) controller = GetComponent<UIGasController>();
        if (!rect) rect = transform as RectTransform;
    }

    void OnEnable()
    {
        if (animateSizeOnEnable && rect)
            StartSizeIn();
    }

    void OnDestroy()
    {
        StopAllCoroutines(); // 兜底
    }

    // =================== 外部接口（factor） ===================

    public void SetFactorSmooth(float to, float duration = -1f, AnimationCurve curveOverride = null)
    {
        if (!controller) return;

        to = Mathf.Clamp01(to);
        float compareBase = (factorCo != null && currentFactorTarget >= 0f) ? currentFactorTarget : controller.factor;
        if (Mathf.Approximately(compareBase, to)) return;

        float d = duration > 0f ? duration : defaultFactorDuration;
        var curve = curveOverride ?? defaultFactorCurve;

        if (!CanRunCoroutines || d <= 0f)
        {
            SetFactorImmediate(to);
            return;
        }

        StartFactorTween(controller.factor, to, d, curve);
    }

    public void SetFactorImmediate(float value)
    {
        if (!controller) return;
        float v = Mathf.Clamp01(value);
        if (factorCo != null) { StopCoroutine(factorCo); factorCo = null; }
        currentFactorTarget = v;
        controller.SetFactor(v);
    }

    public void StopFactorTween()
    {
        if (factorCo != null) { StopCoroutine(factorCo); factorCo = null; }
    }

    public void ReplaySizeIn()
    {
        if (!rect) return;
        if (sizeCo != null) StopCoroutine(sizeCo);

        if (!CanRunCoroutines)
        {
            Vector2 target = GetCurrentSize(rect);
            SetSize(rect, target.x, target.y);
            return;
        }
        sizeCo = StartCoroutine(Co_SizeIn());
    }

    public float GetCurrentFactor() => controller ? controller.factor : 0f;

    // =================== 外部接口（销毁/尺寸） ===================

    /// <summary>
    /// 顺序出场：先 factor→0（sizeOutFactorDuration），完成后再把参与轴尺寸→0（sizeOutShrinkDuration），最后销毁。
    /// </summary>
    public void DestroySmooth()
    {
        if (isDestroying) return;

        if (!CanRunCoroutines)
        {
            // 降级：直接设 factor=0，尺寸轴向设为 0，然后 Destroy
            if (controller) controller.SetFactor(0f);
            if (rect)
            {
                Vector2 from = GetCurrentSize(rect);
                Vector2 to = new Vector2(
                    animateWidth ? 0f : from.x,
                    animateHeight ? 0f : from.y
                );
                SetSize(rect, to.x, to.y);
            }
            Destroy(gameObject);
            return;
        }

        StartCoroutine(Co_DestroySmooth_Sequential());
    }

    private void StartSizeIn()
    {
        if (!CanRunCoroutines)
        {
            if (rect)
            {
                Vector2 target = GetCurrentSize(rect);
                SetSize(rect, target.x, target.y);
            }
            return;
        }
        if (sizeCo != null) StopCoroutine(sizeCo);
        sizeCo = StartCoroutine(Co_SizeIn());
    }

    /// <summary>入场：先捕捉目标尺寸，再把勾选轴设为 1，接着拉伸到目标。</summary>
    private IEnumerator Co_SizeIn()
    {
        if (!rect) yield break;

        yield return null; // 等布局

        Vector2 target = GetCurrentSize(rect);
        Vector2 from = new Vector2(
            animateWidth ? 1f : target.x,
            animateHeight ? 1f : target.y
        );

        SetSize(rect, from.x, from.y);

        if (!animateWidth && !animateHeight)
        {
            sizeCo = null;
            yield break;
        }

        yield return Co_TweenSizeAxisAware(from, target, sizeInDuration, sizeInCurve);
        sizeCo = null;
    }

    /// <summary>按顺序的出场：先 factor→0，再宽高→0，最后销毁。</summary>
    private IEnumerator Co_DestroySmooth_Sequential()
    {
        isDestroying = true;

        // 清理在播
        if (factorCo != null) { StopCoroutine(factorCo); factorCo = null; }
        if (sizeCo != null) { StopCoroutine(sizeCo); sizeCo = null; }

        // 1) factor -> 0
        if (controller)
        {
            currentFactorTarget = 0f;
            yield return Co_TweenFactor(controller.factor, 0f, sizeOutFactorDuration, sizeOutCurve);
        }

        // 2) 尺寸 -> 0（仅勾选轴）
        if (rect && (animateWidth || animateHeight))
        {
            Vector2 from = GetCurrentSize(rect);
            Vector2 to = new Vector2(
                animateWidth ? 0f : from.x,
                animateHeight ? 0f : from.y
            );
            yield return Co_TweenSizeAxisAware(from, to, sizeOutShrinkDuration, sizeOutCurve);
        }

        Destroy(gameObject);
    }

    // =================== Tween 协程 ===================

    private void StartFactorTween(float from, float to, float duration, AnimationCurve curve)
    {
        if (!CanRunCoroutines || duration <= 0f)
        {
            SetFactorImmediate(to);
            return;
        }
        if (factorCo != null) StopCoroutine(factorCo);
        factorCo = StartCoroutine(Co_TweenFactor(from, to, duration, curve));
    }

    private IEnumerator Co_TweenFactor(float from, float to, float duration, AnimationCurve curve)
    {
        if (!controller) yield break;

        currentFactorTarget = to;

        if (duration <= 0f)
        {
            controller.SetFactor(to);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += DeltaTime();
            float u = Mathf.Clamp01(t / duration);
            if (curve != null) u = Mathf.Clamp01(curve.Evaluate(u));
            controller.SetFactor(Mathf.Lerp(from, to, u));
            yield return null;
        }
        controller.SetFactor(to);
    }

    private IEnumerator Co_TweenSizeAxisAware(Vector2 from, Vector2 to, float duration, AnimationCurve curve)
    {
        if (!rect) yield break;

        if (duration <= 0f)
        {
            float x = animateWidth ? to.x : from.x;
            float y = animateHeight ? to.y : from.y;
            SetSize(rect, x, y);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += DeltaTime();
            float u = Mathf.Clamp01(t / duration);
            if (curve != null) u = Mathf.Clamp01(curve.Evaluate(u));

            float x = animateWidth ? Mathf.Lerp(from.x, to.x, u) : to.x;
            float y = animateHeight ? Mathf.Lerp(from.y, to.y, u) : to.y;

            SetSize(rect, x, y);
            yield return null;
        }

        SetSize(rect, to.x, to.y);
    }

    // =================== 工具 ===================

    private static Vector2 GetCurrentSize(RectTransform rt) => rt.rect.size;

    private static void SetSize(RectTransform rt, float w, float h)
    {
        // 允许精确到 0
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(0f, w));
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, h));
    }

    private float DeltaTime() => ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
}
