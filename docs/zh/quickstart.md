# 快速上手：从空文件夹到二号硬币

[目录](README.md) · [English](../en/quickstart.md) · 下一步：[Lua 与游戏对象](lua-bridge.md)

## 先运行完整示例

1. 在 Steam 中打开青天井的安装目录（管理 → 浏览本地文件），退出游戏。
2. Windows 下找到 `Aotenjo_Data/StreamingAssets/mods/`；没有 `mods` 就新建。实际依据是 Unity 的 `Application.streamingAssetsPath/mods`，不是存档目录。其他平台使用各自的 StreamingAssets 位置。
3. 从仓库 **Code → Download ZIP** 下载并解压。把 `example/mods/00_hello` 这个文件夹复制到 `mods`。直接下一层必须能找到 `modinfo.json`，不要额外套一层仓库文件夹。
4. 启动游戏，进入模组管理器，应该看到 `Hello / 你好`。
5. 打开 `%USERPROFILE%/AppData/LocalLow/Aotenjo/Aotenjo/AML/` 最新的 `AML_*.log`，搜索 `[tutorial_hello] Hello / 你好`。若发行版修改了公司或产品名，以其 `Application.persistentDataPath/AML` 为准。

模组管理器显示“已加载”并不保证 Lua 执行成功：加载器会捕获 Lua 异常，仍把模组加入列表。必须检查日志中的成功标记和 `Failed to load Lua script`。

## 自己写最小藏品

创建如下结构；用 UTF-8 保存，确认文件不是 `init.lua.txt` 或 `modinfo.json.txt`。

```text
mods/
└─ my_first_mod/
   ├─ modinfo.json
   ├─ script/init.lua
   ├─ lang/zh-CN.json
   ├─ lang/en-US.json
   └─ texture/artifact/coin.png
```

`modinfo.json`：

```json
{
  "modID": "my_first_mod",
  "name": "My First Mod / 我的第一个模组",
  "version": "1.0.0",
  "author": "Your name",
  "description": "Played twos earn 2 coins / 打出的二获得2金币"
}
```

`script/init.lua` 全文：

```lua
local A = CS.Aotenjo
function init()
    A.LuaArtifactBuilder.Create("my_first_mod:coin", A.Rarity.COMMON)
        :OnTileEffect(function(player, perm, tile, effects, artifact)
            if player:Selecting(tile) and tile:IsNumbered(2) then
                effects:Add(A.EarnMoneyEffect(2, artifact))
            end
        end)
        :BuildAndRegister()
    A.Logger.Log("[my_first_mod] ready")
end
```

`lang/zh-CN.json`：

```json
{
  "artifact_my_first_mod:coin_name": "二号硬币",
  "artifact_my_first_mod:coin_description": "每张本次打出的二结算时获得2金币。"
}
```

`lang/en-US.json`：

```json
{
  "artifact_my_first_mod:coin_name": "Coin of Twos",
  "artifact_my_first_mod:coin_description": "Gain 2 coins whenever a newly played two scores."
}
```

把示例中的 [coin_twos.png](../../example/mods/ex1_artifact/texture/artifact/coin_twos.png) 复制为 `texture/artifact/coin.png`。游戏自动把它注册成 `artifact:my_first_mod:coin`，与藏品 ID 对应。这已是一份完整模组。

## 第一次验证

每次修改都完整退出并重启。确认日志中 `[my_first_mod] ready`，检查中英文图鉴名称和说明。为确定测试而临时在 `init()` 内加入 `CS.Constants.CONSOLE_ENABLED = true`，再次重启；进入一个测试新局，按 F2 输入：

```text
give my_first_mod:coin
setHand 222m123p
```

先打出二的刻子，观察每张二增加2金币；再打出另一组，已落定的二不会因为这件藏品再赚钱。测试发行版若没有 `Constants.CONSOLE_ENABLED`，移除该行，见[兼容性排错](testing.md)。发布时删除这行调试开关。

现在把条件中的 `2` 改成 `9`，把 `A.EarnMoneyEffect(2, artifact)` 改成 `A.ScoreEffect.AddFu(15, artifact)`，同步修改描述，再用九测试。接口把你的想法拆成了 **触发时机 → 条件 → 效果 → 注册**。

## 加载规则

资源和 `lang` 先加载，随后执行 `script/init.lua` 顶层代码，再调用全局 `init()`。不要自行调用第二次。所有 Lua 模组共享一个 Lua 环境：每个有脚本的模组都应定义自己的 `init()`，其他变量尽量 `local`；多个文件用 `require("my_first_mod.helpers")` 对应 `script/my_first_mod/helpers.lua`。不要使用通用的 `require("util")`，否则可能命中另一模组缓存。

纯材质包可以没有 `script`，详见[材质包](textures.md)。停用本地模组时，把整个文件夹移出 `mods` 并重启；加载器没有读取 `enabled` 这一清单字段。
