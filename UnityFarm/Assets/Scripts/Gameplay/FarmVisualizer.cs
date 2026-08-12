using System.Collections.Generic;
using UnityEngine;
using UnityFarm.Gameplay;

namespace UnityFarm.Gameplay
{
    /// <summary>
    /// 地块视觉同步：根据 CropSystem 的地块状态，在对应网格位置显示耕地/作物各阶段的 Sprite。
    /// 挂在一个 GameObject 上，引用占位素材（tilledSprite + cropStageSprites）。
    /// </summary>
    public class FarmVisualizer : MonoBehaviour
    {
        [Tooltip("耕地显示的 Sprite（Assets/Art/Tiles/tilled_soil.png）")]
        public Sprite tilledSprite;

        [Tooltip("作物生长三阶段的 Sprite（幼苗/成长/成熟）")]
        public Sprite[] cropStageSprites = new Sprite[3];

        private readonly Dictionary<Vector2Int, SpriteRenderer> _renderers = new Dictionary<Vector2Int, SpriteRenderer>();
        private Transform _root;

        private void Awake()
        {
            var rootGo = new GameObject("FarmVisual");
            rootGo.transform.SetParent(transform);
            _root = rootGo.transform;
        }

        private void LateUpdate()
        {
            var cropSystem = CropSystem.Instance;
            if (cropSystem == null) return;

            foreach (var tile in cropSystem.AllTiles())
            {
                var sr = GetOrCreateRenderer(tile.Pos);
                sr.sprite = ResolveSprite(tile);
            }
        }

        private Sprite ResolveSprite(FarmTile tile)
        {
            if (!tile.IsPlanted)
                return tilledSprite; // 纯耕地

            var data = CropSystem.Instance.GetCropData(tile.cropId);
            int daysToGrow = data != null ? Mathf.Max(1, data.daysToGrow) : 1;

            int stage;
            if (tile.growthDays <= 0) stage = 0;
            else if (tile.growthDays >= daysToGrow) stage = 2;
            else stage = 1;

            if (cropStageSprites == null || stage >= cropStageSprites.Length || cropStageSprites[stage] == null)
                return tilledSprite;
            return cropStageSprites[stage];
        }

        private SpriteRenderer GetOrCreateRenderer(Vector2Int pos)
        {
            if (_renderers.TryGetValue(pos, out var existing) && existing != null)
                return existing;

            var go = new GameObject($"Tile_{pos.x}_{pos.y}");
            go.transform.SetParent(_root);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1; // 显示在地面 Tilemap 之上
            _renderers[pos] = sr;
            return sr;
        }
    }
}
