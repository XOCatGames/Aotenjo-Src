# 材质包与牌面配色

[目录](README.md) · [English](../en/textures.md) · [可直接安装的材质包](../../example/mods/ex5_texture_pack) · [配色示例](../../example/mods/ex6_face_colors)

想给喜欢的遗物换个图标，或者给麻将牌换一套配色，也可以通过模组来做。这一篇先做一个只替换图片的材质包，再试试不用图片的牌面配色。

## 1. 给折纸熊换一张图片

我们先把折纸熊的图标换成二号硬币，方便进游戏后马上看出区别。打开 `ex5_texture_pack`，图片已经放在里面，文件结构如下：

```text
ex5_texture_pack/
├─ modinfo.json
├─ img.png
├─ texture-pack.json
└─ texture_pack/origami_bear.png
```

这个模组不需要 Lua 脚本，只要在 `texture-pack.json` 里告诉游戏“把哪张图换成哪张图”。打开文件，可以看到：

```json
{
  "formatVersion": 1,
  "replacements": [
    {
      "target": "artifact:aotenjo:origami_bear",
      "file": "texture_pack/origami_bear.png",
      "filterMode": "Point",
      "pixelsPerUnit": 100
    }
  ]
}
```

其中 `target` 指定要替换的折纸熊图标，`file` 指向我们放在模组里的图片。`Point` 过滤适合像素图，`pixelsPerUnit` 则按示例先填100。

把整个文件夹复制到 `StreamingAssets/mods`，完整重启游戏。在日志里找到 `Loaded 1 texture pack replacement(s)` 后，打开图鉴看看折纸熊；也可以在测试新局输入 `give origami_bear`，直接拿到它。它应该显示为硬币图标，但名称、说明和效果仍然是折纸熊。

再把模组文件夹移出去，重启游戏，图标应该恢复原样。日志中的加载成功只说明清单读进来了，是否换到了想要的图片，还要进游戏看一眼。

## 2. 换成自己的图片

示例能正常显示之后，就可以把 `texture_pack/origami_bear.png` 换成自己画的图片了。如果还想替换其他物品，在 `replacements` 里继续添加项目即可。各个字段的用法如下：

| 字段 | 类型、默认值 | 规则 |
| --- | --- | --- |
| `formatVersion` | 整数，示例显式写1 | 当前只支持1 |
| `replacements` | 对象数组 | 每项独立处理 |
| `target` | 非空字符串，必填 | 使用下面一种已存在的目标名 |
| `file` | 非空字符串，必填 | 模组内相对路径；PNG/JPG/JPEG；禁止绝对路径和 `../` 越界 |
| `filterMode` | 可省略 | `Point` 像素风；`Bilinear`、`Trilinear`；整图未指定则保留原过滤，独立图默认 Point |
| `pixelsPerUnit` | 数字，默认100 | 仅影响新建 Sprite；非正数回退100 |

| 目标方式 | 示例 | 尺寸规则 |
| --- | --- | --- |
| 遗物注册名 | `artifact:aotenjo:origami_bear` | 独立图片，可任意尺寸；建议保持比例与像素密度 |
| 内置材质注册名 | `tile_material:aotenjo:plain_material` | 独立 Sprite，不要误写成自定义材质的无后缀形式 |
| 另一个模组的注册名 | `artifact:tutorial_artifact:coin_twos` | 被覆盖模组必须存在 |
| Resources 路径 | 从[贴图目录](../../reference/textures.csv)复制不带扩展名的 `resource_path` | 整张贴图宽高必须一致 |
| Texture2D 名称 | 贴图目录中的 `texture_name` | 整图宽高一致；重名可能影响多个对象 |
| Sprite 名称 | 实际 Sprite 名；有歧义用 `资源路径/Sprite名` | 独立 Sprite；若同名目标已命中整图，不再按 Sprite 替换 |

选择 `target` 时，要查图片的实际名称。遗物可以从 `GetSpriteNamespaceID` 查起，牌材质则按相应的图片规则填写；有些特殊遗物会自己设置图片名称，直接用中文显示名通常找不到目标。

如果替换的是一整张图集，游戏仍会按原来的位置切出每个小图，所以 **图集的尺寸和格子位置都要保持原样**。没有修改的区域也要保留，否则其他物品可能一起显示错误。独立 Sprite 替换不会保留原来的九宫格边框，需要拉伸的 UI 图片建议优先使用整图替换。

贴图目录列出的是源 PNG 的尺寸。有的平台导入图片时会缩小它，遇到尺寸不匹配的提示，要以日志中的运行时尺寸为准。

## 3. 和其他模组一起使用

同一个模组里可以同时放 `texture`、`lang`、`script` 和 `texture-pack.json`，因此新增玩法和替换图片也可以一起做。每次修改后，都需要完整重启游戏，当前没有热重载。

游戏先加载 Workshop 模组，再加载本地模组，两边各自按目录路径排序。如果几个模组替换同一个 `target`，后加载的会覆盖前面的，日志中也会提示冲突。目标名称的匹配忽略大小写，不过部分平台上的文件路径区分大小写，文件名和清单最好保持完全一致。

## 4. 给牌面换一种颜色

如果只是想调整牌面的颜色，可以试试 `ex6_face_colors`，不用额外准备图片。这个示例把 `plain` 字体改成青绿色，把 `blue` 字体改成彩虹色。

安装并重启后，开一局游戏就能查看普通牌面的变化。再输入 `setFont 0 blue`，看看第一张手牌上的彩虹效果。配色只影响已有字体的外观，它原来的番数效果仍然保留。

注册时，`plain` 和 `plain_font` 都可以，游戏会自动补上后缀。如果再次注册同一个 ID，新的配色会覆盖之前的设置。

需要更多颜色或深度效果时，可以使用下面这些方法。它们都从 `CS.Aotenjo.TileFaceMaterialRegistry` 调用，参数使用字符串、布尔值和数字；注册或取消设置后，会返回 bool 表示是否成功。

| 方法及参数 | 用途 |
| --- | --- |
| `RegisterHex(fontId, color)` | `#RGB`、`#RGBA`、`#RRGGBB`、`#RRGGBBAA` |
| `RegisterColor(fontId, r, g, b, a)` | 0到1的 RGBA |
| `RegisterOriginal(fontId)` | 保留原色，使用默认深度 |
| `RegisterRainbow(fontId)` | 默认全色谱渐变 |
| `RegisterHexWithDepth(fontId, color, strength, pixels, threshold)` | 纯色和深度 |
| `RegisterColorWithDepth(fontId, r, g, b, a, strength, pixels, threshold)` | 同上，使用 RGBA |
| `RegisterOriginalWithDepth(fontId, strength, pixels, threshold)` | 原色和深度 |
| `RegisterRainbowWithSettings(fontId, hueStart, hueRange, saturation, value, strength, pixels, threshold)` | 色相起点与跨度、饱和度、亮度、深度 |
| `SetSwapRedGreen(fontId, enabled)` | 叠加红绿互换 |
| `SetSwapRedGreenWithSaturation(fontId, enabled, saturation)` | 互换并设置目标饱和度 |
| `SetSwapRedGreenAppearance(fontId, enabled, saturation, brightnessBoost)` | 再控制绿转红的提亮程度 |
| `Unregister(fontId)` | 撤销样式，回到源 Sprite 外观 |

刚开始调整时，可以让 strength、threshold、饱和度和亮度取0到1，pixels 取1到4，hueRange 取-1到1；hueRange 为负数时，渐变方向会反过来。渐变按照每个 Sprite 的 UV 范围计算，可以一边改参数，一边重启查看效果。

这里设置的是已有字体的外观。仅仅注册一个叫 `my_font` 的样式，还不能通过 `TileFont.GetFont("my_font")` 获得新字体，因为目前没有创建新字体的 Builder。最后也可以带上三维眼镜等会改变配色的物品，看看几种效果叠加后的样子是否符合预期。
