# ex3_tile_material

## 安装与验证

退出游戏，把本文件夹直接放到 `Aotenjo_Data/StreamingAssets/mods/`，使下一层就是 `modinfo.json`，完整重启。新局选教学宝石组，或 `setHand 222m333p` 后 `setMat 0-2 tutorial_gems:ruby`。红宝石计分加20符，经过关末效果时加2金币；蓝宝石计分乘2番。

主入口：`script/init.lua`。修改后完整重启。控制台开启方法、期望结果与存档测试请阅读在线手册；示例默认不启用调试控制台。

[中文教程](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/materials.md) · [测试与排错](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/testing.md)

## Install and verify

Close the game. Place this folder directly in `Aotenjo_Data/StreamingAssets/mods/`, with `modinfo.json` immediately inside it, then restart. Select Tutorial Gemstones for a new run, or use `setHand 222m333p`, then `setMat 0-2 tutorial_gems:ruby`. Ruby adds 20 Fu and earns 2 coins when its round-end effect runs; Sapphire doubles Fan.

Main entry: `script/init.lua`. Fully restart after edits. The online handbook explains the test console, expected outcomes, and save testing. Debug console access is disabled by default in these examples.

[English guide](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/materials.md) · [Testing and troubleshooting](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/testing.md)

## 修改为自己的模组 / Make it yours

修改 modID、Lua内容ID、模块前缀、双语键和图片文件名，保持相互对应。使用稳定ID，不要把版本号写进ID。更改机制后同步更新说明。

Change modID, Lua content IDs, module prefixes, bilingual keys, and sprite filenames together. Keep IDs stable and version-free. Update descriptions whenever mechanics change.

图片复用本仓库既有公开示例；并非新许可授权。Artwork is reused from this repository's original public examples; no new license is implied.
