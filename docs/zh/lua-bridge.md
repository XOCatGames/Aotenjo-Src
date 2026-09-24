# Lua 与游戏对象

[目录](README.md) · [English](../en/lua-bridge.md)

`CS` 是 xLua 提供的 C# 类型入口。`local A = CS.Aotenjo` 只是缩写。并非所有游戏类型都在这个命名空间：例如 `Yaku`、`TransformMaterialEffect` 是全局类型，对应 `CS.Yaku`、`CS.TransformMaterialEffect`。以[源码签名](../../reference/api-signatures.md)的 namespace 为准。

| 写法 | 含义 |
| --- | --- |
| `A.LuaArtifactBuilder.Create(id, A.Rarity.COMMON)` | 静态方法，用点 |
| `builder:OnSelfEffect(callback)` | 对象方法，用冒号，隐式传 self |
| `A.EarnMoneyEffect(2, artifact)` | 调 C# 构造函数，不写 `new` |
| `player.Level`、`tile.properties.material` | 读取字段或属性 |
| `loc("some_key")` | 调传入的 C# 委托，用普通函数语法 |
| `nil` | C# 引用类型的 null；不等于空数组 |

## 集合与回调

Lua table 通常从1开始。C# `List<T>` 从0开始、用 `.Count`；数组从0开始、用 `.Length`。不能对 C# List 使用 `ipairs`、`#list` 或 `table.insert`。

```lua
local tiles = player:GetHandDeckCopy()
for i = 0, tiles.Count - 1 do
    local tile = tiles[i]
    CS.Aotenjo.Logger.Log(tile:ToString())
end
```

这个方法复制的是容器，里面仍是原 Tile 对象。迭代玩家集合时优先取得副本；更改牌的属性使用 `tile:SetMaterial(material, player)` 等方法，避免绕开解绑、订阅与记录。

需要强类型数组时：

```lua
local function array(t, values)
    local result = CS.System.Array.CreateInstance(typeof(t), #values)
    for i, value in ipairs(values) do result[i - 1] = value end
    return result
end
local names = array(CS.System.String, {"standard"})
local packs = array(CS.System.Int32, {2})
local empty = array(CS.System.String, {})
local artifacts = CS.System.Collections.Generic.List(CS.Aotenjo.Artifact)()
```

回调是交给引擎稍后调用的函数。参数顺序由 API 决定，不能因为名称相近而照搬别的回调。`WithName` 是 `(player, artifact)`，`WithDescription` 才是 `(player, loc, artifact)`。图鉴里 `player` 可以为 nil。返回 bool 的函数必须明确 `return true/false`；构建效果的 Action 回调不需要返回列表，直接对传入的 `effects:Add(...)`。

## 效果队列

引擎先收集效果，再按动画流程执行 `Ingest`。`effects:Add(ScoreEffect.AddFu(...))` 是排队，并非立即修改玩家。`List<Effect>` 放普通效果；`List<IAnimationEffect>` 可以放 `effect:OnTile(tile)`、`:OnBlock(block)` 这样的有目标动画包装。不要把 `OnTile` 包装放进要求 `Effect` 的列表。

判定、高亮、名称、说明函数可能被反复调用，必须只读。成长状态、发奖励和随机抽取放在真正执行效果的位置，见[玩法食谱](cookbook.md)。不要在 `init()` 获取当前玩家；这时通常还在主菜单。

## 用好类型与状态

`LuaArtifact` 的 `GetDataOrDefault(key, defaultValue)` 和 `SetData(key, value)` 都收字符串：读数值用 `tonumber(... ) or 0`，写回用 `tostring(value)`。Lua 闭包中的局部变量、全局 table 和委托都不会因此自动进入存档。

`Rarity` 的值是 `COMMON`、`RARE`、`EPIC`、`LEGENDARY`、`ANCIENT`；是否能出现在某种商店或池中还取决于该系统的筛选逻辑，入门藏品使用前三种。

某方法在源码里存在但游戏报签名/委托错误时，可能是旧发行版或该平台 xLua/AOT 绑定缺失。先用本手册完整示例缩小问题，不要通过改参数个数碰运气。高级泛型 EventBus 见[事件限制](cookbook.md)；旧版 `player:PreKongTileEvent("+", handler)` 已不适用于当前快照。
