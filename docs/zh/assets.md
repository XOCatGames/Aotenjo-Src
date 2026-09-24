# 给模组添加图片和中英文文本

[目录](README.md) · [English](../en/assets.md)

前面的示例里，我们已经用到了 `modinfo.json`、`texture` 和 `lang`。这一篇把它们的填写规则放在一起，方便大家换上自己的图片、名字和说明。

## 1. 填写模组信息

打开模组根目录下的 `modinfo.json`，填写 `modID`、`name`、`version`、`author`、`description`，五项都使用字符串。模组名字、版本、作者和介绍会让其他玩家更容易了解你做的内容。

`modID` 建议用英文字母、数字和下划线组成一个自己的前缀，例如 `alice_lucky_tiles`。它会参与图片的注册名称，所以改动时，要把 Lua 中的 ID 和语言键一起改好。模组文件夹叫什么，并不会自动改变 `modID`；版本号填在 `version` 里即可，内容 ID 尽量保持不变。

第一次发布时，可以不写 `workshopId`。以后更新自己已有的 Workshop 项目，再把对应的项目 ID 作为字符串填进去。`modDir`、`isFromWorkshop` 和 `itemUrl` 由游戏运行时填写，不需要自己设置。

当前清单没有自动处理依赖、版本约束或启用开关的功能。如果模组需要另一个模组，记得在说明里写清楚，并在注册内容前检查它是否存在；单独添加 `dependencies` 字段不会自动安装或加载依赖。

## 2. 放入自己的图片

以遗物为例，把 PNG 放进 `texture/artifact/`，游戏就会按模组 ID 和文件名注册这张图片。普通资源支持下面四个目录，图片要直接放在对应目录中，不能再套一层子文件夹：

```text
texture/artifact/
texture/tile_material/
texture/tile_style/
texture/tile_mask/
```

图片的完整注册名是 `类型:modID:不带扩展名的文件名`。注意文件夹叫 `texture`，最后没有 s；这里读取的是 `*.png` 文件。

普通图标会使用 Point 过滤、Clamp、100 pixels-per-unit 和居中的 pivot。自己画图时，可以先沿用示例的像素尺寸和轮廓，并保留透明背景。`tile_style` 也能加载图片，不过只放一张图片，还不会给游戏创建新的字体玩法。

复制示例制作自己的模组时，先换一个独立的 `modID`，再一起检查内容 ID、图片路径和语言键。如果漏改了其中一处，游戏可能显示占位图，或者直接显示一串未翻译的键名。教程图片来自本仓库原有的公开示例，具体说明见[图片来源](../../example/ARTWORK.md)。

## 3. 添加中文和英文

在模组根目录下新建 `lang` 文件夹，再分别创建 `zh-CN.json` 和 `en-US.json`。每个文件里直接填写“键名 → 对应文本”，照着快速上手里的 JSON 格式就可以，不用再套一层语言名或其他对象。

文件用 UTF-8 保存，JSON 中不要加注释，最后一项后面也不要留逗号。目前加载器识别 `zh-CN`、`en-US`、`zh-TW`、`ja-JP`、`ko-KR`、`de-DE`、`fr-FR`、`es-ES`、`ru-RU`，简体中文文件要叫 `zh-CN.json`，不能换成 `zh-Hans.json`。

游戏会把这些文本同时加入 GameTable 和 YakuInfo，重名的键会覆盖原来的内容。因此，自己新增的键也要带上模组前缀。先给游戏已经支持的语言添加翻译即可；放入一个全新的地区码文件，不一定会让语言选择界面多出这个选项。

不同内容默认使用的键名如下：

| 内容 | 默认键 |
| --- | --- |
| 遗物 | `artifact_<完整遗物ID>_name`、`artifact_<ID>_description` |
| 商店专用遗物描述 | `artifact_<ID>_description_inshop`，没有时回退普通描述 |
| 牌材质 | `tile_<带_material的regName>_name`、`..._description`、`..._name_short` |
| 材质组 | `material_set_<组regName>_name` |
| 自定义番种 | `yaku_custom_yaku:<裸ID>_name`、`..._description`、`..._romaji_name` |
| 自定义 TextEffect/SimpleEffect | 自己的唯一键，例如 `tutorial_artifact_grow` |

## 4. 让说明显示当前数值

如果遗物会成长，可以在说明回调中用 `loc(key)` 取出当前语言的文本，再用 `string.format(loc(key), number)` 把数值填进去。Lua 中，`%.1f` 可以显示一位小数，`%d` 用来显示整数；不要把这里的占位符写成 C# 文案使用的 `{0}`。

普通名称直接交给语言文件处理就好，一般不用写 `WithName`。如果确实需要自定义名称回调，记得它没有 localizer 参数。

文本填好后，完整重启游戏，分别切换简体中文和英文，看看名字、说明中的数值和标点是否正确。先做好这两种语言就可以；缺少其他语言的翻译时，游戏可能回退，也可能显示键名，不一定会自动显示英文。
