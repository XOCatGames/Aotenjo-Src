# 自定义番种

[目录](README.md) · [English](../en/yakus.md) · [全二完整示例](../../example/mods/ex2_pattern) · [实现](../../src/API/Yaku/CustomYakuBuilder.cs)

接下来我们给游戏添加一个新的番种：“全二”。它的条件很简单，组合里至少有一张牌，并且每张牌都是数牌二。

先打开完整示例中的 `script/init.lua`。其中的判定函数会通过 `perm:ToTiles()` 取出组合里的牌，再逐张检查。全部符合条件就返回 true，否则返回 false；如果组合里一张牌都没有，也返回 false。这个函数只负责判断是否成立，番数由游戏计算，所以这里不用修改玩家数据、抽随机数或发奖励。

## 1. 注册一个新的番种

我们使用 `CustomYakuBuilder.RegisterCustomYaku(...)` 注册番种，注册成功后会得到一个 `YakuType` 对象。这个函数的参数比较多，可以对着“全二”示例逐个看：

| 顺序 | 参数 | 全二示例 | 含义 |
| --- | --- | --- | --- |
| 1 | `id: string` | `tutorial_yaku:all_twos` | 唯一裸 ID；不加 `custom_yaku:` |
| 2 | `baseFan: int` | 8 | 完整番数 |
| 3 | `growthFactor: double` | 1.5 | 组合未完整时的大小缩放因子，不是升级倍率 |
| 4 | `levelingFan: double` | 3 | 等级成长增量 |
| 5 | `predicate(perm, player): bool` | 检查所有牌 | 只读；被频繁调用 |
| 6 | `includedYakus: string[]` | 空数组 | 被当前番种继承的番种 |
| 7 | `groups: string[]` | standard, galaxy | 可用番种范围，不是麻将套组名称的任意别名 |
| 8 | `yakuCategories: int[]` | 2 | 已存在的番种包列表索引，从0开始 |
| 9 | `rarity: Rarity` | COMMON | 加入该包的对应稀有度池 |
| 10 | `exampleTiles: string` | `222m222p222s222m22p` | 图鉴例牌；字符串本身不会证明判定正确 |

这里的 `string[]` 和 `int[]` 是 C# 数组，具体写法在 [Lua 与游戏对象](lua-bridge.md)中有说明。如果暂时不需要继承其他番种，就像示例一样传入空的 `string[]`，不要写成 nil。

把注册代码放在模组的 `init()` 中，让它在 `RegisterManager` 初始化之后执行。每个番种的 ID 都要唯一，重复注册会报错；修改代码后，完整重启游戏再测试。

番种范围和番种包是分别设置的。`groups` 决定它属于哪些番种范围，例如 standard 会被 extended 范围继承；使用的套组是否接受这个范围，会影响番种的显示和计分。

`yakuCategories` 则决定把它放进哪些已有的番种包。前四个包是 **0 风、1 林、2 火、3 山**，所以示例里的 `2` 表示火包。其他包的索引可以查[番种包目录](../../reference/catalogs.md)。

## 2. 给番种添加名字和说明

注册时，我们填写的是 `tutorial_yaku:all_twos`。游戏会自动给它加上 `custom_yaku:` 前缀，返回对象的 `ToString()` 就是 `custom_yaku:tutorial_yaku:all_twos`。

接着在 `lang/zh-CN.json` 和 `lang/en-US.json` 中，为下面三个键填写名称、说明和罗马字名称。完整示例已经放好了对应文本，可以直接对照修改：

```text
yaku_custom_yaku:tutorial_yaku:all_twos_name
yaku_custom_yaku:tutorial_yaku:all_twos_description
yaku_custom_yaku:tutorial_yaku:all_twos_romaji_name
```

以后引用这个番种时，可以使用注册返回的对象，或者它的完整 ID。不过，`RegisterCustomYaku` 和 `YakuType(string)` 本身会添加前缀，传给它们时仍要使用未加 `custom_yaku:` 的 ID。引用内置番种则使用 [FixedYakuType](../../src/HandAndTile/Yaku/FixedYakuType.cs) 中的名称，例如 `PingHu`。

## 3. 添加番种继承

等自己的番种能正常判定之后，还可以给它添加继承关系。`A.includedYakus = [B]` 表示 A 继承 B：A 成立时，普通计分展示会移除已经成立的 B，以及它下面的下级番种。

这里有一点容易弄混：设置了继承关系，两个番种的判定函数仍然各自运行。所以设计条件时，要保证 A 成立确实意味着 B 也成立。升级带来的继承番数由 `SkillSet.CalculateInheritedFan` 处理，不需要把 B 的完整番数直接加进 A 的 `baseFan`。

注册时先写下级，再写继承它的上级。也可以等两者都注册好后，用下面的方法追加关系：

```lua
-- advanced 与 prerequisite 都是已经注册的 YakuType。
local inherited = CS.System.Array.CreateInstance(typeof(CS.Aotenjo.YakuType), 1)
inherited[0] = prerequisite
CS.Aotenjo.CustomYakuBuilder.AddInheritanceRelation(advanced:ToString(), inherited)
```

第一个参数填写要增加继承关系的番种，可以是内置番种，也可以是自定义番种的完整 ID。这个函数只合并已有番种的直接继承关系，并去掉其中的重复项，不会帮我们注册新番种。

设置时避免自己继承自己、彼此循环继承，或通过多条路径重复继承同一个祖先。当前递归计分没有统一处理这些情况。拿不准方向时，可以用 `YakuTester.IncludeYaku(advanced, prerequisite)` 检查。

## 4. 进入游戏试试看

重启游戏，用标准范围的套组开始一个新局。开启测试控制台后，输入 `upgradeYaku custom_yaku:tutorial_yaku:all_twos 1`，给全二增加1级。注意，这条指令是在现有等级上加1，多输入一次就会再加1。

接着打出全部由二组成的面子，应该能看到全二成立；换入其他数牌或字牌，则应该不成立。前期组合还不完整时，得到的番数可能小于8，这是大小缩放的结果。

最后再看看火包里能否抽到它，图鉴例牌和中英文文本是否正确。如果加了继承关系，也要试一下下级番种的隐藏和升级贡献。具体操作可以对照[测试页](testing.md)。
