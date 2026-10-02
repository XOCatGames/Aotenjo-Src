# 用 Lua 注册术语

[目录](README.md) · [English](../en/terms.md) · [图片与本地化](assets.md)

**版本要求：本篇的 `LuaTermBuilder.Register` 仅在青天井 0.7.1.0 及之后版本可用；0.7.0.x 不支持。**

术语可以在遗物、番种和牌体等描述中标色。把鼠标移到术语上，游戏会显示它的标题和解释。
我们以“过载”为例，为已有模组补上术语；不需要制作 Unity `.asset`。

## 1. 在 init.lua 注册

打开模组的 `script/init.lua`，把注册语句放进已有的 `init()` 函数。没有 `init()` 时，可以这样写：

```lua
function init()
    local ok = CS.Aotenjo.LuaTermBuilder.Register("tutorial_terms_overload", "purple")
    if not ok then
        print("过载术语注册失败，请检查日志里的 [LuaTermBuilder]")
    end
end
```

把 `tutorial_terms` 换成自己模组的前缀。术语 ID 使用下划线，不能照搬其他 Builder 的 `modID:内容名` 写法，因为这里不接受冒号。
`purple` 是紫色，也可以选 `yellow`、`green`、`blue` 或 `red`，沿用当前界面的配色。

## 2. 填写标题和解释

在 `lang/zh-CN.json` 中合并这两个键，保留原来的名称和描述：

```json
{
  "tooltip_tutorial_terms_overload_header": "过载",
  "tooltip_tutorial_terms_overload_content": "本次激活效果翻倍。"
}
```

在 `lang/en-US.json` 中填写对应翻译：

```json
{
  "tooltip_tutorial_terms_overload_header": "Overload",
  "tooltip_tutorial_terms_overload_content": "Double this activation's effect."
}
```

语言文件仍是平铺的 JSON 字典。加载器先加载语言文件，再执行 `init.lua`。
缺少当前语言的键时，沿用现有本地化行为，可能显示原始键名；注册术语不会自动产生翻译。

## 3. 在描述里使用

```text
获得<style="tutorial_terms_overload">过载</style>效果。
```

如果描述来自语言文件，JSON 字符串中的双引号需要转义，例如：

```json
{
  "artifact_tutorial_terms:token_description": "获得<style=\"tutorial_terms_overload\">过载</style>效果。"
}
```

把示例描述键换成自己的遗物或牌体描述键。注册只定义颜色和悬浮链接；显示的词，仍由每种语言的描述决定。
文本对象需要已有术语悬浮处理组件，游戏现有的相关描述可以复用；接口不会给任意新建 UI 自动添加交互。

## 4. 参数和注册规则

`bool CS.Aotenjo.LuaTermBuilder.Register(string id, string color)` **仅在 0.7.1.0 及之后版本可用**。

| 参数或结果 | 规则 |
| --- | --- |
| `id` | 以 ASCII 字母开头，只含 ASCII 字母、数字和下划线，最多 128 个字符。自动转为小写；样式标签和语言键请统一使用小写。 |
| `color` | `yellow`、`purple`、`green`、`blue`、`red`，大小写均可。 |
| 返回值 | 成功为 `true`；失败为 `false`，日志带 `[LuaTermBuilder]` 前缀。 |

重复 ID、已有样式名和基础颜色名都会被拒绝，保留原来的定义。
`pattern_`、`tilemat_`、`tilefont_`、`boss_reversed_effect_` 前缀，以及 `jixiepai`，用于特殊提示，不能注册为普通术语。

请在 `init.lua` 注册，再显示包含新术语的描述。术语只在当前加载会话生效，不写入存档或正式样式资产。
重新初始化模组、退出游戏、退出 Play Mode 或编辑器重载时会清理样式副本；本接口不提供动态解释回调或模组热卸载。

## 5. 重启后检查

1. 确认游戏为 **0.7.1.0 或更新版本**，完整重启以重新加载模组。
2. 打开相关描述，确认术语标色，标签没有显示为原始文本。
3. 鼠标悬浮术语，检查标题和解释。
4. 切换简体中文和英文，重新打开描述与提示，检查翻译。
5. 检查术语后面的文字和外层强调，确认字体、颜色和链接范围没有串到其他文字。

源码：[LuaTermBuilder](../../src/API/Terms/LuaTermBuilder.cs)。版本与源码补充范围见[来源说明](../SOURCE.md)。
