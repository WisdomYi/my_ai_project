# UnityFarm 原型 · 使用说明

星露谷风格 2D 像素农场游戏的核心种植循环原型（团结引擎 / Unity 2022.3 LTS）。

## 快速开始

1. 用团结 Hub 打开本项目（选择 `UnityFarm` 目录）
2. 顶部菜单点 **UnityFarm → Build Prototype Scene**，一键生成原型场景
3. 打开 `Assets/Scenes/MainFarm.unity`，点 **Play**
4. 用键盘操作跑通种植循环：

| 按键 | 作用 |
|------|------|
| `WASD` / 方向键 | 移动角色 |
| `1` `2` `3` `4` | 切换工具（锄头 / 种子 / 水壶 / 收获） |
| `空格` | 对角色前方一格执行当前工具 |
| `N` | 睡觉（结束当天，作物生长结算） |
| `F5` / `F9` | 存档 / 读档 |

**完整种植循环**：`1`+空格锄地 → `2`+空格播种 → `3`+空格浇水 → 按 `N` 睡几觉（每天记得浇水）→ `4`+空格收获。

## 场景搭建方式

- **自动**（推荐）：菜单 `UnityFarm → Build Prototype Scene`，脚本程序化生成 Tilemap 地面 + 玩家 + 相机 + 各系统。
- **手动**：Window → 2D → Tile Palette，用 `Assets/Art/Tiles/` 下的素材手动刷瓦片（适合正式开发期）。

## 替换占位素材

原型用**程序生成的纯色瓦片**占位（`SceneBuilder.CreatePlaceholderSprite`）。换正式像素素材的方法：

1. 把 tileset 图片拖进 `Assets/Art/Tiles/`，在 Inspector 设置 `Filter Mode = Point`、`Pixels Per Unit = 16`
2. 切分后把瓦片拖进 Tile Palette，重新刷地面
3. 或直接改 `SceneBuilder.cs` 里的占位 Sprite 为你的素材

## 目录结构

```
UnityFarm/
├─ Assets/
│  ├─ Scripts/
│  │  ├─ Core/         GameManager、TimeManager、SaveSystem
│  │  ├─ Gameplay/     CropSystem、Crop、CropData、Inventory、Player、PlantingTool
│  │  ├─ UI/           TimeUI（HUD）
│  │  └─ Editor/       SceneBuilder（场景生成）、FarmingLoopValidator（逻辑验证）
│  ├─ Scenes/          MainFarm.unity（自动生成）
│  ├─ Art/             美术素材（待替换）
│  ├─ Prefabs/         预制体
│  └─ Data/            CropData 等 ScriptableObject 资产
└─ Docs/               设计文档（gdd.md、coding-conventions.md、asset-sources.md）
```

## 核心系统职责

| 系统 | 职责 |
|------|------|
| `TimeManager` | 游戏内时间、天数、睡觉结算，触发 `OnDayEnd` |
| `CropSystem` | 地块管理、锄地/播种/浇水/收获、按天生长 |
| `Inventory` | 背包（物品名 → 数量） |
| `SaveSystem` | JSON 存档/读档（天数、地块、背包） |
| `CropData` | ScriptableObject 作物数据（名称、生长天数、售价） |

## 逻辑自检

命令行验证核心循环（无需打开编辑器）：

```bash
Tuanjie.exe -batchmode -projectPath UnityFarm -executeMethod UnityFarm.EditorTools.FarmingLoopValidator.Validate -quit
```

输出 `=== 种植循环验证全部通过 ===` 即表示核心逻辑无误。
