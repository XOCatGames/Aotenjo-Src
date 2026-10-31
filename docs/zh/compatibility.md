# 填写模组支持的游戏版本

[目录](README.md) · [English](../en/compatibility.md) · [快速上手](quickstart.md)

在模组根目录的 `modinfo.json` 中填写 `supportedGameVersions`。每一项用 `minInclusive` 和 `maxInclusive` 声明一个范围，两个端点都包含。只有一个支持版本时，把两端写成相同值。

## 完整示例：支持一个版本

下面以 `0.8.0.0-PRE1` 为例；制作自己的模组时，替换成你实际测试的游戏版本。

```json
{
  "modID": "example.content",
  "name": "Example Content",
  "version": "1.2.0",
  "artifactApiVersion": 2,
  "supportedGameVersions": [
    { "minInclusive": "0.8.0.0-PRE1", "maxInclusive": "0.8.0.0-PRE1" }
  ],
  "author": "Your name",
  "description": "Example mod"
}
```

| 字段 | 填什么 |
| --- | --- |
| `version` | 模组自身的版本，随你的模组更新递增 |
| `supportedGameVersions` | 支持的游戏版本范围，至少填写一项 |
| `artifactApiVersion` | 遗物接口版本；使用当前运行实例接口时填写整数 `2`，详见 [API v2 迁移说明](../ArtifactRuntimeApiV2.md) |

当前下载示例把支持范围固定在本次源码配置版本 `0.8.0.0-PRE1`。如果安装的游戏版本不同，先按[测试与发布](testing.md)验证需要的接口和效果，再调整范围。

## 连续范围与多个范围

下面展示范围写法：第一项从 `0.7.1.0-PRE8` 包含到 `0.7.1.0` 正式版；第二项仅包含 `0.8.0.0-PRE1`。把这段字段合入你的 `modinfo.json`，并按实际测试结果选择范围。

```json
{
  "supportedGameVersions": [
    { "minInclusive": "0.7.1.0-PRE8", "maxInclusive": "0.7.1.0" },
    { "minInclusive": "0.8.0.0-PRE1", "maxInclusive": "0.8.0.0-PRE1" }
  ]
}
```

当前游戏版本落入任意一项即可；每一项本身都必须合法。上例跳过了 `0.7.2.0-PRE1` 等中间版本。

## 版本号格式与比较规则

- 支持三段或四段数字，例如 `0.7.1` 等价于 `0.7.1.0`。
- 先比较数字主体，再比较预览编号。同一主体中，`PRE8 < PRE9 < PRE10 < 正式版`；`0.7.1.0-EA` 等价于 `0.7.1.0`。
- 正式版的单版本范围不包含该版本的 `PRE` 预览版；需要支持预览版时，把下界写成对应的 `PRE` 版本。
- 构建后缀 `-debug` 和 `-console-only` 不影响比较，例如 `0.8.0.0-PRE1-debug` 等价于 `0.8.0.0-PRE1`。通常填写基础游戏版本即可。
- 两端都要填写具体版本；不支持 `*`、`>=`、`^`、`~`、`v` 前缀、空端点或其他自定义后缀。

## 修改后如何生效

保存 `modinfo.json` 后完整退出并重启游戏，在模组管理器里查看状态。缺少或留空 `supportedGameVersions`、非法版本、下界大于上界、当前版本不在范围内，以及重复 `modID`，都会在加载脚本、翻译和贴图之前阻止模组加载。

兼容性检查保留玩家原来的启用选择；版本重新兼容后会按原选择加载。手动启停模组同样需要重启游戏。

实现依据：[清单字段](../../src/API/Mod/Mod.cs) · [版本解析与范围比较](../../src/API/Mod/GameVersion.cs) · [加载前检查](../../src/API/Mod/ModCatalog.cs)。
