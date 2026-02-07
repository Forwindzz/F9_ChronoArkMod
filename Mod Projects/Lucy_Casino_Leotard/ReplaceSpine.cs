using ChronoArkMod;
using ChronoArkMod.ModData;
using HarmonyLib;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Lucy_Casino_Leotard
{
    [HarmonyPatch]
    static class PlayerControllerPatch
    {
        [HarmonyPatch(
            typeof(PlayerController),
            nameof(PlayerController.RoguelikePartSpine))]
        [HarmonyPrefix]
        static void PlayerController_RoguelikePartSpine_Prefix(
            PlayerController __instance
            )
        {
            if (FieldSystem.instance == null)
            {
                return;
            }
            ModInfo info = ModManager.getModInfo(IDs.ID_Mod);
            string imagePath = Path.Combine(info.DirectoryName, "Assets", "lucySD.png");
            Sprite newSprite = SpineSkinReplacer.LoadSprite(imagePath);
            SpineSkinReplacer.ReplaceAtlasTextureForSkin(__instance.Spinedata, "Casino_Skin", newSprite.texture);
        }
    }

    public static class SpineSkinReplacer
    {
        public static int ReplaceAtlasTextureForSkin(
            SkeletonAnimation sa,
            string skinName,
            Texture2D newAtlasTex,
            Material materialPropertySource = null)
        {
            var skeleton = sa.Skeleton;
            var data = skeleton.Data;

            var skin = data.FindSkin(skinName);
            if (skin == null) { Debug.LogError($"Skin not found: {skinName}"); return 0; }

            // 选一个模板材质：确保 shader / stencil / blend 等一致
            if (materialPropertySource == null)
                materialPropertySource = sa.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;

            // 这套材质只给这个 skin 用
            var skinMat = new Material(materialPropertySource);
            skinMat.mainTexture = newAtlasTex;

            // 一个 skin 通常只需要一个 page（如果你确定只有一张 atlas）
            var newPage = new AtlasPage
            {
                rendererObject = skinMat,
                width = newAtlasTex.width,
                height = newAtlasTex.height
            };

            // 缓存：同一个 oldRegion 不要重复 clone（一个 region 可能被多个 attachment 引用）
            var regionMap = new Dictionary<AtlasRegion, AtlasRegion>();

            int changed = 0;

            foreach (var entry in skin.GetAttachments())
            {
                var att = entry.Attachment;

                if (att is RegionAttachment ra)
                {
                    var oldRegion = ra.RendererObject as AtlasRegion;
                    if (oldRegion == null) continue;

                    var newRegion = GetOrCloneRegion(oldRegion, newPage, regionMap);

                    ra.RendererObject = newRegion;
                    ra.SetRegion(newRegion);
                    ra.UpdateOffset();
                    changed++;
                }
                else if (att is MeshAttachment ma)
                {
                    // Mesh 也是挂 AtlasRegion（RendererObject 通常也是 AtlasRegion）
                    var oldRegion = ma.RendererObject as AtlasRegion;
                    if (oldRegion == null) continue;

                    var newRegion = GetOrCloneRegion(oldRegion, newPage, regionMap);

                    ma.RendererObject = newRegion;
                    changed++;
                }
            }

            // 如果当前 skeleton 正在用这个 skin，刷新一下
            skeleton.SetSlotsToSetupPose();
            sa.AnimationState.Apply(skeleton);

            return changed;
        }

        static AtlasRegion GetOrCloneRegion(
            AtlasRegion oldRegion,
            AtlasPage newPage,
            Dictionary<AtlasRegion, AtlasRegion> regionMap)
        {
            if (regionMap.TryGetValue(oldRegion, out var cached))
                return cached;

            // 复制 oldRegion 的所有关键字段：区域、uv、rotate、offset 等完全保持
            var nr = new AtlasRegion
            {
                name = oldRegion.name,
                page = newPage,

                x = oldRegion.x,
                y = oldRegion.y,
                width = oldRegion.width,
                height = oldRegion.height,
                originalWidth = oldRegion.originalWidth,
                originalHeight = oldRegion.originalHeight,
                offsetX = oldRegion.offsetX,
                offsetY = oldRegion.offsetY,
                rotate = oldRegion.rotate,

                u = oldRegion.u,
                v = oldRegion.v,
                u2 = oldRegion.u2,
                v2 = oldRegion.v2,
            };

            regionMap.Add(oldRegion, nr);
            return nr;
        }

        /// <summary>
        /// 从本地PNG路径加载为Sprite
        /// </summary>
        public static Sprite LoadSprite(string path, float pixelsPerUnit = 100f)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("Image Not Exists: " + path);
                return null;
            }

            byte[] imageBytes = File.ReadAllBytes(path);

            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            if (!tex.LoadImage(imageBytes))
            {
                Debug.LogError("PNG load failed: " + path);
                return null;
            }

            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), // pivo
                pixelsPerUnit
            );

            return sprite;
        }
    }





}