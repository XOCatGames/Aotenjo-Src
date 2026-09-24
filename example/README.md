# 完整示例 / Complete examples

每个文件夹都能独立安装，包含实际需要的 PNG、清单、脚本和中英文本；纯换图与配色示例按功能省略不需要的文件。只要求基本 Lua 语法，不需要 Unity。

Each folder installs independently with its required PNGs, metadata, scripts, and Chinese/English text. Visual-only examples omit unnecessary files. You do not need Unity.

**[下载全部示例 / Download all examples](https://github.com/XOCatGames/Aotenjo-Src/raw/refs/heads/main/downloads/all-examples.zip)** · [SHA-256](../downloads/SHA256SUMS.txt)

| 模组 / Mod | 内容 / Contents | 源码 / Source | 独立安装包 / ZIP |
| --- | --- | --- | --- |
| Hello | 启动日志 / Startup log | [00_hello](mods/00_hello) | [ZIP](../downloads/00_hello.zip) |
| Artifacts | 条件触发与成长 / Conditions and growth | [ex1_artifact](mods/ex1_artifact) | [ZIP](../downloads/ex1_artifact.zip) |
| All Twos | 自定义番种 / Custom yaku | [ex2_pattern](mods/ex2_pattern) | [ZIP](../downloads/ex2_pattern.zip) |
| Gemstones | 两种牌体与材质组 / Two materials and a set | [ex3_tile_material](mods/ex3_tile_material) | [ZIP](../downloads/ex3_tile_material.zip) |
| Tokens | 完整合成链 / Complete crafting chain | [ex4_recipe](mods/ex4_recipe) | [ZIP](../downloads/ex4_recipe.zip) |
| Texture pack | 无 Lua 换图 / Texture replacement without Lua | [ex5_texture_pack](mods/ex5_texture_pack) | [ZIP](../downloads/ex5_texture_pack.zip) |
| Face colors | 纯色和彩虹 / Solid and rainbow styles | [ex6_face_colors](mods/ex6_face_colors) | [ZIP](../downloads/ex6_face_colors.zip) |

将 ZIP **解压**到 `Aotenjo_Data/StreamingAssets/mods/`，保留各模组文件夹；不要让 `modinfo.json` 多套一层。先单独运行，再一起运行。修改后完整退出重启。相同模组不要同时装本地和 Workshop 两份。

**Extract** into `Aotenjo_Data/StreamingAssets/mods/`, retaining each mod folder. Avoid an extra directory layer above modinfo.json. Test separately before testing together. Restart after every edit. Do not install local and Workshop copies of the same mod simultaneously.

[中文快速上手](../docs/zh/quickstart.md) · [English quick start](../docs/en/quickstart.md) · [验收范围 / Validation scope](../docs/VALIDATION.md) · [图片来源 / Artwork](ARTWORK.md)

These rewritten examples use new `tutorial_*` IDs. Replace the old example folders completely when upgrading; they are teaching projects, not a migration of old demonstration saves. 重写示例使用新的 tutorial 前缀；升级时整体替换旧示例，不承诺迁移旧教学存档。
