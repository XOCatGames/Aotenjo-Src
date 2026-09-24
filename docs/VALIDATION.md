# 验证范围 / Validation scope

基准与源码哈希见 [SOURCE.md](SOURCE.md)。验收步骤在[中文测试页](zh/testing.md)和[English test guide](en/testing.md)。

## Automated checks / 自动检查

The 2026-09-24 local run passed **2,126 static checks**, **77 Lua scenario assertions**, **19 typed xLua/C# bridge checks**, and integrity/file comparisons for **8 ZIPs**. These are separate layers with the limits below.

2026-09-24 本地检查通过：**2,126 项静态检查、77 项 Lua 场景断言、19 项 xLua/C# 类型桥接检查、8 个 ZIP 的完整性和逐文件比对**。各层测试的含义与边界如下。

- `python tools/validate.py`: source snapshot hashes, Chinese/English chapter and translation parity, local Markdown links, JSON duplicate keys, manifest fields, PNG dimensions/chunk CRC/decompression, required sprite/localization coverage, module namespacing, and callback arity extracted from real C# Builder declarations.
- `lua tools/tests/examples.lua`, or `python tools/run_lua_tests.py --library /path/to/xlua.dll`: execute all six script entry points in a shared Lua environment with **fake CS objects**; assert selected/unselected tile behavior, positive/negative/empty yaku conditions, deferred growth, nil-player descriptions, material effects, crafting registration, and color targets.
- `python tools/bridge_probe.py --unity-root /path/to/Aotenjo-Unity`: a developer-only supplement compiling the **real exported Builder and Lua-instance implementations**, real xLua, and Newtonsoft.Json, with **stub game/Unity base types**. It checks real C# arrays, generic lists, delegates, callback invocation, deferred state changes, artifact JSON data round-trip, material copy isolation, and recipe registration. It uses xLua's general mode and small explicit delegate helpers; it does not load the game's generated wrappers or certify a shipped AOT build. Requires .NET 10 and the matching developer checkout; private dependencies are not bundled or needed by players.
- `python tools/package_examples.py`: build seven individual ZIPs and one combined ZIP with reproducible timestamps; verify archive integrity, install layout, and every archived byte against its source file.

静态检查核对相对链接的文件是否存在，不核验网页锚点或所有外部链接。Lua 场景使用真实解释器，但 **CS 对象是测试替身**。补充桥接检查使用真实导出的 Builder 和 Lua 实例类、xLua 与 Newtonsoft.Json，验证实际 C# 类型转换、回调、藏品数据 JSON 往返和材质复制；周边游戏对象仍是替身，且使用通用模式与少量测试委托桥，不是游戏生成绑定或发行版 AOT 验收。

Committed CI repeats the portable static, Lua-scenario, and package-reproducibility checks on Linux. It cannot run the developer-only bridge probe without the private Unity checkout. Text file line endings and ZIP entry metadata are fixed for consistent Windows/Linux archives.

仓库 CI 在 Linux 重复静态、Lua 场景和安装包可复现检查；不具备私有 Unity 工作区，因此不执行补充桥接检查。示例文本使用 LF，ZIP 条目元数据固定，使 Windows/Linux 输出一致。

## In-game acceptance / 游戏内验收

The matrix in the testing guides remains a required manual game check. No Play Mode, packaged-game session, Workshop upload, or cross-process game save/resume is claimed by the repository tests. Rendering, shop availability, actual crafting flow, material delegate restoration, and platform binding must be verified on the intended game build before release.

测试页的实机矩阵仍须在目标发行版执行。本次不把模拟对象测试冒充 Play Mode、游戏包运行、Workshop 上传或跨进程续局验收。尤其注意材质委托的读档恢复，以及新客户端功能在旧版本是否存在。

## Reproduce / 复现

Run commands from the repository root with Python 3.10+ and Lua 5.3/5.4. Python tools use only the standard library. The native-library option can use the existing Windows x64 xLua library from a developer checkout; that library is not redistributed here. Readers do not need these maintainer tools to install or edit the mods.

在仓库根目录运行；Python工具只使用标准库。原生解释器选项可使用开发工作区已有的xLua库，本仓库不再分发该库。玩家安装示例无需 Python 或这些检查工具。
