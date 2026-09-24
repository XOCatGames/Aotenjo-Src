# 自定义遗物

[目录](README.md) · [English](../en/artifacts.md) · [完整模组](../../example/mods/ex1_artifact) · [实现](../../src/API/Artifact/LuaArtifactBuilder.cs)

在快速上手里，我们已经做出了二号硬币。这一篇继续看看它是怎么创建的，再给同一个模组里的练习玉加上成长效果。

可以先打开完整示例中的 `script/tutorial_artifact/artifacts.lua`，一边看代码，一边对照下面的说明。二号硬币负责演示“满足条件时触发”，练习玉则演示“每次触发后变强”。

## 1. 创建一个遗物

我们用 `LuaArtifactBuilder.Create("你的modID:物品名", rarity)` 开始创建遗物。它会返回一个 builder，后面的 `:OnTileEffect(...)`、`:WithDescription(...)` 等方法，都是在给这个遗物添加设置，所以可以像示例里那样一行接一行地写。

这些设置都是可选的，只添加自己需要的就好；没有设置的部分会沿用基类的行为。设置完成后，用下面的方法创建遗物：

| 方法 | 返回值及用途 |
| --- | --- |
| `Build()` | `LuaArtifact`，只创建，不加入注册表 |
| `BuildAndRegister()` | 创建并加入 `Artifacts.ArtifactList`，普通遗物用此方法 |
| `BuildCraftable()` | `LuaCraftableArtifact`，只创建合成结果 |
| `BuildAndRegisterCraftable()` | 创建并注册合成结果，需同时注册[配方](recipes.md) |

普通遗物使用 `BuildAndRegister()` 就可以。把整段创建代码放进模组的 `init()` 中，每次启动注册一次，不用在每局开始或每次计分时重新注册。同一个 ID 重复注册时，游戏不会自动替换原来的遗物。

这里的 ID 用来让游戏找到遗物，显示给玩家看的名字则放在语言文件里。默认会读取 `artifact_ID_name`，所以做双语模组时，通常不用额外写 `WithName`。

## 2. 设置名称、说明和可用条件

如果想让说明随着等级变化，或者只让遗物在某些条件下出现，可以使用下面这些设置。表中括号里的内容，就是传给 Lua `function(...)` 的参数，按这个顺序填写即可。

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

需要限制适用套组时，`WithDeckIn(string[])` 和 `WithDeckBlocked(string[])` 接收套组的 regName。材质方面，`WithSetBlocked(string[])` 可以排除材质组，`WithMaterialRequired(string[])` 则要求当前材质组包含列出的所有材质。

`WithSetIn(string[])` 的判断稍有不同：在同时查询套组和材质组时，它会检查所列各组与当前组是否有共同材质。刚开始使用时，建议先用更直观的材质要求。另一个需要留意的地方是 `WithAvailability`：自己设置这个回调后，会覆盖 Player 版本的默认检查，上面这些限制不会自动叠加进去。

## 3. 给遗物添加触发效果

二号硬币使用了 `OnTileEffect`，在每张牌计分时检查条件。如果想改成弃牌、关末或获得遗物时触发，就要选择对应的回调。

下面列出了全部触发回调，方便查阅。为了让表格更容易看，`p` 表示 Player，`perm` 表示 Permutation，`a` 表示当前遗物；`E` 是普通效果列表 `List<Effect>`，`AE` 是动画效果列表 `List<IAnimationEffect>`。

| 方法 | 参数顺序 | 时机 |
| --- | --- | --- |
| `OnObtain` | `(p, a)` | 获得时，基类已经订阅 |
| `OnRemoved` | `(p, a)` | 移除时，基类先处理解绑 |
| `PreGameInitialized` | `(p, a)` | 新局初始化；会遍历全部注册遗物，不表示玩家拥有它 |
| `ResetArtifactState` | `(p, a)` | 重置状态；新局、移除并要求重置、失败结束等路径会调用，不是每关开始 |
| `OnSubscribeToPlayer` | `(p, a)` | 绑定玩家，也可能在读档/重绑时发生 |
| `OnUnsubscribeToPlayer` | `(p, a)` | 解绑玩家，撤销对应订阅 |
| `OnTileEffect` | `(p, perm, tile, E, a)` | 单牌计分效果 |
| `OnTilePostEffect` | `(p, perm, tile, E, a)` | 单牌后置效果收集阶段 |
| `OnUnusedTileEffect` | `(p, perm, tile, E, a)` | 未使用牌效果收集 |
| `OnDiscardTileEffect` | `(p, tile, AE, withForce, isClone, a)` | 弃牌效果；没有 perm 参数 |
| `OnBlockEffect` | `(p, perm, block, E, a)` | 面子效果收集 |
| `OnBlockAnimEffect` | `(p, perm, block, AE, a)` | 面子动画效果收集 |
| `OnSelfEffect` | `(p, perm, E, a)` | 遗物自身计分阶段 |
| `OnRoundEndEffect` | `(p, perm, AE, a)` | 关末效果收集；处理可能为空的 perm |

像二号硬币这样，只想让“本次打出的牌”触发时，记得保留 `p:Selecting(tile)`。如果只判断点数，之前已经落定的二也可能再次触发。另外，游戏中的重触发机制也会影响调用次数，`OnSelfEffect` 并不是整局只执行一次。

## 4. 做一个会成长的遗物

现在来看练习玉。我们希望它第一次触发时乘1.0番，第二次乘1.1番，之后继续成长。示例用 `plays` 记录触发次数，并把它存成字符串。

说明函数只读取次数，计算出当前倍率，再显示给玩家。真正计分时，`OnSelfEffect` 先添加当前倍率的效果，再添加一个 `SimpleEffect`；等这个效果实际执行，才把次数加一。这样，反复打开图鉴查看说明就不会让遗物偷偷成长了。

`Serialize()` 和 `Deserialize(string)` 会读写 Lua 遗物保存数据的字典。完整游戏的续局还会经过其他存档步骤，所以做好之后，记得实际保存、退出游戏，再重新进入检查一次。

还要注意，注册表里保存的是同一个遗物对象，玩家获得它时不会重新复制一份 Lua 对象。如果要做多个互不影响的实例，或者分别记录多个玩家的数据，就不能只用同一个 ID 下的一份状态。`OnSubscribeToPlayer` 在读档或重新绑定时也可能调用，清零成长次数应放在合适的重置回调里，不要每次绑定都清零。

以后更新模组，给旧存档读取的字段留一个默认值，例如 `tonumber(a:GetDataOrDefault("level", "0")) or 0`。如果更换了字段名，还需要在读取时处理旧数据。已经发布的遗物尽量保留原来的 ID；只是想换个显示名字，修改语言文件就可以了。

## 5. 进入游戏检查效果

先给二号硬币准备二和非二的牌，分别测试本次打出和之前已经落定的情况。再拿到练习玉，连续计分两次，看看倍率是否依次为×1.0和×1.1，反复查看说明时是否保持不变。

最后试一下保存后退出续局、移除遗物和重新开局：成长数值应该按设计保留，移除后不再触发，新局则重新归零。这些都确认之后，才能说明遗物的注册、效果和存档配合正常。
