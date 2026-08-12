using UnityEngine;

namespace UnityFarm.Gameplay
{
    /// <summary>
    /// 作物数据定义（ScriptableObject）。
    /// 在 Project 窗口右键 → Create → UnityFarm → Crop Data 创建资产，
    /// 然后把资产拖到场景里 CropSystem 组件的 availableCrops 列表。
    /// </summary>
    [CreateAssetMenu(fileName = "NewCropData", menuName = "UnityFarm/Crop Data")]
    public class CropData : ScriptableObject
    {
        [Header("基本信息")]
        [Tooltip("作物名，同时作为作物 id")]
        public string cropName = "萝卜";

        [Tooltip("播种所需的种子在背包里的物品名")]
        public string seedItemName = "萝卜种子";

        [Tooltip("成熟后收获到背包里的物品名")]
        public string harvestItemName = "萝卜";

        [Header("生长")]
        [Tooltip("从播种到成熟所需天数")]
        public int daysToGrow = 3;

        [Header("经济")]
        [Tooltip("单个收获物的售价（金币）")]
        public int sellPrice = 10;
    }
}
