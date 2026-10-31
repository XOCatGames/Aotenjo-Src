# 自定义遗物词条

[English](../en/artifact-affixes.md) · [目录](README.md)

本接口随遗物词条功能开发，目前位于 `codex/artifact-runtime-instances` 分支。使用前确认安装的游戏包含 `LuaArtifactAffixBuilder`；不能仅凭版本号判断已有此接口。

## 注册与赋予

在模组的 `script/init.lua` 注册定义。注册不会自动生成附魔遗物，也不会加入商店或奖励池。

```lua
local A = CS.Aotenjo
local id = "example.enchants:ember"
assert(A.LuaArtifactAffixBuilder.Create(id)
    :WithNameKey("example_enchants_ember_name")
    :WithDescriptionKey("example_enchants_ember_description")
    :OnSelfEffect(function(player, perm, effects, artifact)
        effects:Add(A.ScoreEffect.AddFu(7, artifact))
    end)
    :BuildAndRegister())
```

在 `lang/zh-CN.json` 添加这两个翻译键（语言文件格式见[图片与双语文本](assets.md)）：

```json
{
  "example_enchants_ember_name": "余烬",
  "example_enchants_ember_description": "此遗物计分时: +7符。"
}
```

在已有的遗物 `OnObtain` 回调或模组玩法回调中，用回调传入的玩家和已持有遗物赋予词条：

```lua
local applied = player:SetCustomArtifactAffix(artifact, "example.enchants:ember")
```

每件遗物只有一个词条，自定义词条与内置词条互相替换。碎裂、临时、正在升级的遗物不能赋予；替换“轻盈”导致栏位不足也会返回 `false`，原词条保持不变。重复赋予同一 ID 保留原实例和成长数据，不重新触发宿主订阅。`WithCanApply(function(player, artifact) ... end)` 可增加条件，此回调只做判定，不修改状态。

ID 使用 `模组命名空间:词条名`，例如 `example.enchants:ember`，两段都以小写字母开头，其余字符允许小写字母、数字、`_`、`.`、`-`，总长最多 128 个字符。名称键必填，描述键或描述回调至少提供一个。`BuildAndRegister()` 成功返回 `true`；无效配置和重复 ID 返回 `false`，已有定义不会被覆盖。发布后保持 ID 稳定。

## 效果时机

| Builder 方法 | Lua 回调参数 | effects 的内容 |
| --- | --- | --- |
| `OnSelfEffect` | `player, perm, effects, artifact` | `Effect` |
| `OnTileEffect` | `player, perm, tile, effects, artifact` | `Effect` |
| `OnBlockEffect` | `player, perm, block, effects, artifact` | `Effect` |
| `OnRoundEndEffect` | `player, perm, effects, artifact` | `IAnimationEffect` |

效果接在宿主遗物对应时机的效果之后，沿用计分和动画流程。碎裂、临时、受压制或因旅行袋等规则失效时，自定义词条不触发。此接口目前不提供自定义售价、栏位规则或自然生成权重。

每件遗物的 `artifact.Affix` 持有独立词条实例，随宿主的 `SubscribeToPlayer/UnsubscribeToPlayer` 订阅／退订 `ArtifactEffectEvent.Self/Tile/Block/RoundEnd`。Builder 签名不变，模组无需额外订阅；覆盖宿主订阅方法时仍须调用 `base`。每件宿主完成本体效果后才发布相应事件，词条只响应自己的宿主，沿用库存订阅顺序。读档先恢复属性，激活宿主才绑定；处理器异常向调用方传播并终止当前效果收集。

内置容量、禁售和快照传递使用实例内的同步查询事件，查询候选产物不触发获取／移除，也不修改实局订阅。幸运通过携带来源的权重事件影响商店抽取。自定义词条仍使用本页列出的 Builder 回调；这些底层查询不增加新的 Lua 栏位或抽池回调。

内置轻盈提供 +1 遗物栏位，并让宿主的番符效果减半，不再扣除小道具栏位。`ScoreEffect` 显示和执行时发布 `ArtifactScoreValueEvent`，轻盈实例只修正自己宿主的值：加符、加番除以 2，乘番为 `1 + (倍率 - 1) / 2`（X2 → X1.5）。模组遗物也应使用 `ScoreEffect` 产生番符效果，避免直接修改累计分数绕过该查询；钱币和成长数据不受此修正影响。

小局开局准备使用独立的 `PlayerRoundEvent.Start.Prepare`，由游戏流程发布；模组不要自行发布此事件或调用开局准备入口。当前版本保存开局准备标记以防重复，不提供旧版词条结算标记迁移。关末词条通过宿主的 `ArtifactEffectEvent.RoundEnd` 追加效果，沿用普通关末流程。

## 成长与存档

`player:GetArtifactAffixId(artifact)` 返回自定义 ID、内置词条的小写英文名，或无词条时的空字符串。原有 `GetArtifactAffix` 仍只返回内置枚举。

词条实例负责自身 ID、名称、描述和成长数据，已有 Player 方法及 Builder 签名保持可用。通过 `player:SetArtifactAffixData` 写入时仍会检查持有关系、合成和待升级限制，并更新计分缓存。缺失内容检查只读取保存快照，旧档字段为空时也不会初始化或改写它们。

使用 `GetArtifactAffixData(artifact, key, fallback)` 和 `SetArtifactAffixData(artifact, key, value)` 保存字符串数据，传入 `nil` 删除该键。数据按玩家、遗物分别保存；不要把成长值放在 Builder 或全局 Lua 变量上。`player:ClearArtifactAffix(artifact)` 清除词条及其数据。

收集效果和生成描述可能用于预览，会重复调用；只向列表追加效果，不在这些回调里直接增加成长。实际修改放在 `SimpleEffect` 中，并检查词条尚未被替换。例如，在 `OnRoundEndEffect` 中追加：

```lua
effects:Add(CS.Aotenjo.SimpleEffect("example_enchants_growth", artifact, function(p)
    if p:GetArtifactAffixId(artifact) ~= "example.enchants:ember" then return end
    local n = tonumber(p:GetArtifactAffixData(artifact, "growth", "0"))
    p:SetArtifactAffixData(artifact, "growth", tostring(n + 1))
end))
```

`WithDescription(function(player, loc, artifact) ... end)` 可以读取成长值生成动态描述；`loc(key)` 使用当前语言，描述回调必须无副作用。上例的 `example_enchants_growth` 是效果文本键，也应写入模组翻译。

词条 ID、所属模组和成长数据随玩家存档保存。遗物升级保留独立快照，升级等待期间拒绝修改词条和数据；正常移除、出售后清除，重新获得时不会继承。合成先排除无词条材料，按不同词条 ID 去重后等概率继承一个：A＋无词条为 A，A＋A 仍为 A，A＋B 各 50%。相同自定义 ID 的成长不同，采用配方材料顺序中的首个来源并深复制，不混合、不叠加；消耗材料后清除其旧数据。抽选先固定并随存档保存，容量或获取条件不满足时保留材料、产物原状态和奖励预约，重试／续局不重抽同一组合。容量计算包含最终轻盈词条、临时栏位及预约的遗物和小道具。缺少定义或所属模组变化会阻止该对局继续及写入，并显示缺失 ID，恢复正确模组后再加载；不会静默删除词条。旧存档不含自定义字段时保持无自定义词条。
