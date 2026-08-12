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
    /// 一键生成原型场景：Tilemap 地面 + 玩家 + 管理器 + 地块视觉 + 相机 + HUD。
    /// 菜单：UnityFarm → Build Prototype Scene
    /// 生成后打开 Assets/Scenes/MainFarm.unity 点 Play 即可跑通种植循环。
    /// </summary>
    public static class SceneBuilder
    {
        [MenuItem("UnityFarm/Build Prototype Scene")]
        public static void Build()
        {
            // 素材缺失时先生成
            if (!File.Exists("Assets/Art/Tiles/grass.png"))
                AssetGenerator.Generate();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 正交相机
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            camGo.transform.position = new Vector3(0, 0, -10f);

            // 地面（草地 Tilemap）
            CreateGroundTilemap();

            // 玩家
            var player = CreatePlayer();

            // 管理器（单例系统）
            CreateManager("TimeManager");
            var cropSys = CreateManager("CropSystem");
            CreateManager("SaveSystem");
            CreateManager("GameManager");

            // 地块视觉（耕地/作物各阶段显示）
            CreateFarmVisualizer();

            // HUD
            var hud = new GameObject("HUD");
            hud.AddComponent<TimeUI>();

            // CropData 资产
            var cropData = CreateCropData();
            cropSys.GetComponent<CropSystem>().availableCrops.Add(cropData);
            player.GetComponent<PlantingTool>().selectedCrop = cropData;

            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainFarm.unity");
            Debug.Log("原型场景已生成：Assets/Scenes/MainFarm.unity（按 Play 运行，WASD 移动、空格操作）");
        }

        private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

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
            tile.sprite = LoadSprite("Assets/Art/Tiles/grass.png");

            for (int x = -10; x <= 10; x++)
                for (int y = -8; y <= 8; y++)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }

        private static void CreateFarmVisualizer()
        {
            var go = new GameObject("FarmVisualizer");
            var vis = go.AddComponent<FarmVisualizer>();
            vis.tilledSprite = LoadSprite("Assets/Art/Tiles/tilled_soil.png");
            vis.cropStageSprites = new[]
            {
                LoadSprite("Assets/Art/Sprites/crop_0.png"),
                LoadSprite("Assets/Art/Sprites/crop_1.png"),
                LoadSprite("Assets/Art/Sprites/crop_2.png"),
            };
        }

        private static GameObject CreatePlayer()
        {
            var go = new GameObject("Player");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = LoadSprite("Assets/Art/Sprites/player.png");
            sr.sortingOrder = 2; // 玩家绘制在地块之上

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
    }
}
#endif
