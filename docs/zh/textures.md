# 材质包与牌面配色

[目录](README.md) · [English](../en/textures.md) · [可直接安装的材质包](../../example/mods/ex5_texture_pack) · [配色示例](../../example/mods/ex6_face_colors)

## 不写 Lua 的换图包

完整示例把折纸熊替换成仓库自带的硬币图标，故意选择肉眼明显的变化。它已经附带真实 PNG，不需要自己补图片。

```text
ex5_texture_pack/
├─ modinfo.json
├─ img.png
├─ texture-pack.json
└─ texture_pack/origami_bear.png
```

`texture-pack.json` 全文：

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

复制文件夹到 `StreamingAssets/mods` 并完整重启。日志应有 `Loaded 1 texture pack replacement(s)`。在图鉴找到折纸熊，或测试新局 `give origami_bear`，核对它显示硬币；名称、说明和机制仍是折纸熊。移走文件夹并重启，应恢复原图。成功解析清单不等于目标已在屏幕上命中，必须目视核对。

## 清单字段与目标

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
| 藏品注册名 | `artifact:aotenjo:origami_bear` | 独立图片，可任意尺寸；建议保持比例与像素密度 |
| 内置材质注册名 | `tile_material:aotenjo:plain_material` | 独立 Sprite，不要误写成自定义材质的无后缀形式 |
| 另一个模组的注册名 | `artifact:tutorial_artifact:coin_twos` | 被覆盖模组必须存在 |
| Resources 路径 | 从[贴图目录](../../reference/textures.csv)复制不带扩展名的 `resource_path` | 整张贴图宽高必须一致 |
| Texture2D 名称 | 贴图目录中的 `texture_name` | 整图宽高一致；重名可能影响多个对象 |
| Sprite 名称 | 实际 Sprite 名；有歧义用 `资源路径/Sprite名` | 独立 Sprite；若同名目标已命中整图，不再按 Sprite 替换 |

不要凭汉化显示名猜 target。藏品的 `GetSpriteNamespaceID` 和材质图片规则是依据；特殊物品可覆盖该方法。整图替换保留原有对象和切片坐标，因此 **图集不能改变尺寸或移动格子位置**。未指定的区域也要保留，否则其他切片会损坏。独立 Sprite 不继承原九宫格边框；可拉伸 UI 优先整图替换。贴图目录给出源 PNG 尺寸，若平台导入时缩小了贴图，以日志报告的运行时尺寸为准。

目标字典忽略大小写，但文件路径在部分平台区分大小写。Workshop 先加载，本地后加载，各自按目录路径排序；相同 target 后加载者覆盖前者，日志会报告冲突。混合模组可同时包含 `texture`、`lang`、`script` 和清单。游戏没有热重载。

## 不需要图片的牌面配色

配色示例把 `plain` 字体改成青绿，把 `blue` 改成彩虹。进入新局即可看见普通牌面改变；`setFont 0 blue` 可测试彩虹。修改的是已有字体外观，不改变它的番数效果。`plain` 与 `plain_font` 等价，注册时自动补后缀，同 ID 再注册会覆盖。

所有下列入口都是 `CS.Aotenjo.TileFaceMaterialRegistry` 的静态方法；参数为 string/bool/number，注册与取消方法返回 bool。

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

建议 strength、threshold、饱和度和亮度用0到1，pixels 用1到4，hueRange 用-1到1，负数反向。渐变按每个 Sprite 的 UV 范围计算。仅注册 `my_font` 样式不会让 `TileFont.GetFont("my_font")` 成功；当前没有新字体注册 Builder。与三维眼镜等运行时配色效果组合后应再次目视验证。
