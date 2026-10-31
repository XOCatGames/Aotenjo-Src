# 遗物 API v2 迁移说明 / Artifact API v2 migration

本分支引入 `ArtifactRegistry.RuntimeApiVersion == 2`。开发基线为 `v0.7.1.0-PRE16`，版本号本身没有上调，不能据此声称此前发布的 PRE16 二进制已支持 API v2。发布此改动时，应把模组最低游戏版本设为实际包含该功能的发布版本。

This source introduces Artifact API v2. The branch is based on PRE16 without changing the game version; earlier PRE16 binaries do not thereby gain v2 support. Set the minimum supported game version to the actual release that ships this change.

## 注册定义与运行实例

`LuaArtifactBuilder.BuildAndRegister()` 和 `BuildAndRegisterCraftable()` 现在返回 `ArtifactRegistration`。它只提供 ID、只读配置和解锁信息；不能用来读写成长、Lua 数据、状态或触发事件。注册会冻结 Builder 的配置列表和回调，此后修改 Builder 不影响已注册工厂。

模组仍在 `init()` 中注册一次。每局目录会按注册顺序构造初始实例，并包含尚未持有的遗物。同局可持有多件同名遗物，各自拥有成长、状态、词条及订阅。操作当前宿主时使用回调参数 `artifact`；`player:GetRunArtifact(id)` 返回库存中该类型的第一件，没有持有时返回初始候选。检查类型存在用 `HasArtifact(id)`，检查具体对象用 `HoldsArtifact(artifact)`。不要在文件顶层缓存运行实例后跨局使用。

```lua
local definition = CS.Aotenjo.LuaArtifactBuilder.Create(
    "example:counter", CS.Aotenjo.Rarity.COMMON)
    :OnSelfEffect(function(player, perm, effects, artifact)
        local growth = tonumber(artifact:GetDataOrDefault("growth", "0"))
        effects:Add(CS.Aotenjo.ScoreEffect.AddFu(growth, artifact))
    end):BuildAndRegister()
local id = definition:GetRegName()
```

`Artifacts.<名称>` 同样是定义记录。`Artifacts.GetArtifact`、`Artifacts.ArtifactList`、`Artifacts.NameToArtifactMap` 已移除；获取、移除和效果来源参数需要本局实例。`ArtifactRegistry.CreatePreview(id)` 只生成展示对象，不执行新局随机预设，不能加入库存或订阅战斗事件。

Registration now returns an immutable definition record. Callback parameters and `player:GetRunArtifact(id)` provide the current run's instance. Register once in `init()` and never cache a runtime object across runs. Builder changes after registration do not change its frozen factory. The old global instance lookup/list APIs have been removed.

新增副本使用 `player:CreateArtifactInstance(id)`，再通过普通获取接口授予。每个对象有对局内稳定的整数 `InstanceId`，`player:GetArtifactInstance(instanceId)` 精确查找；创建会执行正常重置和预初始化，查找不会消耗随机数。

```lua
local first = player:CreateArtifactInstance("example:counter")
local second = player:CreateArtifactInstance("example:counter")
assert(player:ObtainArtifact(first, true))
assert(player:ObtainArtifact(second, true))
assert(first.InstanceId ~= second.InstanceId)
```

Multiple copies of the same definition are supported. Use the callback's artifact for host operations, `CreateArtifactInstance(id)` to create a fresh copy, `GetArtifactInstance(instanceId)` for exact lookup and `HoldsArtifact(artifact)` for membership. Type lookup returns the first held copy in inventory order, or the initial candidate when none is held. Copies have separate state and event subscriptions.

控制台重复执行 `give <类型ID> true` 可获取多件。`affix <类型ID>#<实例编号> gilded` 指定一件；补全会列出持有对象的编号。不带编号时选择库存第一件。`true` 沿用强制获取语义，正常商店的类型抽池规则不变。

## 普通 C# 遗物与旧计分工厂

内置普通遗物和 Boss 反转能力现已迁入各自的 `Artifact` 子类。旧的三个 `CreateOn…EffectArtifact` 静态工厂、`Artifact.onTileEffect/onBlockEffect/selfEffect` 字段，以及 `Artifact` 和 `CraftableArtifact` 的五参数计分回调构造器已移除。C# 扩展应通过子类覆写 `AppendOnTileEffects`、`AddOnBlockEffects` 或 `AppendOnSelfEffects`，使用 `this` 作为自身效果来源。

Lua 模组继续通过 `LuaArtifactBuilder` 的 `OnTileEffect`、`OnBlockEffect`、`OnSelfEffect` 注册，回调最后一个参数仍为实际运行遗物，签名不变。高亮、可用性和赠牌配置入口继续保留。

Built-in callback artifacts and reversed boss rewards now have concrete classes. The old scoring factories, base scoring callback fields and five-argument callback constructors have been removed. C# extensions override the scoring methods; Lua mods keep using the Builder callbacks with the runtime artifact as their final parameter.

## 配方使用 ID

`LuaArtifactRecipeBuilder.Build`、`BuildAndRegister` 改为接收 `List<string>` 和结果 ID：

```lua
local inputs = CS.System.Collections.Generic.List(CS.System.String)()
inputs:Add("example:copper")
inputs:Add("example:silver")
local recipe = CS.Aotenjo.LuaArtifactRecipeBuilder.BuildAndRegister(
    "example:combined_recipe", inputs, "example:combined")
assert(recipe ~= nil)
```

参与合成时，每个材料 ID 按库存顺序选取尚未选中的实例；重复 ID 表示需要多件。产物使用未持有、未预约的对象，必要时创建副本；已有同名产物不再阻止合成。词条继承仍按配方材料顺序取同词条的首个来源，并保存具体材料实例的冻结抽选结果。

Recipes take input type IDs and a result type ID. Repeated input IDs require distinct copies, selected in inventory order. Existing or pending copies of an output do not get overwritten; crafting uses a separate instance. Preview queries do not allocate runtime instances or draw RNG.

## 版本与失败诊断

`modinfo.json` 应声明 `"artifactApiVersion": 2`。非零且不匹配的版本会明确报错；未声明版本的旧模组仍尝试加载，使用已删除接口时会失败，不会静默代理回全局单例。日志会标出模组 ID、入口路径和 API v2；本次失败加载新增的遗物工厂和词条注册会回滚。

切换模组会话前会解绑并释放旧玩家目录，清理注册工厂后再释放 LuaEnv。旧运行对象不能被新局获取；同一个对象不能重复持有或预约，同类型的不同对象可以。`force` 只绕过原有业务准入规则，不绕过这些实例约束。

Declare `"artifactApiVersion": 2` in modinfo.json. Unsupported explicit versions fail with a diagnostic. Undeclared legacy mods may load if compatible; removed calls fail with the mod ID and entry path in the log. Failed registrations roll back. Forced acquisition still rejects foreign, preview and duplicate instances.

## 状态、词条与存档

`GetArtifacts()` 返回持有顺序的列表快照，修改它不会修改库存；重排请传入完整、无重复、全部属于本局的排列。`AffixState` 是只读状态副本；设置词条使用 Player 接口。自定义词条数据在读取、保存和延迟替换时均复制，不能靠保留外部字典引用修改库存状态。

存档格式升为 4：保存实例编号到类型的目录、有序实例库存，`Round.artifactData/artifactStatusData` 和词条映射改用实例编号字符串作键；子类字符串载荷不变。延迟奖励保存来源、目标及替换对象的实例编号。完整目录仍包含未持有的随机预设。旧数字 ID、旧类型映射仍可读；旧档重复类型迁移成独立对象，各自读取旧载荷。缺失实例载荷、未知编号、重复实例库存、定义来源冲突会拒绝续局并保留原存档。图鉴使用独立预览；历史成长描述使用历史快照。

`GetArtifacts()` returns a list snapshot; reordering requires an exact permutation of held instances. `AffixState` is a copied snapshot. Use Player APIs to change affixes. Save format 4 keys state and affixes by instance ID and preserves exact deferred reward references. Legacy type-keyed saves remain readable; repeated type IDs become separate objects. Invalid instance data or mismatched definition owners stop continuation without overwriting the original save. Collection previews do not consume run RNG; historical descriptions use saved text.

测试覆盖真实 xLua 的独立副本、词条成长及退订，托管回归覆盖独立实例、合成和存档引用迁移。原生 JsonUtility 往返用例位于 `ArtifactAffixSaveTests`，需要在 Unity 中执行；编译通过不能代替原生执行。教程与兼容性示例均采用 v2。
