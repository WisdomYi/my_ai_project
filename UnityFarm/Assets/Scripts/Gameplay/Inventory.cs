using System;
using System.Collections.Generic;

namespace UnityFarm.Gameplay
{
    /// <summary>
    /// 背包：以"物品名 → 数量"管理物品。
    /// 用可序列化的 List 存储，便于 JsonUtility 直接存档。
    /// </summary>
    [Serializable]
    public class Inventory
    {
        [Serializable]
        public class ItemEntry
        {
            public string itemName;
            public int count;
        }

        public List<ItemEntry> items = new List<ItemEntry>();

        public void Add(string itemName, int count = 1)
        {
            if (string.IsNullOrEmpty(itemName) || count <= 0) return;
            var entry = items.Find(e => e.itemName == itemName);
            if (entry == null)
            {
                entry = new ItemEntry { itemName = itemName, count = 0 };
                items.Add(entry);
            }
            entry.count += count;
        }

        public bool Remove(string itemName, int count = 1)
        {
            var entry = items.Find(e => e.itemName == itemName);
            if (entry == null || entry.count < count) return false;
            entry.count -= count;
            if (entry.count <= 0) items.Remove(entry);
            return true;
        }

        public int Count(string itemName)
        {
            var entry = items.Find(e => e.itemName == itemName);
            return entry == null ? 0 : entry.count;
        }
    }
}
