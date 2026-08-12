#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityFarm.Core;
using UnityFarm.Gameplay;
using UnityFarm.UI;

namespace UnityFarm.EditorTools
{
    /// <summary>
    /// 一键生成原型场景：Tilemap 地面 + 玩家 + 管理器 + 相机 + HUD，并创建 CropData 资产。
    /// 菜单：UnityFarm → Build Prototype Scene
    /// 生成后打开 Assets/Scenes/MainFarm.unity 点 Play 即可跑通种植循环。
    /// </summary>
    public static class SceneBuilder
    {
        [MenuItem("UnityFarm/Build Prototype Scene")]
        public static void Build()
        {
            // 1. 新场景
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. 正交相机
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            camGo.transform.position = new Vector3(0, 0, -10f);

            // 3. Tilemap 地面（程序生成的占位瓦片，正式开发换素材）
            CreateGroundTilemap();

            // 4. 玩家
            var player = CreatePlayer();

            // 5. 管理器（单例系统）
            CreateManager("TimeManager");
            var cropSys = CreateManager("CropSystem");
            CreateManager("SaveSystem");
            CreateManager("GameManager");

            // 6. HUD
            var hud = new GameObject("HUD");
            hud.AddComponent<TimeUI>();

            // 7. 创建并配置 CropData 资产
            var cropData = CreateCropData();
            cropSys.GetComponent<CropSystem>().availableCrops.Add(cropData);
            player.GetComponent<PlantingTool>().selectedCrop = cropData;

            // 8. 保存场景
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainFarm.unity");
            Debug.Log("原型场景已生成：Assets/Scenes/MainFarm.unity（按 Play 运行，WASD 移动、空格操作）");
        }

        private static GameObject CreateManager(string name)
        {
            var go = new GameObject(name);
            switch (name)
            {
                case "TimeManager": go.AddComponent<TimeManager>(); break;
                case "CropSystem": go.AddComponent<CropSystem>(); break;
                case "SaveSystem": go.AddComponent<SaveSystem>(); break;
                case "GameManager": go.AddComponent<GameManager>(); break;
            }
            return go;
        }

        private static void CreateGroundTilemap()
        {
            var gridGo = new GameObject("Grid");
            gridGo.AddComponent<Grid>();

            var tilemapGo = new GameObject("Ground");
            tilemapGo.transform.SetParent(gridGo.transform);
            var tilemap = tilemapGo.AddComponent<Tilemap>();
            tilemapGo.AddComponent<TilemapRenderer>();

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = CreatePlaceholderSprite("grass", new Color(0.32f, 0.6f, 0.32f));

            for (int x = -10; x <= 10; x++)
                for (int y = -8; y <= 8; y++)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }

        private static GameObject CreatePlayer()
        {
            var go = new GameObject("Player");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite("player", Color.white);
            sr.sortingOrder = 1; // 玩家绘制在地面之上

            go.AddComponent<PlayerController>();
            go.AddComponent<PlantingTool>();

            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            return go;
        }

        private static CropData CreateCropData()
        {
            const string path = "Assets/Data/CarrotData.asset";
            var existing = AssetDatabase.LoadAssetAtPath<CropData>(path);
            if (existing != null) return existing;

            var data = ScriptableObject.CreateInstance<CropData>();
            data.cropName = "萝卜";
            data.seedItemName = "萝卜种子";
            data.harvestItemName = "萝卜";
            data.daysToGrow = 3;
            data.sellPrice = 10;

            Directory.CreateDirectory("Assets/Data");
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            return data;
        }

        private static Sprite CreatePlaceholderSprite(string name, Color color)
        {
            var tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            var pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();

            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
            sprite.name = name;
            return sprite;
        }
    }
}
#endif
