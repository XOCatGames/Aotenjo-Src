# 图片、清单与双语文本

[目录](README.md) · [English](../en/assets.md)

## modinfo.json

写明 `modID`、`name`、`version`、`author`、`description` 五个字符串。`modID` 建议使用唯一 ASCII 前缀，例如 `alice_lucky_tiles`；它参与图片命名空间，改它时要一起改 Lua ID 和文本键。文件夹名称不自动成为 modID。不要把版本号写进稳定的内容 ID。

`workshopId` 是更新自己已有 Workshop 项目的可选字符串，首次发布省略。`modDir`、`isFromWorkshop`、`itemUrl` 是运行时数据，不必写入；清单没有依赖解析器、版本约束或启用开关。需要另一模组时在说明中明确依赖并在注册前检查它，而不是虚构 `dependencies` 功能。

## 自定义图标

普通资源加载器只扫描以下四个目录的直接子文件 `*.png`，不递归扫描子目录：

```text
texture/artifact/
texture/tile_material/
texture/tile_style/
texture/tile_mask/
```

统一注册为 `类型:modID:不带扩展名的文件名`。普通图标使用 Point 过滤、Clamp、100 pixels-per-unit、中心 pivot。PNG 保留透明背景，建议沿用示例的像素尺寸与轮廓。不要把文件夹误写成 `textures`。`tile_style` 能加载图片，不等于为游戏自动创建新的字体机制。

两个模组不要共用 modID。同一对象的注册名、图片路径、语言键必须一起改；忘记改任一处，会出现默认占位图或未翻译的键名。示例所用图片来自仓库原有公开示例，见[图片来源](../../example/ARTWORK.md)。

## lang 文件

`lang/zh-CN.json` 与 `lang/en-US.json` 是扁平的“键 → 字符串”字典，不是嵌套对象，也不使用每种语言一个顶层字段。UTF-8，不允许 JSON 注释或尾逗号。目前加载器识别：`zh-CN`、`en-US`、`zh-TW`、`ja-JP`、`ko-KR`、`de-DE`、`fr-FR`、`es-ES`、`ru-RU`。不要把中文文件命名成 `zh-Hans.json`。

加载器把每个语言文件的键同时合并进 GameTable 与 YakuInfo；重名键会覆盖已有值。因此全部自定义键加前缀。为已有内置语言提供翻译最稳妥；仅放一个全新地区码文件不能保证游戏的语言选择 UI 支持它。

| 内容 | 默认键 |
| --- | --- |
| 藏品 | `artifact_<完整藏品ID>_name`、`artifact_<ID>_description` |
| 商店专用藏品描述 | `artifact_<ID>_description_inshop`，没有时回退普通描述 |
| 牌材质 | `tile_<带_material的regName>_name`、`..._description`、`..._name_short` |
| 材质组 | `material_set_<组regName>_name` |
| 自定义番种 | `yaku_custom_yaku:<裸ID>_name`、`..._description`、`..._romaji_name` |
| 自定义 TextEffect/SimpleEffect | 自己的唯一键，例如 `tutorial_artifact_grow` |

`loc(key)` 返回该语言文本。动态描述可用 `string.format(loc(key), number)`，Lua 占位符用 `%.1f` / `%d`；这和 C# 文案的 `{0}` 不是同一种格式。尽量不写 `WithName`，让默认本地化负责名字；藏品的 `WithName` 没有 localizer 参数。

示例初次只需中英两个文件；若没有当前语言翻译，游戏可能回退或显示键名，不能假定一定自动用英文。验收时实际切换简体中文和英文，核对相同内容、数值和标点。修改语言文件后完整重启。
