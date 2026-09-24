# ex1_artifact

## 安装与验证

退出游戏，把本文件夹直接放到 `Aotenjo_Data/StreamingAssets/mods/`，使下一层就是 `modinfo.json`，完整重启。测试新局中输入 `give tutorial_artifact:coin_twos`，用 `setHand 222m123p` 打出二应每张得2金币。`give tutorial_artifact:practice_jade` 后两次计分应分别乘1.0、1.1番。

主入口：`script/tutorial_artifact/artifacts.lua`。修改后完整重启。控制台开启方法、期望结果与存档测试请阅读在线手册；示例默认不启用调试控制台。

[中文教程](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/artifacts.md) · [测试与排错](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/testing.md)

## Install and verify

Close the game. Place this folder directly in `Aotenjo_Data/StreamingAssets/mods/`, with `modinfo.json` immediately inside it, then restart. In a test run, use `give tutorial_artifact:coin_twos` and `setHand 222m123p`; newly played twos earn 2 coins each. Use `give tutorial_artifact:practice_jade`; the first two scoring triggers multiply Fan by 1.0 and 1.1.

Main entry: `script/tutorial_artifact/artifacts.lua`. Fully restart after edits. The online handbook explains the test console, expected outcomes, and save testing. Debug console access is disabled by default in these examples.

[English guide](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/artifacts.md) · [Testing and troubleshooting](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/testing.md)

## 修改为自己的模组 / Make it yours

修改 modID、Lua内容ID、模块前缀、双语键和图片文件名，保持相互对应。使用稳定ID，不要把版本号写进ID。更改机制后同步更新说明。

Change modID, Lua content IDs, module prefixes, bilingual keys, and sprite filenames together. Keep IDs stable and version-free. Update descriptions whenever mechanics change.

图片复用本仓库既有公开示例；并非新许可授权。Artwork is reused from this repository's original public examples; no new license is implied.
