# 免费像素素材来源清单

开发期用免费素材占位，避免自制美术卡进度。下面是**具体可下载的素材包**（含许可证），按用途分类。

## 一、具体素材包（推荐，直接下载）

### 农作物多阶段生长（最对口"作物生长"）
| 素材包 | 来源/许可证 | 内容 | 地址 |
|--------|------------|------|------|
| **Free Pixel Art Plants for Farm** | CraftPix · 免费可商用 | 小麦/胡萝卜/卷心菜/甜菜等多阶段生长 + 果树/浆果/蘑菇/收获品（32×32/64×64） | `https://craftpix.net/freebies/free-pixel-art-plants-for-farm/` |
| **[LPC] Farming tilesets** | OpenGameArt · CC-BY-SA 3.0 / GPL 3.0（需署名 Daniel Eddeland） | 小麦/玉米/番茄/土豆/胡萝卜等多阶段作物 + 草地/围栏 tileset + UI | `https://opengameart.org/content/lpc-farming-tilesets-magic-animations-and-ui-elements` |

### 完整农场 Kit（地面+建筑+农民+树，一站配齐）
| 素材包 | 来源/许可证 | 内容 | 地址 |
|--------|------------|------|------|
| **Free Retro Tiny Farm Kit** | CraftPix · 免费可商用 | 地形 tiles + 农场/谷仓 + 猪鸡牛 + 农民主角（带动画）+ 树/草/蘑菇（Pico-8 风） | `https://craftpix.net/freebies/free-retro-tiny-farm-kit-pixel-assets-for-pico-8/` |

### 地面/耕地 tileset
| 素材包 | 来源/许可证 | 说明 |
|--------|------------|------|
| **Top Down season/environment Tileset** | OpenGameArt · **CC0（无需署名）** | 农场/耕地 + 沙漠/冬季变体，20×20/格，较简陋适合原型 | `https://opengameart.org/content/top-down-seasonenvironment-tileset` |
| **[LPC] Farming tilesets** | CC-BY-SA 3.0 / GPL 3.0 | 含草地/围栏 tileset，质量高 | 见上 |

### 角色/农民 sprite
- **Free Retro Tiny Farm Kit**（CraftPix）内含农民主角（带动画）
- itch.io 付费/免费作者（见下）

### 装饰（树/建筑/围栏）
- **[LPC] Fruit Trees**（果树）：`https://opengameart.org/content/lpc-fruit-trees`
- **Kenney Tiny Farm**（CC0，农场建筑/作物/动物，风格偏 Q 版非严格像素）：`https://kenney.nl/assets/tiny-farm`
- **[LPC] Farming tilesets** 内含围栏 tileset

### itch.io 高质感作者（星露谷风格，部分付费）
> 以下链接本次未能逐一验证可访问（itch.io 网络不稳定），下载前请自行打开确认。
- **LimeZu – Serene Village**（村庄/农场 tileset + 角色 + 装饰）：`https://limezu.itch.io/serene-village`
- **Cup Nooble – Sprout Lands Basic Pack**（免费版农田/角色/作物/建筑）：`https://cupnooble.itch.io/sprout-lands-basic-pack`
- **Szadi Art – PIXEL FARM ASSET PACK**：`https://szadiart.itch.io/pixel-farm`

## 二、素材站总览（泛搜索用）

| 站点 | 地址 | 说明 |
|------|------|------|
| **Kenney** | `https://kenney.nl` | CC0，全品类 |
| **itch.io** | `https://itch.io/game-assets/free` | 搜 `pixel farm`、`stardew`、`tileset`、`crops` |
| **OpenGameArt** | `https://opengameart.org` | CC 开源素材 |
| **CraftPix** | `https://craftpix.net/freebies` | 免费农场/农作物包 |
| **GameArtGuppy** | `https://www.gameartguppy.com` | 免费像素角色/道具 |
| **Lospec** | `https://lospec.com` | 像素画教程 + 调色板 |

## 三、许可证速查（用之前看清楚）

| 许可证 | 含义 | 本项目适用 |
|--------|------|-----------|
| **CC0** | 完全自由，无需署名 | ✅ 最省心 |
| **CC-BY** | 需署名作者 | ✅ 记得留署名 |
| **CC-BY-SA** | 需署名 + 衍生作品相同许可分享 | ⚠️ 注意分享义务 |
| **免费可商用（CraftPix 等）** | 可商用，禁止转售素材本身 | ✅ 直接用 |

## 四、使用注意

1. 导入后把 Sprite 的 `Pixels Per Unit` 统一（建议 16 或 32，与素材格大小一致），`Filter Mode = Point`，否则像素会糊
2. 替换现有占位素材：同名覆盖 `Assets/Art/` 下 PNG 即可，或重跑 `UnityFarm → Generate Default Assets`
3. 先占位后替换：原型阶段用免费素材跑通玩法，美术统一风格留到后期
