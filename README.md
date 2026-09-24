# Aotenjo 模组开发手册 · Modding Handbook

欢迎，各位挖井人！这份教程会从新建模组文件夹开始，带大家制作自己的遗物、番种、牌材质和材质包。只要了解 Lua 的基本语法，就可以跟着示例一步步尝试。
Start with basic Lua syntax, install a working mod, then create your own rules and visuals.

**[中文手册](docs/zh/README.md) · [English handbook](docs/en/README.md) · [完整示例 / Complete examples](example/README.md)**

| 我想做 / I want to… | 中文 | English | 可安装示例 / Installable example |
| --- | --- | --- | --- |
| 从零开始 / Start from scratch | [快速上手](docs/zh/quickstart.md) | [Quick start](docs/en/quickstart.md) | [Hello](example/mods/00_hello) |
| 创建遗物、添加成长效果 / Add artifacts and persistent growth | [自定义遗物](docs/zh/artifacts.md) | [Artifacts](docs/en/artifacts.md) | [Artifacts](example/mods/ex1_artifact) |
| 判断新番种 / Recognize a new yaku | [番种](docs/zh/yakus.md) | [Yakus](docs/en/yakus.md) | [All Twos](example/mods/ex2_pattern) |
| 添加有机制的牌体 / Add tile materials with mechanics | [牌材质](docs/zh/materials.md) | [Materials](docs/en/materials.md) | [Gemstones](example/mods/ex3_tile_material) |
| 合成遗物 / Combine artifacts | [合成配方](docs/zh/recipes.md) | [Recipes](docs/en/recipes.md) | [Tokens](example/mods/ex4_recipe) |
| 替换图片、调整牌面颜色 / Replace textures and recolor faces | [材质包](docs/zh/textures.md) | [Texture packs](docs/en/textures.md) | [Texture pack](example/mods/ex5_texture_pack), [colors](example/mods/ex6_face_colors) |
| 调试、翻译、发布 / Debug, translate, publish | [测试与发布](docs/zh/testing.md) | [Testing and publishing](docs/en/testing.md) | [验收步骤 / Acceptance](docs/VALIDATION.md) |

源码快照 / Source snapshot: [`406dba6`](https://github.com/XOCatGames/Aotenjo-Src/commit/406dba6623ad528224eac65f5d894269907681a8), synchronized on 2026-09-24. See [provenance](docs/SOURCE.md).

制作模组时，从教程和 `example/` 中的示例开始就可以；`src/` 留作查阅游戏逻辑，不需要复制到游戏里，也不能单独用来构建完整游戏。教程参考本次同步的源码编写，你安装的游戏版本可能稍旧，可以先运行 Hello 示例，再按[测试与排错](docs/zh/testing.md)里的步骤检查所需功能。

`src/` is a gameplay source reference, **not a standalone game project or an installable mod**. Do not copy it into your game. The handbook targets this source snapshot; your installed game may be older. Start with Hello and use the [compatibility checks](docs/en/testing.md) before adopting newer features.

维护者 / Maintainers: `python tools/validate.py`, `python tools/package_examples.py`. Lua scenario tests: `python tools/run_lua_tests.py --library /path/to/xlua.dll` or `lua tools/tests/examples.lua`. See [validation scope](docs/VALIDATION.md). The repository preserves its existing licensing status; no new license for the game's source or artwork is implied.
