# Lua 与游戏对象

[目录](README.md) · [English](../en/lua-bridge.md)

看过第一个模组后，大家应该已经注意到了 `CS.Aotenjo`、`effects:Add(...)` 这些写法。这一篇就来解释一下，怎样在 Lua 里使用游戏提供的对象和方法。

## 1. 用 CS 找到游戏里的类型

`CS` 是 xLua 提供的入口，我们通过它访问 C# 类型。示例开头的 `local A = CS.Aotenjo` 只是取了一个简写，后面写 `A.LuaArtifactBuilder` 就相当于写 `CS.Aotenjo.LuaArtifactBuilder`。

有些类型不在 `Aotenjo` 下面，例如 `Yaku` 和 `TransformMaterialEffect`，要分别写成 `CS.Yaku`、`CS.TransformMaterialEffect`。如果不确定，可以查 [API 参数速查](../../reference/api-signatures.md)里标出的 namespace。

下面是教程中经常用到的几种写法：

| 写法 | 含义 |
| --- | --- |
| `A.LuaArtifactBuilder.Create(id, A.Rarity.COMMON)` | 静态方法，用点 |
| `builder:OnSelfEffect(callback)` | 对象方法，用冒号，隐式传 self |
| `A.EarnMoneyEffect(2, artifact)` | 调 C# 构造函数，不写 `new` |
| `player.Level`、`tile.properties.material` | 读取字段或属性 |
| `loc("some_key")` | 调传入的 C# 委托，用普通函数语法 |
| `nil` | C# 引用类型的 null；不等于空数组 |

## 2. 取出列表里的每一张牌

假设我们想看看玩家的每张手牌，可以先调用 `player:GetHandDeckCopy()`。它返回的是 C# 的 `List<T>`，访问方法和 Lua table 有一点不同：Lua table 通常从1开始，而 C# 列表从0开始，数量用 `.Count` 获取。

因此，遍历手牌可以这样写：

```lua
local tiles = player:GetHandDeckCopy()
for i = 0, tiles.Count - 1 do
    local tile = tiles[i]
    CS.Aotenjo.Logger.Log(tile:ToString())
end
```

这里的循环从 `0` 一直走到 `tiles.Count - 1`，把每张牌输出到日志中。C# 列表不能直接套用 Lua 的 `ipairs`、`#list` 或 `table.insert`；如果返回的是 C# 数组，数量则要用 `.Length`，下标同样从0开始。

`GetHandDeckCopy()` 会复制一份列表，方便我们遍历，不过列表里的 Tile 仍然是玩家原来的牌。要修改牌的属性时，使用 `tile:SetMaterial(material, player)` 这样的游戏方法，让相关的解绑、订阅和记录也一起处理。

注册番种等接口会要求我们传入指定类型的数组。可以先定义一个小工具，把 Lua table 中的值放进 C# 数组里：

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

上面分别创建了字符串数组、整数数组、空字符串数组，以及一个用来存放遗物的 C# 列表。之后碰到这些参数，就可以按相同的方式填写。

## 3. 游戏什么时候调用我们写的函数

像 `OnTileEffect(function(...) ... end)` 这样的写法，是把一个函数交给游戏，等单张牌计分时再调用，这个函数就叫“回调”。

每种回调收到的参数都由 API 决定。例如，`WithName` 的参数是 `(player, artifact)`，`WithDescription` 才是 `(player, loc, artifact)`，不能因为名字相近就照搬。图鉴中没有正在游玩的玩家时，`player` 还可能是 nil。

需要返回 bool 的回调，要明确写出 `return true` 或 `return false`。添加效果的 Action 回调则不用返回列表，直接向传进来的列表调用 `effects:Add(...)` 就可以。

## 4. 为什么要把效果放进列表

我们在示例里写下 `effects:Add(ScoreEffect.AddFu(...))` 时，是先把加符效果放进列表。游戏收集完效果后，再按照动画流程调用 `Ingest` 来实际执行。所以，添加效果的那一刻，玩家的数值还没有立即改变。

`List<Effect>` 用来放普通效果。`List<IAnimationEffect>` 还可以放 `effect:OnTile(tile)`、`:OnBlock(block)` 这样的效果，告诉游戏把动画关联到哪张牌或哪个面子。回调要求哪一种列表，就使用对应的效果；经过 `OnTile` 包装的结果不能放进要求 `Effect` 的列表。

判定、高亮、名称和说明函数可能在界面刷新时反复调用，因此这些函数只负责读取数据。成长、发奖励和抽随机数，要放在真正执行效果的位置，可以参考[更多玩法示例](cookbook.md)。`init()` 通常在主菜单阶段运行，这时也不适合获取当前玩家；等回调把 `player` 传进来再使用。

## 5. 记录遗物的成长数据

如果要记录遗物触发了几次，可以使用 `LuaArtifact` 的 `GetDataOrDefault(key, defaultValue)` 和 `SetData(key, value)`。它们保存的是字符串，所以读取数值时用 `tonumber(... ) or 0` 转换，写回时再用 `tostring(value)` 转回字符串。

只把数值存在 Lua 局部变量或全局 table 中，并不会自动写入这份存档数据，函数委托也一样。练习玉的完整示例展示了具体用法。

设置稀有度时，可以选择 `COMMON`、`RARE`、`EPIC`、`LEGENDARY`、`ANCIENT`。不过，商店和抽取池还会进行自己的筛选。制作第一个遗物时，先用前三种会更方便测试。

## 6. 遇到类型或参数错误时

如果源码里有这个方法，游戏却提示找不到签名或无法转换委托，先试一下本教程的完整示例。游戏版本较旧，或者当前平台缺少对应的 xLua/AOT 绑定，都可能出现这种情况；只改参数个数通常解决不了问题。

事件系统的进阶说明放在[更多玩法示例](cookbook.md)中。旧教程里的 `player:PreKongTileEvent("+", handler)` 已不适用于当前源码，要按新的事件接口处理。
