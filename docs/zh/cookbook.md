# 给遗物加点新效果

[目录](README.md) · [English](../en/cookbook.md)

二号硬币做好之后，我们再给它换几种玩法。下面这些片段都可以放进遗物的 Builder 链中，开头沿用 `local A = CS.Aotenjo` 的简写。

先复制一份[完整模组](../../example/mods/ex1_artifact)，然后替换对应的回调，最后改掉说明文字。片段本身没有模组清单、图片等文件，所以要放进完整模组里使用。

## 1. 打出九时加15符

先从最熟悉的二号硬币开始。把判断的点数改成9，再把获得金币的效果换成加符：

```lua
:OnTileEffect(function(player, perm, tile, effects, artifact)
    if player:Selecting(tile) and tile:IsNumbered(9) then
        effects:Add(A.ScoreEffect.AddFu(15, artifact))
    end
end)
```

这样，每张本次打出的九就会额外加15符。想按花色判断的话，还可以把点数条件换成 `tile:GetCategory() == A.Tile.Category.Suo`，表示索子；`Wan` 表示万，`Bing` 表示饼。`tile:IsYaoJiu(player)` 则可以判断幺九牌，并考虑当前玩家的规则。

每次先改一个条件，再准备符合和不符合条件的牌各试一次，这样比较容易看出效果是否写对。

## 2. 普通弃牌时返还1金币

这次把触发时机换成弃牌。我们希望普通弃牌时获得1金币，强制弃牌和克隆触发时不给奖励：

```lua
:OnDiscardTileEffect(function(player, tile, effects, withForce, isClone, artifact)
    if not withForce and not isClone then
        effects:Add(A.EarnMoneyEffect(1, artifact):OnTile(tile))
    end
end)
```

注意 `OnDiscardTileEffect` 的参数中没有 `perm`。它收到的是动画效果列表，所以这里用 `:OnTile(tile)` 把奖励动画关联到被弃掉的牌。是否排除克隆触发可以按自己的玩法设计，这个例子选择了排除。

## 3. 有10%的机会获得3金币

接下来加一点随机性：每次遗物自身计分时，有10%的机会获得3金币，并把中奖次数记下来。随机判断放进 `SimpleEffect`，等效果实际执行时再抽取：

```lua
:OnSelfEffect(function(player, perm, effects, artifact)
    effects:Add(A.SimpleEffect("my_mod_roll", artifact, function(p)
        if p:GenerateRandomInt(10, "my_mod:reward") == 0 then
            p:EarnMoney(3)
            local wins = tonumber(artifact:GetDataOrDefault("wins", "0")) or 0
            artifact:SetData("wins", tostring(wins + 1))
        end
    end))
end)
```

`GenerateRandomInt(10, "my_mod:reward")` 会得到0到9中的一个整数，抽到0就算中奖，因此概率是10%。这里给随机类别加了自己的名字，避免和默认随机序列混在一起。使用其他范围时，`n` 要是正数，结果从0到n-1。

记得给 `my_mod_roll` 添加中英文文本，并在 `ResetArtifactState` 中把 `wins` 清零。描述、番种判定和高亮回调会反复调用，不适合放随机抽取。

这个 `SimpleEffect` 即使没有中奖，也会显示自己的提示文字。如果想只在中奖时播放动画，可以进一步看看 [MaybeEffect](../../src/Effects/MaybeEffect.cs) 的实现，把随机判断留在效果流程里。

## 4. 计分后把牌变成红宝石

如果同时装好了宝石模组，并且红宝石已经注册，就可以在收到 `tile` 的回调中添加下面的效果：

```lua
effects:Add(CS.TransformMaterialEffect(
    A.TileMaterial.GetMaterial("tutorial_gems:ruby"), artifact, tile, "my_mod_transform"))
```

这里的 `TransformMaterialEffect` 是全局类型，要写成 `CS.TransformMaterialEffect`，不能放在 `A` 下面。它会把指定的牌改成红宝石，`my_mod_transform` 是显示提示用的语言键，也要补上中英文文本。如果放进弃牌或关末的动画效果列表，在后面追加 `:OnTile(tile)` 即可。

给新的目标牌使用新的材质实例，可以避免数据互相影响。另一个要注意的地方是依赖：没有安装宝石模组时，`GetMaterial` 会报错。准备发布时，可以把材质一起放进自己的模组并先注册；也可以保留依赖，但要在说明中写清楚，并检查它是否存在。

## 5. 还可以调用哪些方法

下面整理了一些常用方法。想给上面的效果换一个判断条件，或者操作玩家和牌的数据时，可以从这里找起：

| 对象 | 入口 | 返回与注意 |
| --- | --- | --- |
| Player | `Selecting(tile)` | 返回 bool，判断是否为本次选中的牌 |
| Player | `GetHandDeckCopy()`、`GetTilePool()` | C# List，容器副本里的牌仍是对象引用 |
| Player | `GetArtifacts()` | 获取玩家当前的遗物列表；让玩家获得遗物请用 ObtainArtifact |
| Player | `ObtainArtifact(artifact, forced=false)` | bool；用正常获取流程 |
| Player | `AddNewTileToPool(tile)` | bool，可能被规则取消 |
| Player | `EarnMoney(int)`、`GenerateRandomInt(int, string)` | 真正执行效果时调用 |
| Tile | `GetOrder()`、`GetCategory()` | 经过变换后的牌值与类别 |
| Tile | `IsNumbered()` / `IsNumbered(int)` | 数牌 / 指定点数 |
| Tile | `Copy()` | 独立可变状态副本；特殊牌优先用此虚方法 |
| Tile | `SetMaterial(mat, player)`、`SetFont(font, player)`、`SetMask(mask, player)` | 应用属性并走玩家通知与订阅 |
| Permutation | `ToTiles()` | 当前组合全部牌，可能包含以前已落定牌 |
| Block | `tiles`、`IsAAA()` | 牌列表与刻子判断；更多见源码 |
| Artifact | `GetRegName()` | 稳定 ID，不是翻译名 |

更多参数可以查 [API 参数速查](../../reference/api-signatures.md)。加符、加番和乘番可以使用 `ScoreEffect.AddFu/AddFan/MulFan(number, source)`，发金币用 `EarnMoneyEffect(int, source)`，显示文字用 `TextEffect(key, source)`，需要自己写执行逻辑时用 `SimpleEffect(key, source, function(player))`。

这些方法中的 `source` 填当前遗物对象；如果是没有来源遗物的纯材质效果，可以传 nil。

## 6. 进一步使用事件和小道具接口

前面的效果用 Builder 提供的回调就能完成。如果想监听更细的游戏事件，可以继续了解 [EventBus](../../src/Event/NewEventSystem/EventBus.cs) 和对应的[事件类型](../../reference/events.md)。当前 Player 已经使用这套事件系统，旧教程中给玩家实例事件加减订阅的写法需要相应调整。

在 C# 中，订阅和取消订阅的方法分别是 `Subscribe<T>(player, Action<T>, priority=0, once=false)` 和 `Unsubscribe<T>(player, Action<T>)`。T 必须是 PlayerEvent 的子类，并且和实际发布的事件类型完全对应；订阅父类型，不会自动收到它下面的全部子事件。priority 越高越先执行，once 为 true 时，触发一次就会移除订阅。

使用时要保存同一个委托引用，方便解绑时移除，并避免重复绑定。查询类事件可能反复触发，也不要在查询过程中发奖励。

这些是 C# 的泛型方法，不能直接照抄成 Lua 的 `EventBus.Subscribe<T>`。xLua 需要先确定具体泛型类型，再匹配对应的 Action 委托，不同平台的反射和 AOT 支持也可能不同。目前还没有专门给 Lua 使用的非泛型事件接口，所以这部分需要先确认目标游戏版本的绑定支持。如果 Builder 已经提供了所需回调，先用 Builder 会更方便。

例如，想在成功加杠后触发效果，应关注 `PostKongTilesEvent`。`PlayerEvents.PreKongTileEvent` 发生在操作前，之后仍可能被取消，不能用它表示已经成功加杠。

研究小道具接口时也要留意：`UseOnTiles` 返回的是 `GadgetUseResult`，要读取 `.Success` 判断是否成功，不能把整个结果对象直接当作布尔值。成功使用后的次数由 `Player.PostUsedGadget` 统一处理。目前还没有 `LuaGadgetBuilder` 或完整的 Lua Boss 注册接口，新增这两类内容还需要额外的游戏接口支持。
