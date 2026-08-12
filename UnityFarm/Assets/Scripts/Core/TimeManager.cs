using System;
using UnityEngine;

namespace UnityFarm.Core
{
    /// <summary>
    /// 游戏内时间：一天按真实秒数流逝，结束时触发 OnDayEnd（作物生长结算在 CropSystem 监听）。
    /// 单例，场景中需存在一个挂了本脚本的 GameObject。
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [Tooltip("一天对应的真实秒数（原型用短天方便测试）")]
        public float dayLengthSeconds = 60f;

        public int Day { get; private set; } = 1;
        public float SecondsRemaining { get; private set; }

        /// <summary>一天结束时触发（参数为结束后进入的新天数）</summary>
        public event Action<int> OnDayEnd;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            SecondsRemaining = dayLengthSeconds;
        }

        private void Update()
        {
            SecondsRemaining -= Time.deltaTime;
            if (SecondsRemaining <= 0f)
                EndDay();
        }

        /// <summary>立即结束当天（相当于睡觉），天数 +1 并触发结算</summary>
        public void EndDay()
        {
            Day++;
            SecondsRemaining = dayLengthSeconds;
            OnDayEnd?.Invoke(Day);
        }

        /// <summary>读档时设置天数（不影响当天剩余时间）</summary>
        public void SetDay(int day)
        {
            Day = Mathf.Max(1, day);
        }
    }
}
