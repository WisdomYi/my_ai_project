#if UNITY_EDITOR
using UnityEngine;
using UnityFarm.Core;
using UnityFarm.Gameplay;

namespace UnityFarm.EditorTools
{
    /// <summary>
    /// 在 batchmode 下验证核心种植循环逻辑（锄地→播种→浇水→生长→收获→存档）。
    /// 运行：Tuanjie.exe -batchmode -projectPath ... -executeMethod UnityFarm.EditorTools.FarmingLoopValidator.Validate -quit
    /// </summary>
    public static class FarmingLoopValidator
    {
        public static void Validate()
        {
            bool allPassed = true;

            // 造一份作物数据
            var cropData = ScriptableObject.CreateInstance<CropData>();
            cropData.cropName = "萝卜";
            cropData.seedItemName = "萝卜种子";
            cropData.harvestItemName = "萝卜";
            cropData.daysToGrow = 3;

            // CropSystem（用真实 GameObject 挂组件，触发 Awake 逻辑）
            var go = new GameObject("TestCropSystem");
            var cropSystem = go.AddComponent<CropSystem>();
            cropSystem.availableCrops.Add(cropData);

            // 背包，初始 5 个种子
            var inventory = new Inventory();
            inventory.Add("萝卜种子", 5);

            var tile = new Vector2Int(0, 0);

            // 1. 锄地
            Log("锄地", cropSystem.Till(tile), ref allPassed);

            // 2. 播种（消耗种子）
            bool plant = inventory.Remove("萝卜种子") && cropSystem.Plant(tile, cropData);
            Log("播种", plant, ref allPassed);

            // 3. 浇水
            Log("浇水", cropSystem.Water(tile), ref allPassed);

            // 4. 生长 3 天（每天浇一次水）
            for (int i = 0; i < 3; i++)
            {
                cropSystem.AdvanceDay();
                cropSystem.Water(tile);
            }

            // 5. 收获
            bool harvest = cropSystem.Harvest(tile, out var crop, out var count);
            Log("收获", harvest && count == 1 && crop.cropName == "萝卜", ref allPassed);
            if (harvest) inventory.Add(crop.harvestItemName, count);

            // 6. 背包校验：收获 1 个萝卜，种子剩 4
            bool invOk = inventory.Count("萝卜") == 1 && inventory.Count("萝卜种子") == 4;
            Log("背包(萝卜×1、种子×4)", invOk, ref allPassed);

            // 7. 存档序列化往返
            var save = new SaveData
            {
                day = 7,
                tiles = cropSystem.SerializeTiles(),
                inventory = inventory.items
            };
            var loaded = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save));
            bool saveOk = loaded.day == 7 && loaded.inventory.Count == 2 && loaded.tiles.Count == 1;
            Log("存档序列化往返", saveOk, ref allPassed);

            Debug.Log(allPassed ? "=== 种植循环验证全部通过 ===" : "=== 存在失败项，见上方日志 ===");
            Object.DestroyImmediate(go);
        }

        private static void Log(string name, bool pass, ref bool allPassed)
        {
            Debug.Log($"[验证] {name}: {(pass ? "通过" : "失败")}");
            if (!pass) allPassed = false;
        }
    }
}
#endif
