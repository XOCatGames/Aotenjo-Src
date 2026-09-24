# 藏品：条件、效果与成长

[目录](README.md) · [English](../en/artifacts.md) · [完整模组](../../example/mods/ex1_artifact) · [实现](../../src/API/Artifact/LuaArtifactBuilder.cs)

先运行二号硬币，再阅读同一模组中的练习玉。前者筛选本次打出的二，后者展示说明格式化、效果队列和字符串存档。所有回调都是可选的；没有设置时保留基类行为。

## 创建与注册

`LuaArtifactBuilder.Create("你的modID:物品名", rarity)` 返回 builder。链式方法返回同一个 builder。

| 方法 | 返回值及用途 |
| --- | --- |
| `Build()` | `LuaArtifact`，只创建，不加入注册表 |
| `BuildAndRegister()` | 创建并加入 `Artifacts.ArtifactList`，普通藏品用此方法 |
| `BuildCraftable()` | `LuaCraftableArtifact`，只创建合成结果 |
| `BuildAndRegisterCraftable()` | 创建并注册合成结果，需同时注册[配方](recipes.md) |

每次启动每个 ID 只注册一次；注册器不自动替换重复 ID。不要在每局、每次计分中重新注册。`WithName` 不是 ID；若不用它，名称自动查 `artifact_ID_name`，适合双语模组。

## 展示和可用性回调

表中的参数就是 Lua `function(...)` 的参数顺序。

| 方法 | 回调参数 → 返回 | 含义 |
| --- | --- | --- |
| `WithHighlight` | `(tile, player, artifact) → bool` | 悬停时高亮；不代表自动触发效果 |
| `WithAvailability` | `(player, artifact) → bool` | 覆盖 Player 版本的全局可用性；不要假定所有 deck/set 重载都调用它 |
| `WithDescription` | `(player, loc, artifact) → string` | 描述；player 可为 nil |
| `WithInShopDescription` | `(player, loc, artifact) → string` | 商店描述 |
| `WithAdditionalInfo` | `(player, artifact) → (string,double)` | 返回 C# ValueTuple；可直接 `return A.Artifact.ToMulFanFormat(1.2)` |
| `WithName` | `(player, artifact) → string` | 名称；这里没有 loc，当前调用传 nil player |
| `WithNameWithColor` | `(player, artifact) → string` | 带样式名称；同样没有 loc |
| `WithSubHeader` | `(player, artifact) → string` | 副标题 |
| `WithSpriteID` | `(player, artifact) → string` | 返回完整注册名，如 `artifact:tutorial_artifact:coin_twos`，不是整数 |

`WithDeckIn(string[])`、`WithDeckBlocked(string[])` 用套组 regName 限定。`WithSetBlocked(string[])` 排除材质组；`WithMaterialRequired(string[])` 要求材质组包含所有列出的材质。`WithSetIn(string[])` 在 deck+set 查询中检查所列各组与当前组是否有材质交集，并非简单 ID 白名单；初学者优先使用材质要求。自定义 `WithAvailability` 会覆盖 Player 重载的默认检查，不能依赖它自动叠加这些限制。

## 触发回调全表

`p=Player`、`perm=Permutation`、`a=当前藏品`。`E` 是 `List<Effect>`；`AE` 是 `List<IAnimationEffect>`。

| 方法 | 参数顺序 | 时机 |
| --- | --- | --- |
| `OnObtain` | `(p, a)` | 获得时，基类已经订阅 |
| `OnRemoved` | `(p, a)` | 移除时，基类先处理解绑 |
| `PreGameInitialized` | `(p, a)` | 新局初始化；会遍历全部注册藏品，不表示玩家拥有它 |
| `ResetArtifactState` | `(p, a)` | 重置状态；新局、移除并要求重置、失败结束等路径会调用，不是每关开始 |
| `OnSubscribeToPlayer` | `(p, a)` | 绑定玩家，也可能在读档/重绑时发生 |
| `OnUnsubscribeToPlayer` | `(p, a)` | 解绑玩家，撤销对应订阅 |
| `OnTileEffect` | `(p, perm, tile, E, a)` | 单牌计分效果 |
| `OnTilePostEffect` | `(p, perm, tile, E, a)` | 单牌后置效果收集阶段 |
| `OnUnusedTileEffect` | `(p, perm, tile, E, a)` | 未使用牌效果收集 |
| `OnDiscardTileEffect` | `(p, tile, AE, withForce, isClone, a)` | 弃牌效果；没有 perm 参数 |
| `OnBlockEffect` | `(p, perm, block, E, a)` | 面子效果收集 |
| `OnBlockAnimEffect` | `(p, perm, block, AE, a)` | 面子动画效果收集 |
| `OnSelfEffect` | `(p, perm, E, a)` | 藏品自身计分阶段 |
| `OnRoundEndEffect` | `(p, perm, AE, a)` | 关末效果收集；处理可能为空的 perm |

在牌回调中，想表达“本次打出”，请检查 `p:Selecting(tile)`。只检查牌点数会误把已有牌当成本次输入。效果触发次数还会受重触发机制影响；`OnSelfEffect` 不等于整局只执行一次。

## 成长与存档

完整示例把 `plays` 存成字符串。说明函数读取状态并返回文字；`OnSelfEffect` 先追加当前倍率，再追加 `SimpleEffect`，由后者真正执行时加一次计数。对该 Lua 藏品调用 `Serialize()` / `Deserialize(string)` 会读写这个字典。跨进程续局仍须实测完整游戏存档路径。

注册表保存的是共享藏品对象，不是每次获得都克隆一个独立 Lua 对象。不要用同一个 ID 同时代表多个独立实例或多个玩家的隔离状态。也不要在 `OnSubscribeToPlayer` 无条件清零存档；重新绑定不等于新局。

如果已有版本字段叫 `level`，新增字段应保留默认值，必要时在读取时迁移：`tonumber(a:GetDataOrDefault("level", "0")) or 0`。已发布 ID 改名会影响存档引用；改显示名通常只需改语言文件。

验收：二与非二、选中与未选中分别测试；练习玉第一次×1.0，第二次×1.1；打开图鉴不成长；保存退出重进后检查数值；移除后不再触发；新局归零。不要把“成功注册”当作上述行为已经通过。
