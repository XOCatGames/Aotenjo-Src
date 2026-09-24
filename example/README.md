# 完整示例 / Complete examples

这里放好了七个完整示例，大家可以先挑一个感兴趣的装进游戏，再对照教程改成自己的模组。每个文件夹都能单独使用，需要的图片、清单、脚本和中英文文本已经配好；纯换图和配色示例只保留各自需要的文件。了解 Lua 基本语法就可以开始，不需要安装 Unity。

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

下载后，把 ZIP **解压**到 `Aotenjo_Data/StreamingAssets/mods/`，保留每个模组自己的文件夹。比如 Hello 放好后，应该能直接找到 `mods/00_hello/modinfo.json`。先单独运行一个示例，确认正常后再试着一起安装；每次修改都要完整退出并重启游戏。同一个模组安装一份就够了，避免本地和 Workshop 重复加载。

**Extract** into `Aotenjo_Data/StreamingAssets/mods/`, retaining each mod folder. Avoid an extra directory layer above modinfo.json. Test separately before testing together. Restart after every edit. Do not install local and Workshop copies of the same mod simultaneously.

[中文快速上手](../docs/zh/quickstart.md) · [English quick start](../docs/en/quickstart.md) · [验收范围 / Validation scope](../docs/VALIDATION.md) · [图片来源 / Artwork](ARTWORK.md)

These rewritten examples use new `tutorial_*` IDs. Replace the old example folders completely when upgrading; they are teaching projects, not a migration of old demonstration saves. 如果以前装过旧版示例，更新时请用新文件夹整体替换。新版使用 `tutorial_*` 前缀，旧教学存档不保证能继续使用，建议开新局测试。
