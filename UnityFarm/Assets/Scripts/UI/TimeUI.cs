using System.Text;
using UnityEngine;
using UnityFarm.Core;

namespace UnityFarm.UI
{
    /// <summary>
    /// 简单 HUD：显示天数、剩余时间、背包、操作提示。
    /// 原型用 OnGUI，避免复杂 UI 搭建；正式版可换 uGUI/UI Toolkit。
    /// </summary>
    public class TimeUI : MonoBehaviour
    {
        private void OnGUI()
        {
            var tm = TimeManager.Instance;
            var gm = GameManager.Instance;

            string dayText = tm != null ? $"第 {tm.Day} 天" : "无时间系统";
            string timeText = tm != null ? $"剩余 {tm.SecondsRemaining:0} 秒" : "";
            GUI.Label(new Rect(10, 10, 300, 30), dayText + "   " + timeText);

            if (gm != null)
            {
                var sb = new StringBuilder("背包: ");
                if (gm.Inventory.items.Count == 0) sb.Append("空");
                foreach (var e in gm.Inventory.items)
                    sb.Append($"{e.itemName}×{e.count}   ");
                GUI.Label(new Rect(10, 40, 600, 30), sb.ToString());
            }

            GUI.Label(new Rect(10, 70, 700, 30),
                "操作: WASD移动 | 1锄头 2种子 3水壶 4收获 | 空格执行 | F5存档 F9读档");
        }
    }
}
