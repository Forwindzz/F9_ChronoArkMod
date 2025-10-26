// ResUtils.cs
using ChronoArkMod;
using ChronoArkMod.ModData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RHA_Merankori
{
    /// <summary>
    /// 统一资源加载与实例化（含 LRU 缓存）：
    /// - Mod 资源：通过 Mod 的 AssetBundle 加载，进入 LRU（不 Addressables.Release）
    /// - Base 资源：直接调用 AddressableLoadManager.LoadAddressableAsset<T>；必要时回退 Unity Addressables
    /// - Prefab 实例化：优先使用缓存好的 Prefab；失败兜底 AddressableLoadManager.Instantiate
    /// - LRU 清理：Base 条目 Addressables.Release(asset)；Mod 条目不释放
    /// </summary>
    public static class ResUtils
    {
        private static readonly string ASSET_BUNDLE_NAME = "rha_merankoriunityassetbundle";

        public static int MaxCacheItems { get; set; } = 256;
        public static TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(30);
        public static TimeSpan SweepInterval { get; set; } = TimeSpan.FromMinutes(10);

        // 记录来源，便于释放策略
        private enum CacheSource
        {
            Mod,                // 通过 Mod bundle 加载
            BaseAddrMgr,        // 通过 AddressableLoadManager.LoadAddressableAsset 加载
            BaseAddressables    // 通过 Unity Addressables.LoadAssetAsync 加载（回退）
        }

        private class CacheEntry
        {
            public UnityEngine.Object Asset;
            public DateTime LastAccessUtc;
            public CacheSource Source;
        }

        private static readonly Dictionary<string, CacheEntry> s_Cache =
            new Dictionary<string, CacheEntry>(StringComparer.Ordinal);

        private static readonly object s_Lock = new object();
        private static CacheJanitor s_Janitor;

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

        // =============== 公共 API（Mod 侧） ===============

        public static T LoadModAsset<T>(string assetPath) where T : UnityEngine.Object
            => LoadModAssetCached<T>(assetPath);

        public static GameObject LoadModPrefab(string assetPath)
            => InstantiateModPrefab(assetPath);

        public static GameObject InstantiateModPrefab(string assetPath, Transform parent = null, bool worldPositionStays = false)
        {
            var prefab = LoadModAssetCached<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogError($"[RHA_Merankori] Cannot Load Mod Prefab {assetPath}");
                return null;
            }
            return parent == null
                ? UnityEngine.Object.Instantiate(prefab)
                : UnityEngine.Object.Instantiate(prefab, parent, worldPositionStays);
        }

        // =============== 公共 API（Base 侧） ===============

        /// <summary>
        /// 载入原版/基座 Addressables 资源并进入统一 LRU。
        /// 首选 AddressableLoadManager.LoadAddressableAsset；若返回 null，再回退 Unity Addressables。
        /// </summary>
        public static T LoadBaseGameAsset<T>(string key) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[RHA_Merankori] LoadBaseGameAsset: key is null/empty.");
                return null;
            }

            string cacheKey = $"base::{key}";
            lock (s_Lock)
            {
                if (s_Cache.TryGetValue(cacheKey, out var hit) && hit.Asset != null)
                {
                    hit.LastAccessUtc = DateTime.UtcNow;
                    return hit.Asset as T;
                }
            }

            // 1) 首选：项目原有 AddressableLoadManager（同步 WaitForCompletion）
            T loaded = AddressableLoadManager.LoadAddressableAsset<T>(key);
            if (loaded != null)
            {
                lock (s_Lock)
                {
                    s_Cache[cacheKey] = new CacheEntry
                    {
                        Asset = loaded,
                        LastAccessUtc = DateTime.UtcNow,
                        Source = CacheSource.BaseAddrMgr
                    };
                }
                return loaded;
            }

            // 2) 回退：Unity Addressables（同步 WaitForCompletion）
            try
            {
                var h = Addressables.LoadAssetAsync<T>(key);
                loaded = h.WaitForCompletion();
                if (loaded == null)
                {
                    Debug.LogError($"[RHA_Merankori] LoadBaseGameAsset<{typeof(T).Name}>('{key}') returned null.");
                    return null;
                }

                lock (s_Lock)
                {
                    s_Cache[cacheKey] = new CacheEntry
                    {
                        Asset = loaded,
                        LastAccessUtc = DateTime.UtcNow,
                        Source = CacheSource.BaseAddressables
                    };
                }
                return loaded;
            }
            catch (Exception e)
            {
                Debug.LogError($"[RHA_Merankori] LoadBaseGameAsset<{typeof(T).Name}>('{key}') failed: {e}");
                return null;
            }
        }

        /// <summary>
        /// 实例化原版 Prefab：优先用缓存 Prefab，兜底 AddressableLoadManager.Instantiate。
        /// </summary>
        public static GameObject InstantiateBaseGamePrefab(
            string key,
            Transform parent = null,
            AddressableLoadManager.ManageType manageType = AddressableLoadManager.ManageType.Stage,
            bool worldPositionStays = false)
        {
            // 优先：缓存的 Prefab（通过 LoadBaseGameAsset<GameObject> 进入）
            var prefab = LoadBaseGameAsset<GameObject>(key);
            if (prefab != null)
            {
                return parent == null
                    ? UnityEngine.Object.Instantiate(prefab)
                    : UnityEngine.Object.Instantiate(prefab, parent, worldPositionStays);
            }

            // 兜底：项目现有同步 Instantiate
            try
            {
                return AddressableLoadManager.Instantiate(key, manageType, parent);
            }
            catch (Exception e)
            {
                Debug.LogError($"[RHA_Merankori] InstantiateBaseGamePrefab('{key}') fallback failed: {e}");
                return null;
            }
        }

        // =============== 内部：Mod 加载并入 LRU ===============

        public static T LoadModAssetCached<T>(string assetPath) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("[RHA_Merankori] LoadModAssetCached: assetPath is null/empty.");
                return null;
            }

            string cacheKey = $"mod::{assetPath}";
            lock (s_Lock)
            {
                if (s_Cache.TryGetValue(cacheKey, out var hit) && hit.Asset != null)
                {
                    hit.LastAccessUtc = DateTime.UtcNow;
                    return hit.Asset as T;
                }
            }

            ModInfo info = ModManager.getModInfo(IDs.ID_Mod);
            T loaded = info.assetInfo.LoadFromAsset<T>(ASSET_BUNDLE_NAME, assetPath);
            if (loaded == null)
            {
                Debug.LogError($"[RHA_Merankori] Cannot Load Mod Asset {assetPath}");
                return null;
            }

            lock (s_Lock)
            {
                s_Cache[cacheKey] = new CacheEntry
                {
                    Asset = loaded,
                    LastAccessUtc = DateTime.UtcNow,
                    Source = CacheSource.Mod
                };
            }
            return loaded;
        }

        // =============== 缓存维护 / 清理 ===============

        // 清理：过期 + LRU 超容，并对 Base 条目调用 Addressables.Release(asset)
        private static void SweepOnce()
        {
            lock (s_Lock)
            {
                if (s_Cache.Count == 0) return;

                var now = DateTime.UtcNow;

                // 过期清理
                var expired = new List<string>();
                foreach (var kv in s_Cache)
                {
                    if (now - kv.Value.LastAccessUtc >= TimeToLive)
                        expired.Add(kv.Key);
                }
                foreach (var k in expired)
                {
                    ReleaseIfBaseAddressable(s_Cache[k]);
                    s_Cache.Remove(k);
                }

                // LRU 超容
                if (s_Cache.Count > MaxCacheItems)
                {
                    var list = new List<KeyValuePair<string, CacheEntry>>(s_Cache);
                    list.Sort((a, b) => a.Value.LastAccessUtc.CompareTo(b.Value.LastAccessUtc));
                    int needRemove = s_Cache.Count - MaxCacheItems;
                    for (int i = 0; i < needRemove; i++)
                    {
                        var key = list[i].Key;
                        ReleaseIfBaseAddressable(s_Cache[key]);
                        s_Cache.Remove(key);
                    }
                }
            }
        }

        // 仅对 Base（Addressables）资产执行 Release
        private static void ReleaseIfBaseAddressable(CacheEntry entry)
        {
            if (entry == null || entry.Asset == null) return;

            if (entry.Source == CacheSource.BaseAddrMgr || entry.Source == CacheSource.BaseAddressables)
            {
                try
                {
                    Addressables.Release(entry.Asset);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[RHA_Merankori] Addressables.Release(asset) failed: {e.Message}");
                }
            }

            entry.Asset = null;
        }

        public static void Invalidate(string pathOrKey, bool isBase = false)
        {
            string cacheKey = isBase ? $"base::{pathOrKey}" : $"mod::{pathOrKey}";
            lock (s_Lock)
            {
                if (s_Cache.TryGetValue(cacheKey, out var ent))
                {
                    ReleaseIfBaseAddressable(ent);
                    s_Cache.Remove(cacheKey);
                }
            }
        }

        public static void ClearCache()
        {
            lock (s_Lock)
            {
                foreach (var kv in s_Cache)
                    ReleaseIfBaseAddressable(kv.Value);
                s_Cache.Clear();
            }
        }

        public static void Configure(int maxItems, TimeSpan ttl, TimeSpan sweepInterval)
        {
            MaxCacheItems = Mathf.Max(8, maxItems);
            TimeToLive = ttl < TimeSpan.FromSeconds(5) ? TimeSpan.FromSeconds(5) : ttl;
            SweepInterval = sweepInterval < TimeSpan.FromSeconds(5) ? TimeSpan.FromSeconds(5) : sweepInterval;
            if (s_Janitor != null) s_Janitor.Setup(SweepInterval, SweepOnce);
        }

        // =============== 内部清道夫 ===============

        private class CacheJanitor : MonoBehaviour
        {
            private float _interval = 30f;
            private Action _sweep;
            private float _next;

            public void Setup(TimeSpan interval, Action sweep)
            {
                _interval = Mathf.Max(5f, (float)interval.TotalSeconds);
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
