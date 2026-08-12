using System;
using UnityEngine;

namespace UnityFarm.Gameplay
{
    /// <summary>
    /// 单个地块的状态（由 CropSystem 管理，可被 JSON 序列化存档）。
    /// </summary>
    [Serializable]
    public class FarmTile
    {
        public int x;              // 网格坐标 x
        public int y;              // 网格坐标 y
        public bool tilled;        // 是否为耕地（锄过地）
        public string cropId = ""; // 已播种的作物 id（CropData.cropName），空 = 未播种
        public int growthDays;     // 已生长天数
        public bool wateredToday;  // 今天是否浇过水

        public bool IsPlanted => !string.IsNullOrEmpty(cropId);
        public bool IsWatered => wateredToday;

        public Vector2Int Pos => new Vector2Int(x, y);
    }
}
