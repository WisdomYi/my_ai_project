using UnityEngine;

namespace UnityFarm.Gameplay
{
    /// <summary>
    /// 角色移动：WASD / 方向键，直接移动位置（原型从简，不用 Rigidbody）。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Tooltip("移动速度（单位/秒）")]
        public float moveSpeed = 5f;

        private void Update()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            Vector2 dir = new Vector2(x, y).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }
    }
}
