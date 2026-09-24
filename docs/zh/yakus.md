# 自定义番种

[目录](README.md) · [English](../en/yakus.md) · [全二完整示例](../../example/mods/ex2_pattern) · [实现](../../src/API/Yaku/CustomYakuBuilder.cs)

番种由纯判定函数决定是否成立，再由游戏计算番数。打开示例 `script/init.lua`：它检查 `perm:ToTiles()` 中至少有一张牌且全部为数牌二，空组合返回 false。没有修改玩家、调用随机数或发放奖励。

## 注册函数的十个参数

`CustomYakuBuilder.RegisterCustomYaku(...)` 返回 `YakuType`。

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

数组写法见[Lua 桥接](lua-bridge.md)。无继承传空 `string[]`，不要传 nil。注册依赖 `RegisterManager` 已初始化；在模组 `init()` 中注册。ID 重复会抛异常，完整重启后再测试。

前四包为 **0 风、1 林、2 火、3 山**。完整列表见[番种包目录](../../reference/catalogs.md)。`groups` 与包索引无关；例如 standard 范围会被 extended 范围继承。套组是否接受该范围仍决定能否显示/计分。

## ID 和翻译

返回对象的 `ToString()` 是 `custom_yaku:tutorial_yaku:all_twos`。三个文本键：

```text
yaku_custom_yaku:tutorial_yaku:all_twos_name
yaku_custom_yaku:tutorial_yaku:all_twos_description
yaku_custom_yaku:tutorial_yaku:all_twos_romaji_name
```

不要把完整 ID 再传给 `RegisterCustomYaku` 或 `YakuType(string)`，否则重复添加前缀。引用已注册自定义番种用其返回对象或完整字符串；内置番种用 [FixedYakuType](../../src/HandAndTile/Yaku/FixedYakuType.cs) 的枚举名称，例如 `PingHu`。

## 继承不是调用父判定

`A.includedYakus = [B]` 表示 A 继承 B。A 成立时，普通计分展示移除已成立的直接、间接下级番种。判定依然各自运行，所以 A 的条件要实际蕴含 B。升级的继承由 `SkillSet.CalculateInheritedFan` 处理，不能把 B 的完整番数机械地加到 A.baseFan。

先注册下级再注册上级，也可以注册完之后追加：

```lua
-- advanced 与 prerequisite 都是已经注册的 YakuType。
local inherited = CS.System.Array.CreateInstance(typeof(CS.Aotenjo.YakuType), 1)
inherited[0] = prerequisite
CS.Aotenjo.CustomYakuBuilder.AddInheritanceRelation(advanced:ToString(), inherited)
```

第一个参数是被修改的继承者，支持内置或自定义完整 ID。此函数合并并去重直接关系，不负责注册。禁止自环、循环与多路径重复祖先：递归计分没有通用的循环/祖先去重保护。用 `YakuTester.IncludeYaku(advanced, prerequisite)` 检查方向。

## 验证

用标准范围套组开始新局，开启测试控制台：`upgradeYaku custom_yaku:tutorial_yaku:all_twos 1` 为等级**增加**1，不是设为1。设置并打出二的面子，应看到全二；换入非二或字牌则不成立。早期不完整组合番数可能小于8，这是大小缩放。再检查火包抽取、例牌、双语文本、继承后的隐藏和升级贡献。具体测试矩阵见[测试页](testing.md)。
