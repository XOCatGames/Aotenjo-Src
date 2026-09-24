# 源码基准与范围 / Source baseline and scope

- Date / 日期: 2026-09-24.
- Public source / 公开源码: [406dba6623ad528224eac65f5d894269907681a8](https://github.com/XOCatGames/Aotenjo-Src/commit/406dba6623ad528224eac65f5d894269907681a8).
- Upstream working checkout base / 上游工作区基线: `c66f4500de0d2d19d627cc9d2455832ba7ff66c4`.
- 638 files copied from `Assets/LogicScripts` into `src`, excluding Unity `.meta`, `.git*`, and `.DS_Store`, following the existing public sync scope.
- The current upstream working tree omits `Assets/LogicScripts/Utils/Constants.cs`; the public snapshot reflects that omission. No unrelated localization edits were included. The upstream worktree was not committed or modified by this documentation task.

638个源码文件同步自当前 LogicScripts，遵循原有同步过滤。工作区已有的 Constants.cs 缺失反映在本次公开快照中；未带入无关本地化改动，也未提交或修改上游工作区。因此此仓库依然是逻辑参考，不能单独构建游戏。

The handbook additionally traces the actual mod loader, localization loader, texture manager, face-color registry, console parsers, and Workshop uploader at this baseline. These runtime integration files are outside the public LogicScripts export. Their paths and normalized content hashes are recorded in [provenance.json](../reference/provenance.json); private project files, assets, credentials, and build dependencies are not added here.

文档额外核对了 LogicScripts 以外的加载器、图片、配色、控制台和上传流程；来源路径及哈希见上表链接。生成的[贴图目录](../reference/textures.csv)只列路径、名称和尺寸，不分发游戏贴图。

[source-manifest.json](../reference/source-manifest.json) contains SHA-256 hashes after CRLF→LF normalization, allowing exact content verification across Git line-ending settings. The original source's whitespace is preserved. New handbook/example files are checked separately for whitespace errors.

源码散列使用 CRLF→LF 归一化，以适配 Git 换行设置；源码原有空白保持不变，新文档和示例另行检查。源码快照不是 Steam 发行版本号，未推断最低发行版；请按[中文测试指南](zh/testing.md) / [English testing guide](en/testing.md)验证安装版本。
