using System;
using System.Collections.Generic;
using UnityEngine;
using UnityFarm.Core;

namespace UnityFarm.Gameplay
{
    /// <summary>
    /// 地块与种植系统：锄地、播种、浇水、收获、按天生长结算。
    /// 地块以网格坐标 (x, y) 为 key 管理，单例。
    /// </summary>
    public class CropSystem : MonoBehaviour
    {
        public static CropSystem Instance { get; private set; }

        [Tooltip("可用的作物数据（在 Inspector 里拖入 CropData 资产）")]
        public List<CropData> availableCrops = new List<CropData>();

        private readonly Dictionary<Vector2Int, FarmTile> _tiles = new Dictionary<Vector2Int, FarmTile>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // Start 在所有 Awake 之后执行，此时 TimeManager.Instance 已就绪
        private void Start()
        {
            if (TimeManager.Instance != null)
                TimeManager.Instance.OnDayEnd += OnDayEnd;
        }

        private void OnDestroy()
        {
            if (TimeManager.Instance != null)
                TimeManager.Instance.OnDayEnd -= OnDayEnd;
        }

        // ---- 查询 ----
        public FarmTile GetTile(Vector2Int pos)
        {
            _tiles.TryGetValue(pos, out var tile);
            return tile;
        }

        public CropData GetCropData(string cropId)
        {
            return availableCrops.Find(c => c.cropName == cropId);
        }

        /// <summary>枚举所有已开垦的地块（供视觉层同步显示）</summary>
        public IEnumerable<FarmTile> AllTiles()
        {
            return _tiles.Values;
        }

        // ---- 操作 ----
        public bool Till(Vector2Int pos)
        {
            var tile = GetOrCreate(pos);
            if (tile.tilled) return false; // 已是耕地
            tile.tilled = true;
            return true;
        }

        public bool Plant(Vector2Int pos, CropData crop)
        {
            if (crop == null) return false;
            var tile = GetOrCreate(pos);
            if (!tile.tilled || tile.IsPlanted) return false; // 需是耕地且未播种
            tile.cropId = crop.cropName;
            tile.growthDays = 0;
            return true;
        }

        public bool Water(Vector2Int pos)
        {
            var tile = GetTile(pos);
            if (tile == null || !tile.tilled || tile.wateredToday) return false;
            tile.wateredToday = true;
            return true;
        }

        /// <summary>收获成熟作物，返回作物数据和收获数量</summary>
        public bool Harvest(Vector2Int pos, out CropData crop, out int count)
        {
            crop = null;
            count = 0;
            var tile = GetTile(pos);
            if (tile == null || !tile.IsPlanted) return false;

            var data = GetCropData(tile.cropId);
            if (data == null) return false;

            if (tile.growthDays < data.daysToGrow) return false; // 未成熟

            crop = data;
            count = 1; // 原型：每个地块收获 1 个
            tile.cropId = "";
            tile.growthDays = 0;
            return true;
        }

        // ---- 生长结算（每天结束时调用）----
        /// <summary>推进一天：浇过水的作物生长，重置浇水状态</summary>
        public void AdvanceDay()
        {
            foreach (var tile in _tiles.Values)
            {
                if (tile.IsPlanted && tile.wateredToday)
                    tile.growthDays++;       // 浇过水才生长
                tile.wateredToday = false;   // 次日重置浇水状态
            }
        }

        private void OnDayEnd(int day)
        {
            AdvanceDay();
        }

        // ---- 存档支持 ----
        public List<FarmTile> SerializeTiles()
        {
            return new List<FarmTile>(_tiles.Values);
        }

        public void DeserializeTiles(List<FarmTile> tiles)
        {
            _tiles.Clear();
            if (tiles == null) return;
            foreach (var t in tiles)
                _tiles[t.Pos] = t;
        }

        private FarmTile GetOrCreate(Vector2Int pos)
        {
            if (!_tiles.TryGetValue(pos, out var tile))
            {
                tile = new FarmTile { x = pos.x, y = pos.y };
                _tiles[pos] = tile;
            }
            return tile;
        }
    }
}
