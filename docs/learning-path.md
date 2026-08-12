# 星露谷风格游戏 · 学习路线（零基础）

> 技术栈：团结引擎（Tuanjie，基于 Unity 2022.3 LTS）+ C# + Visual Studio
> 目标：做出一个"耕地→播种→浇水→生长→收获"的最小农场循环游戏

---

## 0. 学习顺序总览（重要）

按下面顺序学，**边学边在编辑器里动手验证**，不要只看视频：

1. **C# 基础**（约 1~2 周）——先脱离 Unity，纯学语言语法
2. **Unity 核心概念**（约 1 周）——GameObject/Component/生命周期/Prefab
3. **2D 专项**（约 1 周）——Sprite、Tilemap、相机、2D 物理
4. **跟做 Ruby's Adventure**（约 1 周）——官方 2D 教程，把前 3 步串起来
5. **开始农场原型**——进入本项目的实际开发

每个阶段学完，用对应的"练习"验证，再进入下一阶段。

---

## 1. C# 基础（先学语言）

**目标**：能看懂和写出面向对象的 C# 代码，理解"类/对象/方法/字段"。

| 主题 | 要掌握的点 | 对应练习 |
|------|-----------|---------|
| 变量与类型 | `int/float/double/bool/string`、声明与赋值、类型转换 | 写个计算器小程序 |
| 运算符 | 算术、比较、逻辑（`&&` `\|\|` `!`） | 判断闰年 |
| 流程控制 | `if/else`、`switch`、`for`、`while`、`foreach` | 打印九九乘法表 |
| 方法 | 定义、参数、返回值、重载 | 把重复代码抽成方法 |
| 类与对象 | `class`、`new`、字段、属性（`get/set`） | 定义一个 `Crop`（作物）类 |
| 继承与多态 | `class A : B`、`virtual/override`、`interface` | `Crop` 派生 `Turnip`（萝卜） |
| 事件与委托 | `Action`/`event`、订阅与触发 | 作物成熟时触发事件 |
| 集合 | `List<T>`、`Dictionary<K,V>`、遍历 | 用 `List<Crop>` 管理一块地 |

**命名规范（现在就养成习惯）**：类名 `PascalCase`、方法名 `PascalCase`、字段/局部变量 `camelCase`、私有字段前缀 `_`（如 `_growthDays`）。

---

## 2. Unity 核心概念

**目标**：理解 Unity 的"组件式"设计，能独立搭出一个会动的物体。

| 主题 | 要掌握的点 |
|------|-----------|
| GameObject 与 Component | 场景里一切都是 GameObject，功能靠挂 Component（脚本也是 Component） |
| Transform | `position`/`rotation`/`scale`、父子层级 |
| MonoBehaviour 生命周期 | `Awake` → `Start` → `Update` → `OnDestroy` 的执行时机 |
| Prefab | 把做好的物体存成预制体，反复复用（作物、玩家都要用） |
| Scene 与序列化 | 场景文件、Inspector 里给脚本字段赋值、`[SerializeField]` 私有字段 |
| 输入 | 旧 `Input` 类 vs 新 **Input System**（本项目用新 Input System） |

**练习**：新建脚本 `Player.cs`，挂到角色上，用键盘 `WASD` 让一个方块在场景里移动。

---

## 3. 2D 专项（本项目核心）

**目标**：掌握 2D 游戏三大件——精灵、瓦片地图、物理碰撞。

| 主题 | 要掌握的点 | 注意 |
|------|-----------|------|
| Sprite / SpriteRenderer | 导入图片 → 设成 Sprite、`Pixels Per Unit`、`Filter Mode: Point`（像素风关键） | 像素图一定用 Point 过滤，否则会糊 |
| Tilemap | Grid + Tilemap + Tile Palette，用瓦片刷出农场地面 | 星露谷式地图全靠它 |
| 相机 | 正交投影（Orthographic）、相机跟随角色 | 2D 用正交相机 |
| Collider 2D / Rigidbody 2D | 碰撞体、刚体、`OnTriggerEnter2D`/`OnCollisionEnter2D` | 角色撞墙、走进耕地用触发器 |
| 2D 动画（可选） | Animator + Animation Clip 做走路动画 | 先用静态图片，后补动画 |

**练习**：用 Tilemap 铺一块 20×15 的地面 + 一圈围墙（碰撞体），让角色只能在墙内走动。

---

## 4. 学习资源清单

| 资源 | 地址/渠道 | 说明 |
|------|----------|------|
| **Ruby's Adventure**（强烈推荐先做） | Unity Learn 官网搜索 "Ruby's Adventure" | 官方免费 2D 教程，把 C#+2D 串起来，跟着做完基本就入门了 |
| Unity Learn | `https://learn.unity.com` | 官方免费课程，按主题搜 |
| Brackeys 系列 | B 站搜索 "Brackeys 中文" 或 YouTube | 经典入门系列，中文搬运版很多 |
| 团结引擎中文手册 | `https://docs.unity.cn/cn/2022.3/` | 团结引擎官方文档，查 API 用 |
| 2D 像素教程 | B 站搜索 "Unity 2D 游戏开发" | 大量中文实战教程，挑"星露谷/农场/2D 像素"关键词 |

**B 站建议**：优先看带"完整项目""从零做一个 2D 游戏"字样的系列，边看边抄代码。

---

## 5. 阶段验收练习（里程碑）

学完上面内容后，独立完成这个小 demo（不照抄）：

- 用 **Tilemap** 搭一个场景（地面 + 不可穿越的围墙/障碍）
- 用 **Input System** 让一个角色（Sprite）用 `WASD` 移动
- 角色撞到围墙会被 **Collider 2D** 挡住，走不出去

**验收标准**：能自己解释每一步在做什么（GameObject、Component、Tilemap、碰撞），而不仅是"能跑"。

完成这个 demo 后，就具备了进入本项目原型开发的能力，进入下一阶段：搭建 UnityFarm 项目。
