# Aotenjo 模组开发手册 · Modding Handbook

欢迎，各位挖井人！这份教程会从新建模组文件夹开始，带大家制作自己的遗物、番种、牌材质和材质包。只要了解 Lua 的基本语法，就可以跟着示例一步步尝试。
Start with basic Lua syntax, install a working mod, then create your own rules and visuals.

**[中文手册](docs/zh/README.md) · [English handbook](docs/en/README.md) · [完整示例 / Complete examples](example/README.md)**

| 我想做 / I want to… | 中文 | English | 可安装示例 / Installable example |
| --- | --- | --- | --- |
| 从零开始 / Start from scratch | [快速上手](docs/zh/quickstart.md) | [Quick start](docs/en/quickstart.md) | [Hello](example/mods/00_hello) |
| 填写兼容版本号 / Declare supported versions | [版本兼容](docs/zh/compatibility.md) | [Version compatibility](docs/en/compatibility.md) | `modinfo.json` 中的 / field `supportedGameVersions` |
| 创建遗物、添加成长效果 / Add artifacts and persistent growth | [自定义遗物](docs/zh/artifacts.md) | [Artifacts](docs/en/artifacts.md) | [Artifacts](example/mods/ex1_artifact) |
| 自定义遗物附魔 / Custom artifact enchantments | [遗物词条（开发中）](docs/zh/artifact-affixes.md) | [Artifact affixes (in development)](docs/en/artifact-affixes.md) | 在已有模组中注册并赋予 / Register and apply in an existing mod |
| 判断新番种 / Recognize a new yaku | [番种](docs/zh/yakus.md) | [Yakus](docs/en/yakus.md) | [All Twos](example/mods/ex2_pattern) |
| 添加有机制的牌体 / Add tile materials with mechanics | [牌材质](docs/zh/materials.md) | [Materials](docs/en/materials.md) | [Gemstones](example/mods/ex3_tile_material) |
| 按状态切换牌体图片、装饰和阴影 / Change body sprites, ornaments, and shadows by state | [动态牌体（仅 0.7.1.0+）](docs/zh/materials.md) | [Dynamic bodies (0.7.1.0+ only)](docs/en/materials.md) | 在已有模组中添加 / Add to an existing mod |
| 注册可悬浮解释的术语 / Register hoverable glossary terms | [术语（仅 0.7.1.0+）](docs/zh/terms.md) | [Terms (0.7.1.0+ only)](docs/en/terms.md) | 在已有模组中添加 / Add to an existing mod |
| 合成遗物 / Combine artifacts | [合成配方](docs/zh/recipes.md) | [Recipes](docs/en/recipes.md) | [Tokens](example/mods/ex4_recipe) |
| 替换图片、调整牌面颜色 / Replace textures and recolor faces | [材质包](docs/zh/textures.md) | [Texture packs](docs/en/textures.md) | [Texture pack](example/mods/ex5_texture_pack), [colors](example/mods/ex6_face_colors) |
| 添加新的可选语言 / Add a selectable language | [添加语言](docs/zh/localization.md) | [Add a language](docs/en/localization.md) | 纯 JSON 模板 / JSON-only template in the guide |
| 调试、翻译、发布 / Debug, translate, publish | [测试与发布](docs/zh/testing.md) | [Testing and publishing](docs/en/testing.md) | [验收步骤 / Acceptance](docs/VALIDATION.md) |

当前同步来源 / Current publication provenance: [源码范围与历史](docs/SOURCE.md) · [提交与内容散列 / Commit and content hashes](reference/provenance.json).

维护源位于 Aotenjo-Unity 的 `Docs/Modding/Handbook`；源码与手册在 `[Sync]` 时一起发布。详见[维护方式 / Maintaining this handbook](docs/MAINTAINING.md)。
The authoring source is `Docs/Modding/Handbook` in Aotenjo-Unity. Source and handbook are published together on `[Sync]`.

**新接口版本要求：术语注册和四个动态牌体外观回调（`WithSpriteID`、`WithSpriteNamespaceID`、`WithOrnamentSpriteID`、`WithShadowID`）仅在 0.7.1.0 及之后版本可用，0.7.0.x 不支持。** 这些接口按 `dev-0.7.1.0` 补充，来源范围见上面的说明。
**New API version requirement: glossary registration and the four dynamic body appearance callbacks (`WithSpriteID`, `WithSpriteNamespaceID`, `WithOrnamentSpriteID`, `WithShadowID`) are available only in 0.7.1.0 and later, not in 0.7.0.x.** These supplements come from `dev-0.7.1.0`; see the provenance scope above.

制作模组时，从教程和 `example/` 中的示例开始就可以；`src/` 留作查阅游戏逻辑，不需要复制到游戏里，也不能单独用来构建完整游戏。教程参考本次同步的源码编写，你安装的游戏版本可能稍旧，可以先运行 Hello 示例，再按[测试与排错](docs/zh/testing.md)里的步骤检查所需功能。

`src/` is a gameplay source reference, **not a standalone game project or an installable mod**. Do not copy it into your game. The handbook targets this source snapshot; your installed game may be older. Start with Hello and use the [compatibility checks](docs/en/testing.md) before adopting newer features.

维护者 / Maintainers: `python tools/validate.py`, `python tools/package_examples.py`. Lua scenario tests: `python tools/run_lua_tests.py --library /path/to/xlua.dll` or `lua tools/tests/examples.lua`. See [validation scope](docs/VALIDATION.md). The repository preserves its existing licensing status; no new license for the game's source or artwork is implied.
