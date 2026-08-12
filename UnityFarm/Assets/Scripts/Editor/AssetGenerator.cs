#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityFarm.EditorTools
{
    /// <summary>
    /// 程序化生成默认占位素材（16x16 像素 PNG），保存到 Assets/Art/ 并设置像素导入参数。
    /// 菜单：UnityFarm → Generate Default Assets
    /// 正式开发时用真实像素素材替换这些 PNG 即可。
    /// </summary>
    public static class AssetGenerator
    {
        [MenuItem("UnityFarm/Generate Default Assets")]
        public static void Generate()
        {
            SaveSprite("Assets/Art/Tiles/grass.png", GenerateGrass());
            SaveSprite("Assets/Art/Tiles/tilled_soil.png", GenerateTilledSoil());
            SaveSprite("Assets/Art/Sprites/player.png", FromRows(PLAYER, PLAYER_PALETTE));
            SaveSprite("Assets/Art/Sprites/crop_0.png", FromRows(CROP_0, CROP_PALETTE));
            SaveSprite("Assets/Art/Sprites/crop_1.png", FromRows(CROP_1, CROP_PALETTE));
            SaveSprite("Assets/Art/Sprites/crop_2.png", FromRows(CROP_2, CROP_PALETTE));
            AssetDatabase.SaveAssets();
            Debug.Log("默认素材已生成到 Assets/Art/");
        }

        // ---- 保存并设置像素导入参数 ----
        private static void SaveSprite(string path, Texture2D tex)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 16;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }

        // ---- 草地：绿色底 + 确定性噪点 ----
        private static Texture2D GenerateGrass()
        {
            var rng = new System.Random(12345);
            var baseC = new Color32(82, 153, 82, 255);
            var darkC = new Color32(64, 128, 64, 255);
            var lightC = new Color32(100, 170, 100, 255);
            return GenerateNoise(baseC, darkC, lightC, rng);
        }

        // ---- 耕地：棕色底 + 水平垄沟条纹 ----
        private static Texture2D GenerateTilledSoil()
        {
            var tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            var px = new Color32[16 * 16];
            var baseC = new Color32(140, 102, 64, 255);
            var furrowC = new Color32(120, 85, 50, 255);
            for (int y = 0; y < 16; y++)
            {
                bool furrow = (y % 4) == 3; // 每 4 行一条垄沟
                var c = furrow ? furrowC : baseC;
                for (int x = 0; x < 16; x++) px[y * 16 + x] = c;
            }
            tex.SetPixels32(px);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateNoise(Color32 baseC, Color32 darkC, Color32 lightC, System.Random rng)
        {
            var tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            var px = new Color32[16 * 16];
            for (int i = 0; i < px.Length; i++)
            {
                int r = rng.Next(10);
                px[i] = r < 2 ? darkC : (r < 4 ? lightC : baseC);
            }
            tex.SetPixels32(px);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return tex;
        }

        // ---- 字符画解析：'.' 为透明，其余字符映射调色板 ----
        private static Texture2D FromRows(string[] rows, Dictionary<char, Color32> palette)
        {
            int h = rows.Length;
            int w = rows[0].Length;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
            {
                var row = rows[h - 1 - y]; // rows[0] 是顶部，像素 y=0 是底部，需反转
                for (int x = 0; x < w; x++)
                {
                    char c = row[x];
                    px[y * w + x] = palette.TryGetValue(c, out var col) ? col : new Color32(0, 0, 0, 0);
                }
            }
            tex.SetPixels32(px);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return tex;
        }

        // ---- 调色板 ----
        static readonly Dictionary<char, Color32> PLAYER_PALETTE = new Dictionary<char, Color32>
        {
            { 'H', new Color32(90, 60, 30, 255) },    // 头发
            { 'F', new Color32(240, 200, 160, 255) }, // 脸
            { 'B', new Color32(60, 90, 180, 255) },   // 衣服
            { 'L', new Color32(40, 50, 90, 255) },    // 腿
        };

        static readonly Dictionary<char, Color32> CROP_PALETTE = new Dictionary<char, Color32>
        {
            { 'g', new Color32(60, 160, 60, 255) },   // 茎叶
            { 's', new Color32(140, 102, 64, 255) },  // 土壤
            { 'o', new Color32(230, 120, 30, 255) },  // 果实
        };

        // ---- 玩家（16x16，简单像素小人）----
        static readonly string[] PLAYER =
        {
            "................",
            "................",
            "....HHHHHH......",
            "...HHHHHHHH.....",
            "...HHHHHHHH.....",
            "...FFFFFFFF.....",
            "...FFFFFFFF.....",
            "...FFFFFFFF.....",
            "...BBBBBBBB.....",
            "...BBBBBBBB.....",
            "...BBBBBBBB.....",
            "...BBBBBBBB.....",
            "....BBBBBB......",
            "....LLLLLL......",
            "....LLLLLL......",
            "....LLLLLL......",
        };

        // ---- 作物阶段 0：刚发芽 ----
        static readonly string[] CROP_0 =
        {
            "................",
            "................",
            "................",
            "................",
            "................",
            "................",
            "................",
            "................",
            "................",
            "........g.......",
            "........g.......",
            "................",
            "ssssssssssssssss",
            "ssssssssssssssss",
            "ssssssssssssssss",
            "ssssssssssssssss",
        };

        // ---- 作物阶段 1：成长中的植株 ----
        static readonly string[] CROP_1 =
        {
            "................",
            "................",
            "................",
            "................",
            ".......g........",
            ".......g........",
            "......ggg.......",
            ".......g........",
            ".......g........",
            "......ggg.......",
            ".......g........",
            ".......g........",
            "ssssssssssssssss",
            "ssssssssssssssss",
            "ssssssssssssssss",
            "ssssssssssssssss",
        };

        // ---- 作物阶段 2：成熟（带果实）----
        static readonly string[] CROP_2 =
        {
            "................",
            "................",
            ".......g........",
            "......ggg.......",
            ".......g........",
            "......ggg.......",
            ".......g........",
            ".......g........",
            "......ggg.......",
            ".......g........",
            "..o...g...o.....",
            ".o....g....o....",
            "ssssssssssssssss",
            "ssssssssssssssss",
            "ssssssssssssssss",
            "ssssssssssssssss",
        };
    }
}
#endif
