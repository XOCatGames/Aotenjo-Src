# 测试、排错与发布

[目录](README.md) · [English](../en/testing.md) · [本次验证记录](../VALIDATION.md)

## 确认版本与日志

本手册基于[源码快照](../SOURCE.md)，不是对所有历史发行版的兼容保证。模组没有自动升级游戏的能力。先只装 `00_hello`，再一次增加一个示例；源码同步不代表 Steam 已经发布同一代码。旧客户端的材质包清单甚至可能被忽略而没有报错。

可以在自己的 `init()` 中临时加功能探测：

```lua
local ok, result = pcall(function()
    return typeof(CS.Aotenjo.TileFaceMaterialRegistry)
end)
CS.Aotenjo.Logger.Log("TileFaceMaterialRegistry available: " .. tostring(ok and result ~= nil))
```

探测到类型仍不证明每个方法的目标平台绑定可用。实际执行样例才是下一步。新配色示例使用 `assert` 检查注册返回值；不兼容时查看详细 Lua 错误。

AML 路径见[快速上手](quickstart.md)。本地化错误由 Unity Debug 输出，可能只在同一 persistentDataPath 下的 `Player.log` 出现，因此两处都检查。没有 texture 目录的纯脚本模组出现 `No textures directory found` 警告不一定是失败；真正要排查的是脚本异常和缺少成功标记。

## 开启测试控制台

在测试模组 `init()` 里临时写 `CS.Constants.CONSOLE_ENABLED = true`，完整重启，进入测试新局，按 F2。这个类型是游戏运行时的全局类型，不在当前公开源码快照中。旧版、不同平台可能没有该开关。它报错时删掉该行；通过正常游玩/图鉴测试可用功能，记录发行版信息，不要为了测试而修改真实存档。发布包不要留该开关。

| 指令 | 例子 | 精确含义 |
| --- | --- | --- |
| `give` | `give tutorial_artifact:coin_twos` | 获得已注册藏品；正常槽位与规则仍适用 |
| `addSlot` | `addSlot 3` | 增加3个槽位，便于组合测试 |
| `setHand` | `setHand 222m333p` | 替换手牌；m万、p饼、s索、z字牌；不要在正在计分时调用 |
| `setMat` | `setMat 0-2 tutorial_gems:ruby` | 设置索引0、1、2的材质，区间两端都包含 |
| `setFont` | `setFont 0 blue` | 改第0张手牌字体，便于观察配色 |
| `earn` | `earn 20` | 增加20金币 |
| `upgradeYaku` | `upgradeYaku custom_yaku:tutorial_yaku:all_twos 1` | 增加1级，不是设为1级 |
| `seed` | `seed` | 显示当前种子 |
| `help` | `help` | 列出当前发行版可用命令 |

以实际 `help` 和错误提示为准。某些旧指令的索引、范围语义不同，例如不要把 `setMat` 的包含末端规则套到旧版 `setCorrupted`。

## 七个示例的验收表

每个例子先单独安装，再一起安装；每轮重启。以下是必须实测的结果，**不是本仓库自动化测试已经完成实机验收的声明**。

| 示例 | 操作 | 应看到的结果 |
| --- | --- | --- |
| Hello | 启动，查看 AML | 一条 `[tutorial_hello] Hello / 你好`；再启动重新出现 |
| 二号硬币 | give；setHand 222m123p；依次打出两组 | 本次二每张+2金币；非二不触发，已落定的二不再次触发 |
| 练习玉 | give tutorial_artifact:practice_jade；计分两次 | 第一次×1.0，第二次×1.1；反复查看说明不增加计数 |
| 全二 | 升级自定义番种；先打222m，再在组合里加入非二 | 纯二组合成立；混入非二不成立；图鉴与火包有对应内容 |
| 宝石 | setMat；计分、完成关卡；新局选教学宝石组 | 红宝石每张+20符，蓝宝石×2番；经过关末效果的红宝石+2金币；组内能抽到材质 |
| 合成 | 依次 give 铜、银代币 | 两个输入消失，双代币出现并+5番；仅有一个输入时不合成 |
| 材质包 | 查看折纸熊图鉴/获得折纸熊 | 硬币图标，原名称与机制；移走包重启恢复 |
| 配色 | 进入新局；setFont 0 blue | 普通牌青绿、蓝字体彩虹；移走包重启恢复 |

接着检查：中英文本内容一致；反复触发不会多订阅；强制/普通弃牌符合设计；成长物品保存退出续局后值正确；新局归零；不同牌的材质状态互不污染；与另一模组共存后两个 `init()` 都只运行一次。先做不会用到新内容的存档备份，测试存档单独保留。

## 常见故障

| 症状 | 检查和解决 |
| --- | --- |
| 模组完全不显示 | 目录是否多套一层；modinfo.json 是否有效 JSON；真实扩展名 |
| 显示已加载，但没有内容 | AML 是否 `Failed to load Lua script`；函数是否全局 `init`；是否遗漏 BuildAndRegister |
| 自己的代码变成另一模组行为 | `require("util")` 重名缓存；改成带唯一前缀的模块路径 |
| 字典 duplicate key | 同时安装本地与 Workshop 同一份、重复 ID、手动再次调用 init |
| 图标错误 | modID、Create ID、文件名三者一致；PNG 是否位于正确直接子目录 |
| 描述显示键名 | 语言文件名/键名；Yaku 是否缺 `custom_yaku:`；查看 Player.log |
| 牌效果次数过多 | 是否检查 Selecting；是否误把阶段触发当整局触发；是否在查询时修改状态 |
| Nil player 或函数参数错位 | 图鉴 player 可以为空；逐项核对回调签名 |
| 番种不能抽到/不计分 | 注册前置、groups、包索引、稀有度、解锁等级和 predicate 分别检查 |
| 材质已注册却不出现 | 是否创建并选中材质组，普通/稀有池是否非空；先 setMat 验证 |
| 材质包有加载日志却没变化 | target 是否真实匹配；整图尺寸日志；是否被后加载包覆盖；完整重启 |
| 续局失去状态或方法 | 稳定ID、SetData字符串、模组是否还安装；材质委托恢复是单独兼容项 |

## 分享安装包与 Workshop

分享一个包含模组根目录的 ZIP，解压后直接有 `mods/你的文件夹/modinfo.json`。不要放入游戏 EXE、src、Unity Library、日志或自己的存档。每个示例包含 `img.png`，你可以换成自己的预览图；512×512以内是本教程的建议值，不是本次源码里验证出的服务端硬限制。

安装并验证本地模组后，在游戏模组管理器选择它并点击上传至创意工坊。上传器使用 name、description、整个模组目录和根目录 `img.png`。Steam 需要已连接；按 Steam 提示完成必要的创作者协议。新项目以 Private 可见性创建，检查页面后再改为公开。

首次发布保留 Workshop 页面中的 item ID。后续在同一个模组清单里写 `"workshopId": "你的item ID"`，再次上传更新该项目；省略会创建新项目。只用自己的项目 ID。上传器不会替你验证 Lua。

发布前完成上表，移除调试开关，说明测试过的游戏版本、模组版本、依赖、存档兼容范围和改动。最后在干净的 mods 目录解压你将分享的 ZIP 再测一次。
