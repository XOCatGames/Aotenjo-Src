# 牌材质与材质组

[目录](README.md) · [English](../en/materials.md) · [宝石完整模组](../../example/mods/ex3_tile_material)

牌材质（TileMaterial）带玩法效果；[材质包](textures.md)只替换图片。完整示例注册红宝石（加20符、参与关末效果时加2金币）和蓝宝石（乘2番），再把它们加入 `tutorial_gemstones` 材质组。

## 三种不同的 ID

| 用途 | 红宝石示例 |
| --- | --- |
| `LuaTileMaterialBuilder.Create(nameKey)` | `tutorial_gems:ruby` |
| `material:GetRegName()` | `tutorial_gems:ruby_material`（自动补后缀） |
| `TileMaterial.GetMaterial(id)` | 上面两种都可，必须已注册 |
| PNG 文件 | `texture/tile_material/ruby.png` |
| 图片注册名 | `tile_material:tutorial_gems:ruby` |
| 名称键 | `tile_tutorial_gems:ruby_material_name` |
| 短名称键 | `tile_tutorial_gems:ruby_material_name_short` |
| 描述键 | `tile_tutorial_gems:ruby_material_description` |

不要在 `Create` 参数中再加 `_material`。当前图片解析会移除注册名里所有 `_material` 文本，因此自己的命名空间也不要包含这个片段；示例使用 `tutorial_gems`。

## LuaTileMaterialBuilder 完整表

`p=Player`、`m=当前材质`、`E=List<Effect>`、`AE=List<IAnimationEffect>`。

| 方法 | 参数或回调签名 | 用途 |
| --- | --- | --- |
| `Create` | `nameKey: string` | 静态入口 |
| `WithRarity` | `Rarity` | 默认 COMMON |
| `WithData` | `(key: string, value: string)` | 初始状态；每次 Build 复制数据容器 |
| `WithDebuff` | `(m) → bool` | 是否视为减益 |
| `WithDescription` | `(loc, p, m) → string` | 注意 localizer 在第一个参数 |
| `OnScoringEffect` | `(p, perm, tile, E, m)` | 材质计分 |
| `OnUnusedEffect` | `(p, perm, E, m)` | 未使用效果；没有 tile 参数 |
| `OnDerivedTileUnusedEffect` | `(p, perm, E, scoringTile, onEffectTile, m)` | 关联的计分牌与持有效果牌 |
| `OnRoundEndEffect` | `(p, perm, AE, tile, m)` | 关末动画效果；不是所有牌库牌都会经过此回调 |
| `OnDiscardEffect` | `(p, perm, AE, tile, withForce, isClone, m)` | 弃牌；参数顺序与藏品回调不同 |
| `OnSubscribe` / `OnUnsubscribe` | `(p, m)` | 订阅/解绑 |
| `Build()` | 返回 `LuaTileMaterial` | 只创建 |
| `BuildAndRegister()` | 返回 `LuaTileMaterial` | 把构建工厂加入 MaterialProviders |

材质支持 `GetDataOrDefault` / `SetData` 的字符串状态。`Copy()` 会通过已注册的工厂创建同类材质并复制数据，因此必须先注册。不要把同一个可变实例赋给多张牌；分别 `GetMaterial(...)` 或 `.Copy()`。这不等于保证 Lua 委托在所有发行版的存档反序列化中恢复；含自定义材质的续局必须单独验证。

## 材质组决定在哪里能抽到

| LuaMaterialSetBuilder 方法 | 含义 |
| --- | --- |
| `Create(regName)` | 创建材质组；自定义 ID 内部为 -5，不用数字区分自定义组 |
| `WithAvailableMaterials(string[])` | 替换材质名称列表 |
| `AddMaterial(string)` | 追加一个已注册材质 |
| `OnGenerateCommon(function(set) → LotteryPool<TileMaterial>)` | 覆盖普通池；默认筛选 COMMON |
| `OnGenerateRare(function(set) → LotteryPool<TileMaterial>)` | 覆盖稀有池；默认筛选 RARE 与 EPIC |
| `OnSubscribe(function(p, set))` / `OnUnsubscribe(...)` | 组选中时的玩家绑定生命周期 |
| `Build()` / `BuildAndRegister()` | 只创建 / 加入 MaterialSets；组名重复会跳过注册 |

默认池已经适合示例。要调整权重，可以在 `OnGenerateCommon` 回调里创建 `CS.LotteryPool(CS.Aotenjo.TileMaterial)()`，再 `pool:Add(material, positiveWeight)`，最后返回 pool。`LotteryPool` 是全局 C# 类型。务必同时提供有候选的普通池和稀有池，避免商店抽不到内容。

材质组名称使用 `material_set_tutorial_gemstones_name`。新局选择该组才能自然抽到示例材质；仅注册不会自动把现有牌变成红宝石。

## 确定性测试

开启测试控制台，进入新局后执行 `setHand 222m333p`，再 `setMat 0-2 tutorial_gems:ruby`，打出前三张；每张红宝石额外加20符。区间 `0-2` 包含0、1、2。蓝宝石用 `setMat 0 tutorial_gems:sapphire` 检查乘2番。关末再核对实际参与关末流程的红宝石各加2金币。

在有真实 `tile` 与 `player` 的效果执行回调中，应用方式是 `tile:SetMaterial(A.TileMaterial.GetMaterial("tutorial_gems:ruby"), player)`，不要直接写 `tile.properties.material`。检查复制牌数据不串联、保存退出再进入后的行为，以及取消安装后不要继续依赖该材质的测试存档。

源码：[材质 Builder](../../src/API/TileProperties/LuaTileMaterialBuilder.cs)、[材质实例](../../src/API/TileProperties/LuaTileMaterial.cs)、[材质组 Builder](../../src/API/TileProperties/LuaMaterialSetBuilder.cs)。
