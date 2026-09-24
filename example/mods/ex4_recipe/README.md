# ex4_recipe

## 安装后试试看

先退出游戏，把本文件夹放进 `Aotenjo_Data/StreamingAssets/mods/`。放好后，应该能直接找到 `mods/ex4_recipe/modinfo.json`，然后重新启动游戏。

依次输入 `give tutorial_recipe:copper_token` 和 `give tutorial_recipe:silver_token`，拿到铜代币与银代币。它们应该被消耗，合成出计分时加5番的双代币。三件遗物都在这个模组里，不用另外安装其他示例。

想看看它是怎么写的，可以打开 `script/init.lua`，对照下面的教程修改。每次改完都要完整重启游戏。示例默认没有打开调试控制台，需要使用测试指令时，先按测试页的说明开启；效果检查和存档测试也放在那一页。

[中文教程](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/recipes.md) · [测试与排错](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/testing.md)

## Install and verify

Close the game. Place this folder directly in `Aotenjo_Data/StreamingAssets/mods/`, with `modinfo.json` immediately inside it, then restart. Give `tutorial_recipe:copper_token`, then `tutorial_recipe:silver_token`; the inputs are consumed and Double Token grants +5 Fan when scoring. No other example is required.

Main entry: `script/init.lua`. Fully restart after edits. The online handbook explains the test console, expected outcomes, and save testing. Debug console access is disabled by default in these examples.

[English guide](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/recipes.md) · [Testing and troubleshooting](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/testing.md)

## 修改为自己的模组 / Make it yours

准备改成自己的模组时，先换上自己的 modID，再一起修改 Lua 内容 ID、模块前缀、语言键和图片文件名，让它们互相对应。ID 尽量保持稳定，版本号填在清单的 version 中即可。改了效果之后，也记得更新中英文说明。

Change modID, Lua content IDs, module prefixes, bilingual keys, and sprite filenames together. Keep IDs stable and version-free. Update descriptions whenever mechanics change.

图片沿用本仓库原有公开示例，原有许可范围保持不变。Artwork is reused from this repository's original public examples; no new license is implied.
