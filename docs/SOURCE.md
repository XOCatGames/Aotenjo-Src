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

## 0.7.1.0 接口补充 / API supplements for 0.7.1.0

**下面的新接口仅在青天井 0.7.1.0 及之后版本可用，0.7.0.x 不支持。**
**The new APIs below are available only in Aotenjo 0.7.1.0 and later, not in 0.7.0.x.**

补充来源为 `Aotenjo-Unity/dev-0.7.1.0` 的提交 `69382c07ec20305bf283209d74eab180875c0ef7`，核对日期为 2026-10-03。
Source: commit `69382c07ec20305bf283209d74eab180875c0ef7` on `Aotenjo-Unity/dev-0.7.1.0`, reviewed on 2026-10-03.

| 接口 / API | 教程 / Guide | 同步的公开源码 / Public source supplement |
| --- | --- | --- |
| `LuaTermBuilder.Register` | [中文](zh/terms.md) / [English](en/terms.md) | `API/Terms/LuaTermBuilder.cs`、`API/Terms/link.xml` |
| `LuaTileMaterialBuilder.WithSpriteID`、`WithSpriteNamespaceID`、`WithOrnamentSpriteID`、`WithShadowID` | [中文](zh/materials.md) / [English](en/materials.md) | `API/TileProperties/LuaTileMaterialBuilder.cs`、`API/TileProperties/LuaTileMaterial.cs` |

这里只补充表中接口所需的源码，其他文件继续沿用上面的 2026-09-24 基准；没有执行整个开发分支的源码同步。
Only the listed API sources are supplemented; other files retain the 2026-09-24 baseline. This is not a full export of the development branch.
每次补充的最低游戏版本、上游提交、公开路径与内容哈希记录在 [provenance.json](../reference/provenance.json) 的 `feature_supplements` 中；[source-manifest.json](../reference/source-manifest.json) 校验当前混合快照的完整文件清单。
The `feature_supplements` entries in provenance record minimum versions, upstream commits, public paths, and hashes; the source manifest validates the current combined inventory.
