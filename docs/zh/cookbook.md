# 把自己的想法写成规则

[目录](README.md) · [English](../en/cookbook.md)

以下片段放在藏品示例的 Builder 链中，使用 `local A = CS.Aotenjo`；不是单独可安装模组。先复制[完整模组](../../example/mods/ex1_artifact)，替换对应回调，再同步修改说明文字。

## “每张本次打出的九加15符”

```lua
:OnTileEffect(function(player, perm, tile, effects, artifact)
    if player:Selecting(tile) and tile:IsNumbered(9) then
        effects:Add(A.ScoreEffect.AddFu(15, artifact))
    end
end)
```

改为 `tile:GetCategory() == A.Tile.Category.Suo` 可筛选索子；`Wan` 为万，`Bing` 为饼。`tile:IsYaoJiu(player)` 是会考虑玩家规则的幺九判断。每次只改一个条件，并写一个符合条件和一个不符合条件的测试。

## “普通弃牌返1金币，强制弃牌不返”

```lua
:OnDiscardTileEffect(function(player, tile, effects, withForce, isClone, artifact)
    if not withForce and not isClone then
        effects:Add(A.EarnMoneyEffect(1, artifact):OnTile(tile))
    end
end)
```

这个回调没有 `perm`。这里使用动画效果列表，需要把奖励与这张牌关联。是否排除克隆触发是设计选择；示例明确排除。

## “10%机会得3金币，并保存中奖次数”

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

为 `my_mod_roll` 提供双语文本，并在 `ResetArtifactState` 清零 `wins`。`GenerateRandomInt(n, category)` 返回0到n-1，n必须正数；独立 category 避免与默认流混用。不要在描述、predicate 或高亮回调里抽随机数。这个 SimpleEffect 在未中奖时也有自身的展示文字；需要只在中奖时显示动画时，可研究 [MaybeEffect](../../src/Effects/MaybeEffect.cs) 的语义，而不是把随机判定搬进 UI 查询。

## “计分后把这张牌变成红宝石”

在同时安装并已注册宝石模组时，可以在有 tile 的回调中排队：

```lua
effects:Add(CS.TransformMaterialEffect(
    A.TileMaterial.GetMaterial("tutorial_gems:ruby"), artifact, tile, "my_mod_transform"))
```

此处是全局 `CS.TransformMaterialEffect`，不是 `A.TransformMaterialEffect`。添加双语提示键。如果要作为弃牌或关末动画效果，追加 `:OnTile(tile)`。依赖模组未安装时 GetMaterial 会抛错；发布前把材质放到自己的模组中并先注册，或清楚声明依赖并检测。每个新目标使用新材质实例。

## 常用对象速查

| 对象 | 入口 | 返回与注意 |
| --- | --- | --- |
| Player | `Selecting(tile)` | bool，本次选中语义 |
| Player | `GetHandDeckCopy()`、`GetTilePool()` | C# List，容器副本里的牌仍是对象引用 |
| Player | `GetArtifacts()` | 当前藏品列表，不要直接改注册表模拟获取 |
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

完整声明见[API 签名](../../reference/api-signatures.md)。新增效果优先复用 `ScoreEffect.AddFu/AddFan/MulFan(number, source)`、`EarnMoneyEffect(int, source)`、`TextEffect(key, source)`、`SimpleEffect(key, source, function(player))`。`source` 是本藏品对象，纯材质效果可以传 nil。

## 高级事件与现有限制

当前 Player 使用 [EventBus](../../src/Event/NewEventSystem/EventBus.cs) 发布[事件类型](../../reference/events.md)，旧版的实例事件加减订阅语法已经迁移。只要 Builder 已有相应回调，优先用它。

EventBus 的 C# 契约是 `Subscribe<T>(player, Action<T>, priority=0, once=false)` / `Unsubscribe<T>(player, Action<T>)`。T 必须是 PlayerEvent 子类，按**精确发布类型**匹配；订阅父类型不会自动收到全部子事件。高优先级先处理，once在触发后移除。保存同一个委托引用，在解绑时移除，重绑前避免重复；查询类事件不得顺手发奖励。

这不是能直接写成 `EventBus.Subscribe<T>` 的 Lua 语法。xLua 需要闭合泛型和匹配 Action 委托，不同平台的反射/AOT 支持可能不同；目前没有非泛型 Lua 事件适配层。因此这里不给一个未经目标发行版验证的万能 Lua 订阅器。需要“成功加杠”时，正确事件是 `PostKongTilesEvent`，不是可能被取消的 `PlayerEvents.PreKongTileEvent`；实现新适配前应先验证目标平台绑定。

直接调用小道具时，`UseOnTiles` 现在返回 `GadgetUseResult`，检查 `.Success`，不要把结果对象本身当布尔值。成功使用后的次数由 `Player.PostUsedGadget` 统一处理。当前没有 LuaGadgetBuilder，也没有完整 Lua Boss 注册接口。
