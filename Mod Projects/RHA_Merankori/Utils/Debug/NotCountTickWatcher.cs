// File: SkillNotCountTracer.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HarmonyLib;

public static class SkillNotCountTracer
{
    private const string HarmonyId = "mod.skill.notcount.tracer";
    private static Harmony _harmony;
    private static bool _enabled;

    // 配置
    private static int _minMsBetweenLogs = 200;
    private static bool _onlyWhenChanged = true;
    private static bool _onlyWhenTrue = false;

    // 每个实例的上次状态
    private class State
    {
        public bool LastValue;
        public long LastLogTicks;
        public bool HasLast;
    }

    private static ConditionalWeakTable<object, State> _states =
        new ConditionalWeakTable<object, State>();

    public static void Enable(int minMsBetweenLogs = 200, bool onlyWhenChanged = true, bool onlyWhenTrue = false)
    {
        _minMsBetweenLogs = Math.Max(0, minMsBetweenLogs);
        _onlyWhenChanged = onlyWhenChanged;
        _onlyWhenTrue = onlyWhenTrue;

        if (_enabled) return;
        _enabled = true;

        _harmony = new Harmony(HarmonyId);

        var getter = AccessTools.PropertyGetter(typeof(Skill), "NotCount");

        _harmony.Patch(
            getter,
            prefix: new HarmonyMethod(typeof(SkillNotCountTracer), nameof(Prefix)),
            postfix: new HarmonyMethod(typeof(SkillNotCountTracer), nameof(Postfix))
        );

        UnityEngine.Debug.Log("[SkillNotCountTracer] Enabled (Prefix + Postfix active)");
    }

    public static void Disable()
    {
        if (!_enabled) return;
        _enabled = false;
        try { _harmony?.UnpatchSelf(); } catch { }
        _states = new ConditionalWeakTable<object, State>();
        _harmony = null;
        UnityEngine.Debug.Log("[SkillNotCountTracer] Disabled");
    }

    // 可自定义过滤哪些 Skill 需要追踪
    private static bool ShouldTrace(Skill skill)
    {
        // 例如只看名叫“Fireball”的技能：
        // return skill?.Name == "Fireball";
        return true;
    }

    // ========== Prefix ==========
    private static void Prefix(Skill __instance)
    {
        if (!_enabled || __instance == null) return;
        if (!ShouldTrace(__instance)) return;

        // 限流：防止刷屏
        State st = _states.GetOrCreateValue(__instance);
        long now = DateTime.UtcNow.Ticks;
        if (_minMsBetweenLogs > 0 && st.LastLogTicks != 0)
        {
            long deltaMs = (now - st.LastLogTicks) / TimeSpan.TicksPerMillisecond;
            if (deltaMs < _minMsBetweenLogs) return;
        }

        st.LastLogTicks = now;

        string who = SafeSkillLabel(__instance);
        //UnityEngine.Debug.Log($"[Skill.get_NotCount][PRE] skill={who}\n{new StackTrace(true)}");
    }

    // ========== Postfix ==========
    private static void Postfix(Skill __instance, bool __result)
    {
        if (!_enabled || __instance == null) return;
        if (!ShouldTrace(__instance)) return;

        State st = _states.GetOrCreateValue(__instance);

        if (_onlyWhenTrue && !__result) return;
        if (_onlyWhenChanged && st.HasLast && st.LastValue == __result) return;

        st.LastValue = __result;
        st.HasLast = true;

        string who = SafeSkillLabel(__instance);
        UnityEngine.Debug.Log($"[Skill.get_NotCount][POST] skill={who} -> {__result}\n{new StackTrace(true)}");
    }

    private static string SafeSkillLabel(Skill s)
    {
        try
        {
            var type = s.GetType();
            var nameProp = type.GetProperty("Name");
            if (nameProp != null)
            {
                var v = nameProp.GetValue(s, null);
                if (v != null) return $"{type.Name}({v})";
            }
            var idProp = type.GetProperty("Id");
            if (idProp != null)
            {
                var v = idProp.GetValue(s, null);
                if (v != null) return $"{type.Name}#{v}";
            }
            return type.Name;
        }
        catch
        {
            return "Skill";
        }
    }
}
