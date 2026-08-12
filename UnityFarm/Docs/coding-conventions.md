# 命名与代码规范（UnityFarm）

本项目统一遵循以下约定，从第一个脚本开始就保持一致。

## C# 命名

| 类型 | 规则 | 示例 |
|------|------|------|
| 类 / 结构体 / 枚举 | PascalCase | `PlayerController`、`CropGrowthStage` |
| 方法 | PascalCase | `PlantSeed()`、`Harvest()` |
| 公共属性 | PascalCase | `GrowthDays` |
| 私有字段 | `_camelCase` | `_growthDays`、`_isWatered` |
| 局部变量 / 参数 | camelCase | `targetTile` |
| 接口 | `I` 前缀 | `IInteractable` |
| 常量 | PascalCase | `MaxInventorySize` |
| 布尔字段 | `is/has/can` 前缀 | `isWatered`、`hasSeed` |

## Unity 资源命名

| 资源 | 规则 | 位置示例 |
|------|------|---------|
| 场景 | PascalCase，`.unity` | `Assets/Scenes/MainFarm.unity` |
| 脚本 | 文件名 = 类名（一个文件一个类） | `Assets/Scripts/Gameplay/Crop.cs` |
| Prefab | PascalCase，`.prefab` | `Assets/Prefabs/CarrotPlant.prefab` |
| 贴图素材 | 小写蛇形，按类型分子目录 | `Assets/Art/Tiles/grass.png`、`Assets/Art/Sprites/player.png` |
| ScriptableObject 资产 | 与类名一致，`.asset` | `Assets/Data/CarrotData.asset` |

## 目录职责

- `Scripts/Core/`：全局系统（`GameManager`、`TimeManager`、`SaveSystem`）
- `Scripts/Gameplay/`：玩法逻辑（`Crop`、`Planting`、`Player`）
- `Scripts/UI/`：界面（`InventoryUI`、`HUD`）
- `Scripts/Data/`：数据定义（ScriptableObject 类，如 `CropData`）
- `Scenes/`：场景文件
- `Art/`：美术素材（`Tiles/`、`Sprites/`、`Audio/` 子目录）
- `Prefabs/`：预制体
- `Data/`：ScriptableObject 资产实例

## 其他约定

- 一个脚本一个类，类名与文件名一致
- 需要在 Inspector 配置的字段用 `[SerializeField] private`，避免直接 `public`
- 注释用中文，说明"为什么"而非"是什么"
- 命名空间建议用 `UnityFarm.Core`、`UnityFarm.Gameplay` 等（项目规模变大后再引入）
