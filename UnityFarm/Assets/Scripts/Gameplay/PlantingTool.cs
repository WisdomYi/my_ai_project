using UnityEngine;
using UnityFarm.Core;

namespace UnityFarm.Gameplay
{
    /// <summary>工具类型：锄地、播种、浇水、收获</summary>
    public enum Tool { Hoe, Seed, WateringCan, Harvest }

    /// <summary>
    /// 工具交互：对角色朝向的前方地块执行当前工具操作。
    /// 原型按键：1=锄头 2=种子 3=水壶 4=收获，空格=执行，WASD=移动。
    /// </summary>
    public class PlantingTool : MonoBehaviour
    {
        public Tool currentTool = Tool.Hoe;

        [Tooltip("交互距离（格）")]
        public float interactionRange = 1f;

        [Tooltip("播种时使用的作物（在 Inspector 拖入 CropData 资产）")]
        public CropData selectedCrop;

        private Vector2 _facing = Vector2.down;

        private void Update()
        {
            UpdateFacing();
            SwitchTool();
            if (Input.GetKeyDown(KeyCode.Space)) Interact();
        }

        private void UpdateFacing()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(x) > 0.01f || Mathf.Abs(y) > 0.01f)
                _facing = new Vector2(x, y).normalized;
        }

        private void SwitchTool()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = Tool.Hoe;
            if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = Tool.Seed;
            if (Input.GetKeyDown(KeyCode.Alpha3)) currentTool = Tool.WateringCan;
            if (Input.GetKeyDown(KeyCode.Alpha4)) currentTool = Tool.Harvest;
        }

        private void Interact()
        {
            var cropSystem = CropSystem.Instance;
            if (cropSystem == null) { Debug.LogWarning("场景中缺少 CropSystem"); return; }

            var target = GetTargetTile();
            switch (currentTool)
            {
                case Tool.Hoe:
                    Debug.Log(cropSystem.Till(target) ? $"锄地: {target}" : $"锄地失败（可能已是耕地）: {target}");
                    break;

                case Tool.Seed:
                    if (selectedCrop == null) { Debug.Log("未选择种子作物（在 Inspector 给 PlantingTool 配 selectedCrop）"); break; }
                    if (GameManager.Instance != null && !GameManager.Instance.Inventory.Remove(selectedCrop.seedItemName))
                    { Debug.Log($"背包里没有 {selectedCrop.seedItemName}"); break; }

                    if (cropSystem.Plant(target, selectedCrop))
                        Debug.Log($"播种 {selectedCrop.cropName} 于 {target}");
                    else
                    {
                        GameManager.Instance?.Inventory.Add(selectedCrop.seedItemName); // 失败退回种子
                        Debug.Log($"播种失败（需先锄地且未播种）: {target}");
                    }
                    break;

                case Tool.WateringCan:
                    Debug.Log(cropSystem.Water(target) ? $"浇水: {target}" : $"浇水失败（需是耕地且今天未浇）: {target}");
                    break;

                case Tool.Harvest:
                    if (cropSystem.Harvest(target, out var crop, out var count))
                    {
                        GameManager.Instance?.Inventory.Add(crop.harvestItemName, count);
                        Debug.Log($"收获 {crop.harvestItemName}×{count} 于 {target}");
                    }
                    else
                    {
                        Debug.Log($"收获失败（未播种或未成熟）: {target}");
                    }
                    break;
            }
        }

        private Vector2Int GetTargetTile()
        {
            Vector3 target = transform.position + (Vector3)_facing * interactionRange;
            return new Vector2Int(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y));
        }
    }
}
