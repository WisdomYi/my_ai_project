using UnityEngine;
using UnityFarm.Gameplay;

namespace UnityFarm.Core
{
    /// <summary>
    /// 入口管理器：持有全局背包，初始化初始种子，处理存档/读档快捷键。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Tooltip("初始给玩家的种子物品名")]
        public string startingSeedItem = "萝卜种子";

        [Tooltip("初始种子数量")]
        public int startingSeedCount = 5;

        public Inventory Inventory { get; private set; } = new Inventory();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (Inventory.Count(startingSeedItem) == 0)
                Inventory.Add(startingSeedItem, startingSeedCount);
        }

        private void Update()
        {
            // N 键睡觉（结束当天，触发生长结算）；F5 存档，F9 读档
            if (Input.GetKeyDown(KeyCode.N)) TimeManager.Instance?.EndDay();
            if (Input.GetKeyDown(KeyCode.F5)) SaveSystem.Instance?.Save();
            if (Input.GetKeyDown(KeyCode.F9)) SaveSystem.Instance?.Load();
        }
    }
}
