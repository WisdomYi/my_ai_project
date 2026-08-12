using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityFarm.Gameplay;

namespace UnityFarm.Core
{
    /// <summary>
    /// 存档数据（与 SaveSystem 一起用 JsonUtility 序列化）。
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int day = 1;
        public List<FarmTile> tiles = new List<FarmTile>();
        public List<Inventory.ItemEntry> inventory = new List<Inventory.ItemEntry>();
    }

    /// <summary>
    /// JSON 存档系统：保存/读取天数、地块、背包。
    /// 原型用快捷键 F5 存档、F9 读档（见 GameManager）。
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private string SavePath => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Save()
        {
            var data = new SaveData
            {
                day = TimeManager.Instance != null ? TimeManager.Instance.Day : 1,
                tiles = CropSystem.Instance != null ? CropSystem.Instance.SerializeTiles() : new List<FarmTile>(),
                inventory = GameManager.Instance != null ? GameManager.Instance.Inventory.items : new List<Inventory.ItemEntry>()
            };

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"存档已保存到 {SavePath}");
        }

        public void Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("没有找到存档文件");
                return;
            }

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            CropSystem.Instance?.DeserializeTiles(data.tiles);
            TimeManager.Instance?.SetDay(data.day);
            if (GameManager.Instance != null)
                GameManager.Instance.Inventory.items = data.inventory;
            Debug.Log("存档已读取");
        }
    }
}
