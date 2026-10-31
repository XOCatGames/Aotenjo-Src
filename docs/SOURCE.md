# 源码来源与范围 / Source provenance and scope

[首页 / Home](../README.md) · [维护与发布 / Authoring and publication](MAINTAINING.md)

## 当前同步 / Current publication

源码与手册从同一个 Aotenjo-Unity 提交同步。该次提交 SHA、提交日期、配置版本、运行时文件散列以及源码文件数见 [provenance.json](../reference/provenance.json)；[source-manifest.json](../reference/source-manifest.json) 列出 `src` 的完整文件清单及 CRLF→LF 归一化后的 SHA-256。

Source and handbook are exported from the same Aotenjo-Unity commit. The generated provenance records its SHA, commit date, configured version, runtime integration hashes and source count. The source manifest covers every public source file using CRLF→LF-normalized SHA-256 hashes.

导出范围为 `Assets/LogicScripts` → `src`，排除 `.meta`、`.git*`、`.DS_Store`；公开手册来自 `Docs/Modding/Handbook`。`src` 不能单独构建游戏。加载器、本地化、语言界面、图片、配色、控制台和 Workshop 上传等运行时文件在该范围之外，只记录核对用的路径与散列。生成的[贴图目录](../reference/textures.csv)只列路径、名称和尺寸。

The export maps `Assets/LogicScripts` to `src`, excluding `.meta`, `.git*` and `.DS_Store`, and publishes the handbook from `Docs/Modding/Handbook`. This is not a standalone game project. Loader, localization, language UI, texture/color, console and Workshop runtime files outside this scope are represented only by paths and hashes. The texture catalog contains metadata, not game artwork.

本地预演的 `upstream_worktree_dirty: true` 表示有已跟踪文件尚未提交，SHA 仅标识基线；正式同步使用干净的精确提交。`game_version` 是源码配置，不证明此版本已发行。尚未发布的功能按章节中的“下一版本 / Unreleased”说明判断；安装版本的实际能力仍需按[中文](zh/testing.md) / [English](en/testing.md)测试指南验证。

In a local preview, `upstream_worktree_dirty: true` means tracked changes are uncommitted and the SHA identifies only their base. Publication uses a clean exact checkout. `game_version` is configuration metadata, not a release assertion. Follow chapter availability notes and test against the actual installed build.

## 迁入的历史基线 / Imported historical baseline

以下记录描述迁入前的手册，供追溯使用。主仓库中的初始 `reference` 保留此基线；预演及发布时，生成器会替换为当前代码的完整索引与来源记录。

These entries describe the handbook before migration. Its initially imported references retain this baseline; preview and publication regenerate the complete references from current code.

- 2026-09-24：公开提交 [`406dba6623ad528224eac65f5d894269907681a8`](https://github.com/XOCatGames/Aotenjo-Src/commit/406dba6623ad528224eac65f5d894269907681a8)，上游基线 `c66f4500de0d2d19d627cc9d2455832ba7ff66c4`，638 个源码文件；当时工作区缺少 `Assets/LogicScripts/Utils/Constants.cs`，快照反映了该缺失。
- 2026-10-03：从上游 `69382c07ec20305bf283209d74eab180875c0ef7` 补入 `LuaTermBuilder.Register` 和四个动态牌体外观回调，形成 640 文件的混合快照。教程：[术语 / Terms](zh/terms.md)、[动态牌体 / Materials](en/materials.md)。这些新增接口要求 0.7.1.0 或之后版本。
- 2026-10-08：从 Aotenjo-Src 提交 `3e6165db6c28a648c88d2923683e2b8beef31957` 完整迁入 104 个非 `src` 文件，并逐文件核对原始字节。之后改为在主仓库维护、源码和手册一同发布。

The initial 2026-09-24 export had 638 files and reflected the missing Constants.cs file in that historical working tree. The 2026-10-03 API supplements brought it to 640 files. On 2026-10-08, all 104 non-source files were imported byte-for-byte from public commit `3e6165d`; subsequent publications are full exports of source and handbook from upstream.
