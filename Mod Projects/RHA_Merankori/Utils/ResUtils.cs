using ChronoArkMod;
using ChronoArkMod.ModData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RHA_Merankori
{
    public static class ResUtils
    {
        private static readonly string ASSET_BUNDLE_NAME = "rha_merankoriunityassetbundle";

        public static int MaxCacheItems { get; set; } = 256;     // 最大条目数
        public static TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(30); // 每条目 TTL
        public static TimeSpan SweepInterval { get; set; } = TimeSpan.FromMinutes(10); // 清理频率

        // 缓存条目
        private class CacheEntry
        {
            public UnityEngine.Object Asset;
            public DateTime LastAccessUtc;
        }

        private static readonly Dictionary<string, CacheEntry> s_Cache =
            new Dictionary<string, CacheEntry>(StringComparer.Ordinal);

        private static readonly object s_Lock = new object();
        private static CacheJanitor s_Janitor;

        // 保证清道夫存在
        static ResUtils()
        {
            EnsureJanitor();
        }

        private static void EnsureJanitor()
        {
            if (s_Janitor != null) return;
            var go = new GameObject("[ResUtils.Janitor]");
            UnityEngine.Object.DontDestroyOnLoad(go);
            s_Janitor = go.AddComponent<CacheJanitor>();
            s_Janitor.Setup(SweepInterval, SweepOnce);
        }

        // 清理：过期 + 超容量（LRU）
        private static void SweepOnce()
        {
            lock (s_Lock)
            {
                if (s_Cache.Count == 0) return;

                var now = DateTime.UtcNow;
                // 先删除过期
                var toRemove = new List<string>();
                foreach (var kv in s_Cache)
                {
                    if (now - kv.Value.LastAccessUtc >= TimeToLive)
                        toRemove.Add(kv.Key);
                }
                foreach (var k in toRemove)
                    s_Cache.Remove(k);

                // 再做 LRU（如果仍超容量）
                if (s_Cache.Count > MaxCacheItems)
                {
                    // 按 LastAccess 排序，移除最老
                    var list = new List<KeyValuePair<string, CacheEntry>>(s_Cache);
                    list.Sort((a, b) => a.Value.LastAccessUtc.CompareTo(b.Value.LastAccessUtc));
                    int needRemove = s_Cache.Count - MaxCacheItems;
                    for (int i = 0; i < needRemove; i++)
                        s_Cache.Remove(list[i].Key);
                }
            }
        }

        // 统一的“取或加载并缓存”
        public static T LoadModAssetCached<T>(string assetPath) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("[RHA_Merankori] LoadModAssetCached: assetPath is null/empty.");
                return null;
            }

            lock (s_Lock)
            {
                if (s_Cache.TryGetValue(assetPath, out var entry) && entry.Asset != null)
                {
                    entry.LastAccessUtc = DateTime.UtcNow;
                    return entry.Asset as T;
                }
            }

            // 未命中：真实加载
            ModInfo info = ModManager.getModInfo(IDs.ID_Mod);
            T loaded = info.assetInfo.LoadFromAsset<T>(ASSET_BUNDLE_NAME, assetPath);

            if (loaded == null)
            {
                Debug.LogError($"[RHA_Merankori] Cannot Load Asset {assetPath}");
                return null;
            }

            lock (s_Lock)
            {
                s_Cache[assetPath] = new CacheEntry
                {
                    Asset = loaded,                 // 强引用：保证 TTL 内不被回收
                    LastAccessUtc = DateTime.UtcNow
                };
            }
            return loaded;
        }

        // Prefab 实例化：基于缓存的 Prefab
        public static GameObject InstantiateModPrefab(string assetPath, Transform parent = null, bool worldPositionStays = false)
        {
            var prefab = LoadModAssetCached<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogError($"[RHA_Merankori] Cannot Load Prefab {assetPath}");
                return null;
            }

            return parent == null
                ? UnityEngine.Object.Instantiate(prefab)
                : UnityEngine.Object.Instantiate(prefab, parent, worldPositionStays);
        }

        // 兼容你原来的接口（返回实例）
        public static GameObject LoadModPrefab(string assetPath)
        {
            return InstantiateModPrefab(assetPath);
        }

        // 非 Prefab 资源（贴图/材质等）：直接用缓存版本
        public static T LoadModAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return LoadModAssetCached<T>(assetPath);
        }

        // 手动失效某条缓存
        public static void Invalidate(string assetPath)
        {
            lock (s_Lock) s_Cache.Remove(assetPath);
        }

        // 清空缓存
        public static void ClearCache()
        {
            lock (s_Lock) s_Cache.Clear();
        }

        // 可选：即时调整参数（例如在设置菜单里）
        public static void Configure(int maxItems, TimeSpan ttl, TimeSpan sweepInterval)
        {
            MaxCacheItems = Mathf.Max(8, maxItems);
            TimeToLive = ttl < TimeSpan.FromSeconds(5) ? TimeSpan.FromSeconds(5) : ttl;
            SweepInterval = sweepInterval < TimeSpan.FromSeconds(5) ? TimeSpan.FromSeconds(5) : sweepInterval;
            if (s_Janitor != null) s_Janitor.Setup(SweepInterval, SweepOnce);
        }

        // 内部清道夫组件
        private class CacheJanitor : MonoBehaviour
        {
            private float _interval = 30f;
            private Action _sweep;
            private float _next;

            public void Setup(TimeSpan interval, Action sweep)
            {
                _interval = (float)Mathf.Max(5f, (float)interval.TotalSeconds);
                _sweep = sweep;
                _next = Time.realtimeSinceStartup + _interval;
            }

            private void Update()
            {
                if (_sweep == null) return;
                if (Time.realtimeSinceStartup >= _next)
                {
                    _next = Time.realtimeSinceStartup + _interval;
                    try { _sweep(); } catch (Exception e) { Debug.LogException(e); }
                }
            }
        }
    }
}
