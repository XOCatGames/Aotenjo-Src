# 手册维护与发布 / Authoring and publication

[首页 / Home](../README.md) · [源码来源 / Source provenance](SOURCE.md)

## 中文

手册的维护源为 Aotenjo-Unity 的 `Docs/Modding/Handbook`。在游戏实现所在分支修改教程、参考说明、示例及对应生成器；API 文档可以先写入主仓库，教程只保留完成操作所需的步骤。每个新章节应有中英双语页面，并更新首页、两个目录以及需要时的 Wiki 生成器导航。

推送到主仓库 `main` 或 `dev`，且推送最后一个提交的消息包含 `[Sync]` 时，工作流才会公开同一次提交的源码和手册。普通推送只做相关文档校验。源码提交或配置中的版本字符串不能证明 Steam 构建已经发布。

同步会重新生成 `reference` 下的 API、事件、资源目录、源码散列和来源记录，检查本地链接、回调参数、Lua 示例场景及压缩包可重现性，再将 `src` 和手册一并提交到 Aotenjo-Src。语言、启停、贴图等 Unity 运行时实现的路径与内容散列也会记录，游戏资源文件本身不会跟随手册导出。

直接编辑 Aotenjo-Src 的受管理目录会在下一次同步时被覆盖；需要修正文档时，应回到主仓库修改。首次使用新流程会移除公开仓库旧的仅源码同步工作流，后续由主仓库发起完整同步。Wiki 生成工具仍可手动使用，`[Sync]` 不会自动推送 Wiki。

在主仓库根目录执行 `python .github/scripts/export-modding.py --output Temp/modding-public` 可以预演完整发布目录。对外仓库中的 `python tools/validate.py` 和 `python tools/package_examples.py` 仍可独立运行；它们的检查范围见[验证说明](VALIDATION.md)。

## English

The authoring source is `Docs/Modding/Handbook` in Aotenjo-Unity. Update guides, examples and their generators on the same branch as the game implementation. Draft API guides there, keeping only the steps needed to complete the task. Add both language pages and update the home page, both contents pages, and the Wiki generator navigation when applicable.

Publication requires a push to upstream `main` or `dev` whose final commit message contains `[Sync]`. Other relevant pushes validate the handbook without publishing it. A source commit or a version string in configuration is not evidence that a Steam build has shipped.

The workflow exports the exact triggering commit, regenerates API/event/resource catalogs, source hashes and provenance, checks links, callback contracts, Lua scenarios and reproducible archives, then commits source and handbook together to Aotenjo-Src. Provenance also records paths and hashes for reviewed Unity runtime integration files; the game's resource files are not exported.

Edits made directly to managed paths in Aotenjo-Src will be overwritten by the next sync. Make corrections upstream. The first full publication retires the old public source-only receiver; subsequent publication is controlled upstream. Wiki generation remains manual and `[Sync]` does not push the Wiki.

From the Unity checkout, preview the complete publication using `python .github/scripts/export-modding.py --output Temp/modding-public`. In the public checkout, `python tools/validate.py` and `python tools/package_examples.py` remain available; see [validation scope](VALIDATION.md).
