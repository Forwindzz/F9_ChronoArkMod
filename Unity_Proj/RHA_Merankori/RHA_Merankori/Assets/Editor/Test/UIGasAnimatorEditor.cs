#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIGasAnimator))]
public class UIGasAnimatorInspector : Editor
{
    // 临时的 Inspector-only 输入缓冲（不会写回组件）
    private float customTarget = 0.5f;
    private float customDuration = -1f; // -1 表示用默认

    public override void OnInspectorGUI()
    {
        // 先画默认 Inspector（让你还能改原有序列化字段）
        DrawDefaultInspector();

        var anim = (UIGasAnimator)target;

        EditorGUILayout.Space();
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("Quick Factor Controls", EditorStyles.boldLabel);

            // 当前 factor 显示（仅读）
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Current Factor", GUILayout.Width(110));
                string factorStr = "(n/a)";
                // 若你给 UIGasAnimator 加了 GetCurrentFactor()，可更稳妥读值
                try
                {
                    factorStr = anim != null ? anim.GetCurrentFactor().ToString("0.###") : "(n/a)";
                }
                catch { }
                EditorGUILayout.LabelField(factorStr);
            }

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("→ 0")) anim.SetFactorSmooth(0f);
                if (GUILayout.Button("→ 0.25")) anim.SetFactorSmooth(0.25f);
                if (GUILayout.Button("→ 0.5")) anim.SetFactorSmooth(0.5f);
                if (GUILayout.Button("→ 0.75")) anim.SetFactorSmooth(0.75f);
                if (GUILayout.Button("→ 1")) anim.SetFactorSmooth(1f);
            }

            if (GUILayout.Button("→ Random"))
                anim.SetFactorSmooth(Random.Range(0f, 1f));

            EditorGUILayout.Space();

            // 自定义目标 + 时长
            using (new EditorGUILayout.HorizontalScope())
            {
                customTarget = EditorGUILayout.Slider("Target", customTarget, 0f, 1f);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                customDuration = EditorGUILayout.FloatField(new GUIContent("Duration (s)", "≤0 或 -1 使用组件默认时长"), customDuration);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Play To Target"))
                {
                    float d = (customDuration <= 0f) ? -1f : customDuration; // -1 走默认
                    anim.SetFactorSmooth(customTarget, d, null);
                }
                if (GUILayout.Button("Immediate Set"))
                {
                    anim.SetFactorImmediate(customTarget);
                }
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Stop Factor Tween")) anim.StopFactorTween();
                if (GUILayout.Button("Replay Size In")) anim.ReplaySizeIn();
                if (GUILayout.Button("Destroy Smooth")) anim.DestroySmooth();
            }

            EditorGUI.EndDisabledGroup();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("进入 Play Mode 后按钮可用。", MessageType.Info);
            }
        }
    }
}
#endif
