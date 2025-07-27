using ChronoArkMod;
using ChronoArkMod.ModData;
using DarkTonic.MasterAudio;
using GameDataEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace RHA_Merankori
{
    /// <summary>
    /// 用于修改地图的工具类...
    /// 有亿点长
    /// </summary>
    public static class MapChange
    {
        private const float BASE_ITEM_CHANCE = 0.3f;


        public class BlowUpResult
        {
            public int totalTilesInrange = 0;
            public List<MapTile> hiddenTiles = new List<MapTile>();
            public List<MapTile> rewardTiles = new List<MapTile>();
            public List<MapTile> emptyTiles = new List<MapTile>();

            public bool CanBlowUP => rewardTiles.Count > 0 || emptyTiles.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldPos">世界坐标，比如玩家transform所在的坐标</param>
        /// <param name="range">多少格子，会等比例放大效果</param>
        public static void BlowUpParticleEffect(Vector3 worldPos, int range)
        {
            GameObject effectObject = AddressableLoadManager.Instantiate("Particle/Miss/MissChain_1", AddressableLoadManager.ManageType.None);
            effectObject.SetActive(true);
            //我们只想要特效，关掉无关的组件
            effectObject.GetComponent<SkillParticle>().enabled = false;
            effectObject.transform.localScale = new Vector3(1.0f, 0.3f, 1.0f) * ((range + 1) * 0.07f);
            Vector3 up = -Camera.main.transform.forward;
            //稍微向上一点，这样看起来有点透视
            effectObject.transform.up = (up + new Vector3(0, 0, 2.0f)).normalized;
            effectObject.transform.position = worldPos + new Vector3(0, 0, 1);
            effectObject.transform.SetAsLastSibling();
            ParticleSystem particleSystem = effectObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            //必须在Fog层才能正确渲染，order至少得这个数字，参考GhostEffect
            ParticleSystemUtility.SetParticleSystemSorting(effectObject, "Fog", -1860);
            AutoSelfDestoryComponent.AutoDestoryGameObject(effectObject, 15.0f);


            GameObject dustGO = ResUtils.LoadModPrefab("Assets/ModAssets/Content/Prefabs/blast_dust.prefab");
            dustGO.transform.localScale = Vector3.one * ((range + 1) * 1.4f);
            dustGO.transform.position = worldPos + new Vector3(0, 0, -0.1f);
            dustGO.transform.localEulerAngles = new Vector3(0, 0, 0);// = worldPos + new Vector3(0, 0, 0.5f);
            SpriteRenderer spriteRender = dustGO.GetComponent<SpriteRenderer>();
            ParticleSystemUtility.SetParticleSystemSorting(dustGO, "Floor", 13860);
            int sortingLayerID = SortingLayer.NameToID("Floor");
            if (spriteRender != null)
            {
                spriteRender.sortingLayerID = sortingLayerID;
                spriteRender.sortingOrder = 13850;
            }
            FieldSystemPatch.AddFieldGameObject(dustGO);
        }

        /// <summary>
        /// 摧毁范围内的HexTile
        /// 替换为Road
        /// 有一定概率能发现箱子（Event）
        /// TODO：支持稀有隐藏箱子，或者和HiddenWall联动
        /// </summary>
        /// <param name="tilePos">HexTile的坐标，也是数组下标</param>
        /// <param name="range">范围格子数</param>
        /// <returns></returns>
        public static BlowUpResult BlowUpTiles(Vector3 tilePos, int range)
        {
            if (
                StageSystem.instance == null ||
                StageSystem.instance.Map == null ||
                StageSystem.instance.Map.MapObject == null
                )
            {
                //Debug.Log($"BlowUpTile: Null StageSystem.instance or .Map, Skip");
                return null;
            }
            BlowUpResult result = new BlowUpResult();


            Vector3 cubePlayerPos = MapTile.VecToCube(tilePos);
            List<Vector2> candidatesHexPos = MapTile.MapRange(cubePlayerPos, range, StageSystem.instance.Map.Size);
            result.totalTilesInrange = candidatesHexPos.Count;
            //Debug.Log($"Get {candidatesHexPos.Count} pos, start to modify map");

            if (candidatesHexPos.Count == 0)
            {
                //Debug.Log($"BlowUpTile: No suitable map tile found, skip");
                return result;
            }

            //bool result = false;
            try
            {
                //对附近1格范围内的HexTile进行检查
                foreach (Vector2 cHexPos in candidatesHexPos)
                {
                    MapTile mapTile = StageSystem.instance.Map.MapObject[(int)cHexPos.x, (int)cHexPos.y];
                    BlowUpTile(mapTile, result);
                }
                Transform tileTransform = StageSystem.instance.Map.MapObject[(int)tilePos.x, (int)tilePos.y].TileObject.transform;
                MasterAudio.PlaySound("SE_FireEffect", 1f, null, 0f, null, null, false, false);
            }
            catch (Exception e)
            {
                Debug.LogError("BlowUpTile: encounter error!");
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }
            return result;
        }


        public static void DarkenMapTileColor(MapTile cMapTile)
        {
            DarkenMapTileColor(cMapTile, new Vector3(0.8f, 0.78f, 0.78f));
        }
        public static void DarkenMapTileColor(MapTile cMapTile, Vector3 factor)
        {
            List<SpriteRenderer> sprites = cMapTile.HexTileComponent.Sprites;
            foreach (var sr in sprites)
            {
                if (sr != null)
                {
                    sr.color = DarkenColor(sr.color, factor);
                }
            }
        }

        /// <summary>
        /// 摧毁单个Tile，概率发现小箱子
        /// </summary>
        /// <param name="cMapTile"></param>
        /// <returns></returns>
        public static bool BlowUpTile(MapTile cMapTile, BlowUpResult result)
        {
            if (
                StageSystem.instance == null ||
                StageSystem.instance.Map == null ||
                StageSystem.instance.Map.MapObject == null
                )
            {
                //Debug.Log($"BlowUpTile: Null StageSystem.instance or .Map, Skip");
                return false;
            }
            if (cMapTile == null)
            {
                //Debug.Log($"BlowUpTile: Null tile, Skip");
                return false;
            }
            Vector2 cHexPos = cMapTile.Pos;

            // 一点点变色，不好看，还是删了，自己做个特效更好康一些
            /*
            List<SpriteRenderer> sprites = cMapTile.HexTileComponent.Sprites;
            foreach (var sr in sprites)
            {
                if (sr != null)
                {
                    sr.color = DarkenColor(sr.color, new Vector3(0.99f, 0.96f, 0.96f));
                }
            }*/

            //不是Block/Border的话就不移除
            if (!(cMapTile.Info.Type is TileTypes.Block || cMapTile.Info.Type is TileTypes.Border))
            {
                //Debug.Log($"{cMapTile.Info.Type.GetType().Name} tile @{cHexPos}, Skip");
                //如果是隐藏，那么变黑一点
                if (cMapTile.Info.Type is TileTypes.HiddenWall)
                {
                    result.hiddenTiles.Add(cMapTile);
                }
                
                return false;
            }
            //Debug.Log($"Block tile @{cHexPos} {cMapTile.Info.Type.GetType().Name}, continue:");

            //移除墙壁，置换为road
            RemoveWallTile(cMapTile);

            //决定生成的事件和物品奖励
            float rollPoint = RandomManager.RandomFloat(BattleRandom.UseItem, 0.0f, 1.0f);
            if (rollPoint < BASE_ITEM_CHANCE)
            {
                //Debug.Log($"Block tile @{cHexPos}: Decide set up as item");
                GenerateRandomChestReward(cMapTile, out TileTypes.Event newEvent, out string rewardObjectKey);
                ReplaceTileWithEvent(cMapTile, rewardObjectKey, newEvent);
                result.rewardTiles.Add(cMapTile);
            }
            else
            {
                //Debug.Log($"Block tile @{cHexPos}: Decide set up as road");
                cMapTile.Info.Type = new TileTypes.Road();
                result.emptyTiles.Add(cMapTile);
            }

            //最后根据设定的内容，更新小地图
            CreateMiniMapHexTile(cMapTile);
            //Debug.Log($"Block tile @{cHexPos}, finish modification!");
            return true;
        }

        /// <summary>
        /// 将颜色变得暗一些
        /// </summary>
        /// <param name="orgColor"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        private static Color DarkenColor(Color orgColor, Vector3 factor)
        {
            float r = orgColor.r;
            float g = orgColor.g;
            float b = orgColor.b;
            float norm = 1.0f / Mathf.Sqrt(r * r + g * g + b * b);
            return new Color(
                r * norm * factor.x,
                g * norm * factor.y,
                b * norm * factor.z,
                orgColor.a
                );
        }

        /// <summary>
        /// 用于减少奖励...毕竟箱子多了奖励少一些也合理吧
        /// 虽然很多时候1个物品要去减少奖励...减不了啊
        /// 奖励至少留1个
        /// </summary>
        /// <param name="reward"></param>
        /// <param name="factor">1代表全奖励，0代表没奖励</param>
        private static void ReduceReward(List<ItemBase> reward, float factor)
        {
            reward.Shuffle();
            for (int i = 0; i < reward.Count; i++)
            {
                ItemBase item = reward[i];
                float targetStack = item.StackCount * factor;
                int ensureStack = (int)targetStack;
                float randomChance = targetStack - ensureStack;
                if (RandomManager.RandomFloat(BattleRandom.UseItem, 0.0f, 1.0f) < randomChance)
                {
                    ensureStack++;
                }

                if (reward.Count == 1 && ensureStack == 0) //maintain at least one reward
                {
                    break;
                }
                //Debug.Log($"Reduce Reward <{item.itemkey}> {item.StackCount} -> {ensureStack}");
                item.StackCount = ensureStack;
                if (ensureStack == 0)
                {
                    reward.RemoveAt(i);
                    i--;
                }
            }
            //合并金币奖励
            ItemBase goldItem = null;
            for (int i = 0; i < reward.Count; i++)
            {
                ItemBase item = reward[i];
                if(item.itemkey == GDEItemKeys.Item_Misc_Gold)
                {
                    if(goldItem!=null)
                    {
                        goldItem.StackCount += item.StackCount;
                        reward.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        goldItem = item;
                    }
                }
            }
        }

        /// <summary>
        /// 为MapTile设置新的miniMap相关信息。
        /// 注意Border和Wall类型的Tile，minimap对应下标的对象是null
        /// 我们必须创建对象，不然会出现空指针异常。
        /// 这个只考虑从Border和Wall转换为其他tile类型时的操作
        /// </summary>
        /// <param name="cMapTile"></param>
        public static void CreateMiniMapHexTile(MapTile cMapTile)
        {
            if (FieldSystem.instance == null ||
                FieldSystem.instance.MiniMap == null ||
                FieldSystem.instance.MiniMap.TeleportMap == null ||
                FieldSystem.instance.MiniMap.MapImages == null ||
                FieldSystem.instance.MiniMap.TeleportMap.MapImages == null
                )
            {
                Debug.Log("CreateMiniMapHexTile: null minimap!");
                return;
            }
            CreateHexTileForMini(cMapTile);
            CreateHexTileForTeleport(cMapTile);
        }

        /// <summary>
        /// 这个用于创建右上角小地图的标记
        /// 参考MiniMap.Init()
        /// </summary>
        /// <param name="cMapTile"></param>
        private static void CreateHexTileForMini(MapTile cMapTile)
        {
            HexTile cHexTile = cMapTile.HexTileComponent;
            MiniMap miniMap = FieldSystem.instance.MiniMap;
            MiniHex[,] mapImages = miniMap.MapImages;
            Vector2 pos = cMapTile.Info.Pos;
            int tx = (int)pos.x;
            int ty = (int)pos.y;
            MiniHex miniHex = mapImages[tx, ty];
            if (miniHex == null)
            {
                // Debug.Log("Mini map is null!");
                miniHex = mapImages[tx, ty] = cHexTile.GetComponent<MiniHex>();
                //Debug.Log($"new Mini map {miniHex}");
                if (miniHex == null)
                {
                    GameObject miniHexTileGameObject = GameObject.Instantiate<GameObject>(
                        miniMap.HexTilePrefab,
                        Vector2.zero,
                        Quaternion.identity,
                        miniMap.transform);
                    int localz = 0;
                    foreach (var miniH in mapImages)
                    {
                        if (miniH != null)
                        {
                            localz = miniH.transform.GetSiblingIndex();
                            break;
                        }
                    }
                    Vector3 localPos = miniMap.WorldToMinimap(cMapTile.TileObject.transform.position);
                    // 直接加会有深度问题（人物光标被地图标记遮挡），因此需要微调一下标记
                    miniHexTileGameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, 0);
                    miniHexTileGameObject.transform.SetSiblingIndex(localz + 1);
                    miniHex = mapImages[tx, ty] = miniHexTileGameObject.GetComponent<MiniHex>();
                    miniHex.MyTile = cMapTile;
                    miniHex.Pos = pos;
                    miniHex.Master = miniMap;
                    miniHexTileGameObject.SetActive(false);
                    miniHex.transform.localRotation = Quaternion.identity;
                    cMapTile.Info.MiniMap_On = true;
                    miniHex.On = cMapTile.Info.MiniMap_On;
                    miniHex.FalseView = cMapTile.Info.MiniMap_FalseView;
                    miniHexTileGameObject.SetActive(true);
                    miniHex.SightUpdate(false);
                }
            }
        }

        /// <summary>
        /// 这个是给全屏小地图用的，代码和上面那个差不多
        /// </summary>
        /// <param name="cMapTile"></param>
        private static void CreateHexTileForTeleport(MapTile cMapTile)
        {
            HexTile cHexTile = cMapTile.HexTileComponent;
            Map_Teleport miniMap = FieldSystem.instance.MiniMap.TeleportMap;
            MiniHex[,] mapImages = miniMap.MapImages;
            Vector2 pos = cMapTile.Info.Pos;
            int tx = (int)pos.x;
            int ty = (int)pos.y;
            MiniHex miniHex = mapImages[tx, ty];
            if (miniHex == null)
            {
                miniHex = mapImages[tx, ty] = cHexTile.GetComponent<MiniHex>();
                //Debug.Log($"new Mini map {miniHex}");
                if (miniHex == null)
                {
                    GameObject miniHexTileGameObject = GameObject.Instantiate<GameObject>(
                        miniMap._HexTilePrefab,
                        Vector2.zero,
                        Quaternion.identity,
                        miniMap.Align.transform);
                    int localz = 0;
                    foreach (var miniH in mapImages)
                    {
                        if (miniH != null)
                        {
                            localz = miniH.transform.GetSiblingIndex();
                            break;
                        }
                    }
                    Vector3 localPos = miniMap.WorldToMinimap(cMapTile.TileObject.transform.position);
                    // 深度问题导致需要一定量偏移：
                    miniHexTileGameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, 0);
                    miniHexTileGameObject.transform.SetSiblingIndex(localz + 1);
                    miniHex = mapImages[tx, ty] = miniHexTileGameObject.GetComponent<MiniHex>();
                    miniHex.MyTile = cMapTile;
                    miniHex.Pos = pos;
                    miniHex.Master = miniMap._HexViewer;
                    miniHexTileGameObject.SetActive(false);
                    miniHex.transform.localRotation = Quaternion.identity;
                    cMapTile.Info.MiniMap_On = true;
                    miniHex.On = cMapTile.Info.MiniMap_On;
                    miniHex.FalseView = cMapTile.Info.MiniMap_FalseView;
                    miniHexTileGameObject.SetActive(true);
                    miniHex.SightUpdate(false);
                }
            }
        }

        /// <summary>
        /// 将Road Tile替换成Event Tile。
        /// 这个过程请参考StageSystem.InstantiateIsometric
        /// 由于我们没有加载存档过程和特例，因此代码量不会几百行...
        /// </summary>
        /// <param name="cMapTile">要操作的Tile</param>
        /// <param name="eventKey">事件的key，所有的key变量名会带有FieldObject字样</param>
        /// <param name="newEvent">event具体信息</param>
        public static void ReplaceTileWithEvent(
            MapTile cMapTile,
            string eventKey,
            TileTypes.Event newEvent = null)
        {
            Debug.Log($"Replace HexTile {cMapTile.Info.Pos} with {eventKey}");

            if (newEvent == null)
            {
                newEvent = new TileTypes.Event();
            }

            //设置元信息
            if (cMapTile.Info == null)
            {
                cMapTile.Info = new TileInfo();
                Debug.LogError("ReplaceTileWithEvent: cMapTile.Info is null! this should not happen!");
                return;
            }
            cMapTile.Info.Type = newEvent;
            if (cMapTile.MyMap == null)
            {
                cMapTile.MyMap = StageSystem.instance.Map;
                Debug.LogWarning("ReplaceTileWithEvent: cMapTile.MyMap is null!");
            }
            cMapTile.MyMap.EventTileList.Add(cMapTile);
            HexTile cHexTile = cMapTile.HexTileComponent;


            //加载对象路径
            GDEFieldObjectData gdefieldObjectData = new GDEFieldObjectData(eventKey);
            newEvent.EventObject = gdefieldObjectData;
            //这里可以自定义用哪个配置，我这里使用默认关卡的配置
            string gameObjectPath = DecideWhichObject(newEvent.EventObject);
            Debug.Log($"Load gameObject path: {gameObjectPath}");
            if (gameObjectPath.IsNullOrEmpty())
            {
                Debug.LogError($"ReplaceTileWithEvent: Invalid gameObject path, skip!");
                return;
            }


            //创建对象
            GameObject newTileObject =
                AddressableLoadManager.Instantiate(
                gameObjectPath,
                AddressableLoadManager.ManageType.Stage,
                cHexTile.transform);
            //Debug.Log($"Create new object {newTileObject?.name}");
            if (newTileObject == null)
            {
                Debug.LogError("ReplaceTileWithEvent: Create object failed! Skip!");
                return;
            }
            newTileObject.transform.localPosition = Vector3.zero;
            newTileObject.SetActive(true);


            //创建处理事件的对象
            EventObject eventObject = newTileObject.GetComponent<EventObject>();
            if (eventObject != null)
            {
                eventObject.Init(gdefieldObjectData, cMapTile);
                cMapTile.TileEventObject = eventObject;
                BaseEventObject baseEventObject = newTileObject.GetComponent<BaseEventObject>();
                if (baseEventObject != null)
                {
                    //zhu码里有很多例外条件，我们这里只创造箱子，因此大幅度简化
                    baseEventObject.Init();
                    //这里应该还有已存档的数据处理，不过这边不可能调用存档，因此跳过
                }
            }

        }

        private static string DecideWhichObject(GDEFieldObjectData gdefieldObjectData)
        {
            string result = null;
            if (PlayData.TSavedata != null)
            {
                switch (PlayData.TSavedata.StageNum)
                {
                    case 0:
                    case 1:
                        result = gdefieldObjectData.Object_Path;
                        break;
                    case 2:
                    case 3:
                        result = gdefieldObjectData.Object2_Path;
                        break;
                    case 4:
                        result = gdefieldObjectData.Object3_Path;
                        break;
                    case 5:
                        result = gdefieldObjectData.Object4_Path;
                        break;
                }
            }
            //fall back，有的FieldObject某些字段可能为空
            if (result.IsNullOrEmpty())
            {
                result = gdefieldObjectData.Object4_Path;
            }
            if (result.IsNullOrEmpty())
            {
                result = gdefieldObjectData.Object3_Path;
            }
            if (result.IsNullOrEmpty())
            {
                result = gdefieldObjectData.Object2_Path;
            }
            if (result.IsNullOrEmpty())
            {
                result = gdefieldObjectData.Object_Path;
            }
            return result;
        }

        /// <summary>
        /// 移除Wall或Border的Tile，变成Road Tile。
        /// 推荐参考HexTile.HiddenWallOpen()和StageSystem.Purfication()
        /// </summary>
        /// <param name="cMapTile"></param>
        public static void RemoveWallTile(MapTile cMapTile)
        {
            HexTile cHexTile = cMapTile.HexTileComponent;
            Vector2 cHexPos = cMapTile.Info.Pos;
            IsoMap isoMap = StageSystem.instance.Map_Iso;
            MapTile_Iso[,] isoMapObject = isoMap.MapObject;
            //List<GameObject> tileBlocks = cHexTile.TileBlocks;

            //尝试构建碰撞箱，碰撞箱用于检测玩家是否进入这个格子
            //这个PosEnter碰撞箱始终为Trigger
            if (cMapTile.PosEnter == null)
            {
                //Debug.Log("cMapTile.PosEnter is null!");
                if (cHexTile.PosEnter != null)
                {
                    cMapTile.PosEnter = cHexTile.PosEnter;
                }
                else
                {
                    cMapTile.PosEnter = cMapTile.TileObject.AddComponent<PolygonCollider2D>();
                    cMapTile.PosEnter.isTrigger = true;
                    //Debug.Log("cMapTile.PosEnter create collider trigger");
                }
            }
            if (cHexTile.PosEnter == null)
            {
                //Debug.Log("cHexTile.PosEnter is null!");
                if (cMapTile.PosEnter != null)
                {
                    cHexTile.PosEnter = cMapTile.PosEnter;
                }
            }

            List<GameObject> tileBlocks = new List<GameObject>();
            //这边碰撞箱是真正会阻碍玩家行动的，因此要关掉
            foreach (MapTile_Iso mapTile_Iso in cHexTile.Tile.MyTiles)
            {
                //边界不要关闭..其实关闭也行，但是会不好看，我这里保留边界2格
                //每个HexTile起码有3x3或者4x4个Iso Tile
                Vector2 isoPos = mapTile_Iso.Info.Pos;
                int isoPosX = (int)isoPos.x;
                int isoPosY = (int)isoPos.x;
                if (
                    isoPosX == 0 ||
                    isoPosY == 0 ||
                    isoPosX == 1 ||
                    isoPosY == 1 ||
                    isoPosX == isoMapObject.GetLength(0) - 1 ||
                    isoPosY == isoMapObject.GetLength(1) - 1 ||
                    isoPosX == isoMapObject.GetLength(0) - 2 ||
                    isoPosY == isoMapObject.GetLength(1) - 2
                    )
                {
                    //边界的花花草草稍微变黑一点，表示摧毁不动
                    if (mapTile_Iso.DecoObject != null)
                    {
                        SpriteRenderer sr = mapTile_Iso.DecoObject.GetComponent<SpriteRenderer>();
                        if (sr == null)
                        {
                            sr = mapTile_Iso.TileObject.GetComponent<SpriteRenderer>();
                        }
                        if (sr != null)
                        {
                            sr.color = DarkenColor(sr.color, new Vector3(1.01f, 0.80f, 0.86f));
                        }
                    }
                    continue;
                }
                if (mapTile_Iso.DecoObject != null)
                {
                    tileBlocks.Add(mapTile_Iso.DecoObject);
                }
                mapTile_Iso.TileComponent.GetComponent<PolygonCollider2D>().enabled = false;
                //Debug.Log($"-- HexTile {mapTile_Iso.Info.HexOriginPos} > IsoTile {mapTile_Iso.Info.Pos}");
                //替换成road iso tile 
                mapTile_Iso.Info.Type = new TileTypes_Iso.Road();
                mapTile_Iso.Info.Type.Wall = false;
                mapTile_Iso.TileComponent.Wall = false;

            }

            //关闭装饰对象，并创建一个烧毁的动画效果
            foreach (GameObject tileBlock in tileBlocks)
            {
                //Debug.Log($"Block tile @{cHexPos}, try animate {tileBlock} @{tileBlock.transform.position}");
                GameObject copyTileBlock = GameObject.Instantiate(tileBlock, tileBlock.transform.parent);
                copyTileBlock.SetActive(false);
                Animator burnAnimator = copyTileBlock.AddComponent<Animator>();
                burnAnimator.runtimeAnimatorController = StageSystem.instance.BlockTileAnimation;
                copyTileBlock.AddComponent<_2dxFX_BurnFX>().Destroyed = 0f;
                cHexTile.HiddenWall.Add(copyTileBlock);
                FieldSystem.DelayInput(AnimatedRemoveDecoration(copyTileBlock, tileBlock));
            }
        }

        private static IEnumerator AnimatedRemoveDecoration(GameObject copyTileBlock, GameObject tileBlock)
        {
            copyTileBlock.SetActive(true);
            copyTileBlock.GetComponent<SpriteRenderer>().color = Color.white;
            Animator burnAnimator = copyTileBlock.GetComponent<Animator>();
            burnAnimator.SetBool("Burn", true);
            tileBlock.SetActive(false);
            yield break;
        }

        private static bool RandIs_Jar(float rp) => rp <= 0.1f;
        private static bool RandIs_BrokenCart(float rp) => rp <= 0.4f && rp > 0.1f;
        private static bool RandIs_GoldBag(float rp) => rp <= 0.45f && rp > 0.4f;
        private static bool RandIs_Skulls(float rp) => rp <= 0.5f && rp > 0.45f;
        private static bool RandIs_Chest(float rp) => rp > 0.5f;

        public static void GenerateRandomChestReward(
            MapTile cMapTile,
            out TileTypes.Event newEvent,
            out string rewardObjectKey)
        {

            // decide basic reward
            float rollPoint = RandomManager.RandomFloat(BattleRandom.UseItem, 0.0f, 1.0f);
            float reduceRewardRollPoint = RandomManager.RandomFloat(BattleRandom.UseItem, 0.1f, 0.36f);
            reduceRewardRollPoint *= reduceRewardRollPoint;
            if (rollPoint<0.05f)
            {
                reduceRewardRollPoint = Mathf.Clamp01((0.05f - rollPoint) * 8.0f);
            }
            //衰减为1%~9%，同时有5%的概率衰减为0~40%

            newEvent = new TileTypes.Event();

            rewardObjectKey = GDEItemKeys.FieldObject_F1_Chest;
            //隐藏箱子事件比较特殊，需要用别的手段来解决
            /*
            if (rollPoint <= 0.01f)
            {
                rewardObjectKey = GDEItemKeys.GameobjectDatas_Object_HiddenChest;
            }
            else*/
            if (RandIs_Jar(rollPoint))
            {
                List<ItemBase> overrideReward = new List<ItemBase>();
                rewardObjectKey = GDEItemKeys.FieldObject_O_S_F_Jar;
                overrideReward.AddRange(InventoryManager.RewardKey(GDEItemKeys.Reward_R_Jar, false));
                overrideReward.AddRange(GenerateAdditionalReward(rollPoint));
                ReduceReward(overrideReward, reduceRewardRollPoint);
                newEvent.PlusReward = overrideReward; //这个变量可以覆盖奖励列表，方便我们操纵奖励
            }
            else if (RandIs_BrokenCart(rollPoint))
            {
                List<ItemBase> overrideReward = new List<ItemBase>();
                rewardObjectKey = GDEItemKeys.FieldObject_O_S_F_Wagon;
                overrideReward.AddRange(InventoryManager.RewardKey(GDEItemKeys.Reward_R_BrokenCart, false));
                overrideReward.AddRange(GenerateAdditionalReward(rollPoint));
                ReduceReward(overrideReward, reduceRewardRollPoint);
                newEvent.PlusReward = overrideReward;
            }
            else if (RandIs_GoldBag(rollPoint))
            {
                rewardObjectKey = GDEItemKeys.FieldObject_O_S_F_GoldBag;
            }
            else if (RandIs_Skulls(rollPoint))
            {
                rewardObjectKey = GDEItemKeys.FieldObject_O_S_F_Skulls;
            }
            else
            {
                List<ItemBase> overrideReward = new List<ItemBase>();
                rewardObjectKey = GDEItemKeys.FieldObject_F1_Chest;
                overrideReward.AddRange(InventoryManager.RewardKey(GDEItemKeys.Reward_Object_S, false));
                overrideReward.AddRange(GenerateAdditionalReward(rollPoint));
                ReduceReward(overrideReward, reduceRewardRollPoint);
                newEvent.PlusReward = overrideReward;
            }

            Debug.Log($"Gen Reward @{cMapTile.Info.Pos}: roll {rollPoint}={rewardObjectKey} & {reduceRewardRollPoint} => OverrideRwardCount {newEvent.PlusReward?.Count}");

        }


        // 额外奖励池子，根据rollPoint决定
        private static List<ItemBase> GenerateAdditionalReward(float rollPoint)
        {
            int randomInt = (int)(rollPoint * 10000.0f) % 100; //0~99
            List<ItemBase> result = new List<ItemBase>();
            //--------------------------------------

            for (int i = 0; i < 2; i++)
            {
                // 金币
                int goldStack = (randomInt % 13 * i + 50) * (randomInt % 5 + 2);
                // 1% 概率只有1枚金币，或者10倍金币
                if (randomInt == 1)
                {
                    goldStack = 1;
                }
                else if (randomInt == 2)
                {
                    goldStack *= 70;
                }
                // 部分事件会有2倍的数值
                if (RandIs_BrokenCart(rollPoint))
                {
                    goldStack *= 2;
                }
                result.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, goldStack));


                // 面包
                int breadStack = 1;
                if (randomInt == 3)
                {
                    breadStack = 50;
                }
                else if (randomInt < 30)
                {
                    breadStack = 2;
                }
                result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_Bread, breadStack));

                //卷轴
                if (randomInt == 4)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Item, 50));
                }
                else if (randomInt == 5)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Teleport, 50));
                }
                else if (randomInt == 6)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Teleport, 200));
                }
                else if (randomInt == 7)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Mapping, 15));
                }
                else if (randomInt < 20)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Item, 2));
                }
                else if (randomInt < 30)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Uncurse, 2));
                }
                else if (randomInt < 40)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Vitality));
                }
                else if (randomInt < 50)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Quick, 3));
                }
                else if (randomInt < 55)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Purification, 1));
                }
                else if (randomInt < 60)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Transfer, 1));
                }
                else if (randomInt < 90)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Enchant, 4));
                }


                //废铁
                if (randomInt > 80)
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Scrap_1));
                }
                else
                {
                    result.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Scrap_0));
                }

            }

            //灵魂石
            if (randomInt % 20 == 0)
            {
                result.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul,1));
            }

            //稀有物品
            if (RandIs_Jar(rollPoint))
            {
                switch (randomInt)
                {
                    case 50:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_TimeMoney));
                        break;
                    case 49:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_RWEnterItem));
                        break;
                    case 48:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul,10));
                        break;
                    case 47:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_ArtifactPouch));
                        break;
                    case 46:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_EquipPouch));
                        break;
                    case 45:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_GoldenApple));
                        break;
                    case 44:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_GoldenBread));
                        break;
                    case 1:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_SkillBookCharacter_Rare));
                        break;
                    case 42:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_SkillBookInfinity));
                        break;
                    case 41:
                        result.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_SkillBookLucy));
                        break;
                }
            }
            Debug.Log($"Extra pool rand={randomInt}, gen {result.Count} candidates");
            return result;
        }


    }
}
