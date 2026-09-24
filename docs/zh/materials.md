# 牌材质与材质组

[目录](README.md) · [English](../en/materials.md) · [宝石完整模组](../../example/mods/ex3_tile_material)

这一篇我们来添加两种新的牌材质：红宝石计分时加20符，参与关末效果时再获得2金币；蓝宝石计分时乘2番。然后把它们放进一个新的材质组，让玩家开局时可以选择。

这里的牌材质，也就是 `TileMaterial`，可以带有玩法效果。如果只是想给现有的牌或遗物换张图片，可以直接看[材质包教程](textures.md)。

## 1. 给新材质取一个名字

先打开宝石示例中的 `script/init.lua`。红宝石从 `LuaTileMaterialBuilder.Create("tutorial_gems:ruby")` 开始创建，其中 `tutorial_gems` 是模组前缀，`ruby` 是我们给材质取的名字。

同一个材质在代码、图片和语言文件里，会用到下面这些名称。第一次看可能有些相似，可以先照着红宝石的例子填写：

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

`_material` 后缀由游戏自动添加，传给 `Create` 时不用再写一次。当前图片解析还会移除注册名里所有 `_material` 文本，所以模组前缀也要避开这一段文字。这就是示例使用 `tutorial_gems` 的原因。

## 2. 给材质添加效果

和创建遗物一样，我们可以接着设置稀有度、计分效果和关末效果，最后调用 `BuildAndRegister()` 完成注册。红宝石使用 `OnScoringEffect` 加符，使用 `OnRoundEndEffect` 添加金币；蓝宝石则在 `OnScoringEffect` 中乘番。

需要其他效果时，可以查下面这张表。`p` 表示 Player，`m` 表示当前材质，`E` 是 `List<Effect>`，`AE` 是 `List<IAnimationEffect>`。材质和遗物的回调参数有些不同，填写时要对照各自的顺序。

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
| `OnDiscardEffect` | `(p, perm, AE, tile, withForce, isClone, m)` | 弃牌；参数顺序与遗物回调不同 |
| `OnSubscribe` / `OnUnsubscribe` | `(p, m)` | 订阅/解绑 |
| `Build()` | 返回 `LuaTileMaterial` | 只创建 |
| `BuildAndRegister()` | 返回 `LuaTileMaterial` | 把构建工厂加入 MaterialProviders |

材质也可以用 `GetDataOrDefault` 和 `SetData` 保存字符串数据。给不同的牌应用材质时，分别调用 `GetMaterial(...)` 或 `.Copy()`，让每张牌有自己的材质实例，避免修改一张牌的数据时影响另一张。

`Copy()` 会通过已经注册的创建方法，生成同类材质并复制数据，因此要先注册再复制。保存后退出续局也要单独测试，确认当前游戏版本能够恢复材质的 Lua 回调。

## 3. 创建一个宝石材质组

两种材质注册好之后，再看脚本后面的 `LuaMaterialSetBuilder`。示例创建了名为 `tutorial_gemstones` 的材质组，把红宝石、蓝宝石以及已有的 `golden`、`crystal` 加进去。我们也可以按同样的方式，把自定义材质和原版材质搭配使用。

| LuaMaterialSetBuilder 方法 | 含义 |
| --- | --- |
| `Create(regName)` | 创建材质组；自定义 ID 内部为 -5，不用数字区分自定义组 |
| `WithAvailableMaterials(string[])` | 替换材质名称列表 |
| `AddMaterial(string)` | 追加一个已注册材质 |
| `OnGenerateCommon(function(set) → LotteryPool<TileMaterial>)` | 覆盖普通池；默认筛选 COMMON |
| `OnGenerateRare(function(set) → LotteryPool<TileMaterial>)` | 覆盖稀有池；默认筛选 RARE 与 EPIC |
| `OnSubscribe(function(p, set))` / `OnUnsubscribe(...)` | 组选中时的玩家绑定生命周期 |
| `Build()` / `BuildAndRegister()` | 只创建 / 加入 MaterialSets；组名重复会跳过注册 |

示例直接使用默认抽取池：普通池里放 COMMON 材质，稀有池里放 RARE 和 EPIC 材质。先按这个方式做就可以了。

如果以后想调整抽取权重，可以在 `OnGenerateCommon` 回调里创建 `CS.LotteryPool(CS.Aotenjo.TileMaterial)()`，通过 `pool:Add(material, positiveWeight)` 加入候选材质，最后返回 pool。这里的 `LotteryPool` 是全局 C# 类型，要从 `CS.LotteryPool` 访问。自定义抽取池时，普通池和稀有池都要有可选材质，否则商店可能抽不到内容。

材质组的显示名称填写在 `material_set_tutorial_gemstones_name` 这个语言键里。重启游戏，开新局时选择这个组，就可以在正常流程中抽到示例材质。注册材质本身不会把当前手牌直接变成红宝石，我们下一步用控制台来测试。

## 4. 进入游戏检查效果

开启测试控制台，进入新局后先输入 `setHand 222m333p`，再输入 `setMat 0-2 tutorial_gems:ruby`。这里的 `0-2` 包含下标0、1、2，也就是把前三张牌改成红宝石。打出它们，应该能看到每张红宝石额外加20符。

再用 `setMat 0 tutorial_gems:sapphire` 把一张牌改成蓝宝石，检查它是否乘2番。关末也看一下，实际参与关末效果的红宝石是否各自增加2金币。

以后想在自己的效果中给牌换材质，可以在收到 `tile` 和 `player` 的效果执行回调里，使用 `tile:SetMaterial(A.TileMaterial.GetMaterial("tutorial_gems:ruby"), player)`。通过 `SetMaterial` 设置，才能一起处理相关通知和订阅，不要直接改 `tile.properties.material`。

完成后再试试复制牌、保存并退出续局，确认各张牌的数据和效果都正常。如果移除了宝石模组，就不要继续使用仍依赖这些材质的测试存档。

源码：[材质 Builder](../../src/API/TileProperties/LuaTileMaterialBuilder.cs)、[材质实例](../../src/API/TileProperties/LuaTileMaterial.cs)、[材质组 Builder](../../src/API/TileProperties/LuaMaterialSetBuilder.cs)。
