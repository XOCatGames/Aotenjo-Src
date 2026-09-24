# 快速上手：从空文件夹到二号硬币

[目录](README.md) · [English](../en/quickstart.md) · 下一步：[Lua 与游戏对象](lua-bridge.md)

青天井通过 xLua 加载模组的 Lua 脚本。我们先运行一个最简单的 Hello 示例，确认模组放对了位置，再来制作自己的第一个遗物。

## 1. 先让游戏加载一个模组

1. 先退出游戏，在 Steam 中右键青天井，选择“管理 → 浏览本地文件”，打开游戏安装目录。
2. 在 Windows 下找到 `Aotenjo_Data/StreamingAssets/mods/`。如果没有 `mods` 文件夹，自己新建一个就可以。
3. 在本仓库点击 **Code → Download ZIP**，下载并解压，然后把 `example/mods/00_hello` 整个文件夹复制到 `mods` 里。放好后，应该能直接找到 `mods/00_hello/modinfo.json`。
4. 启动游戏，打开模组管理器，看看列表中是否出现了 `Hello / 你好`。
5. 接着打开 `%USERPROFILE%/AppData/LocalLow/Aotenjo/Aotenjo/AML/`，找到最新的 `AML_*.log`，搜索 `[tutorial_hello] Hello / 你好`。

看到这行日志，就说明 Hello 脚本已经运行了！如果模组出现在列表里，却没有输出这行文字，可以在日志中搜索 `Failed to load Lua script`。脚本出错时，模组仍可能显示在列表里，所以还要看一下日志。

其他平台的模组目录同样位于 `Application.streamingAssetsPath/mods`，日志位于 `Application.persistentDataPath/AML`。上面列的是 Windows 下的常用路径；如果发行版使用了不同的公司名或产品名，日志目录也会相应变化。

## 2. 新建自己的模组文件夹

现在我们来做一个二号硬币：每张本次打出的数牌二结算时，获得2金币。

在 `mods` 里新建 `my_first_mod` 文件夹，再按下面的结构创建文件。文本文件用 UTF-8 保存；如果使用记事本，记得检查扩展名，避免保存成 `init.lua.txt` 或 `modinfo.json.txt`。

```text
mods/
└─ my_first_mod/
   ├─ modinfo.json
   ├─ script/init.lua
   ├─ lang/zh-CN.json
   ├─ lang/en-US.json
   └─ texture/artifact/coin.png
```

先打开 `modinfo.json`，填入模组的基本信息。名字、作者和介绍都可以按自己的想法修改，这里我们先用下面这份：

```json
{
  "modID": "my_first_mod",
  "name": "My First Mod / 我的第一个模组",
  "version": "1.0.0",
  "author": "Your name",
  "description": "Played twos earn 2 coins / 打出的二获得2金币"
}
```

## 3. 给二号硬币添加效果

接着打开 `script/init.lua`，把下面这段代码完整放进去。游戏加载模组时，会自动执行其中的 `init()` 函数：

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

这里用 `LuaArtifactBuilder` 创建了一个遗物。`OnTileEffect` 中的代码会在单张牌计分时被调用：`player:Selecting(tile)` 检查这张牌是不是本次打出的，`tile:IsNumbered(2)` 检查它是不是数牌二。两个条件都满足，就添加一个获得2金币的效果。最后的 `BuildAndRegister()` 把它注册到游戏中。

## 4. 添加名字、说明和图片

有了效果，还要给二号硬币补上名字和图片。我们先打开 `lang/zh-CN.json`，写入中文名称和说明：

```json
{
  "artifact_my_first_mod:coin_name": "二号硬币",
  "artifact_my_first_mod:coin_description": "每张本次打出的二结算时获得2金币。"
}
```

然后在 `lang/en-US.json` 中写入对应的英文：

```json
{
  "artifact_my_first_mod:coin_name": "Coin of Twos",
  "artifact_my_first_mod:coin_description": "Gain 2 coins whenever a newly played two scores."
}
```

最后，把示例中的 [coin_twos.png](../../example/mods/ex1_artifact/texture/artifact/coin_twos.png) 复制过来，放在 `texture/artifact/` 下，并改名为 `coin.png`。这张图可以直接拿来跟着教程使用。

游戏会把图片注册成 `artifact:my_first_mod:coin`，正好对应我们创建的遗物。到这里，脚本、名字、说明和图片就都齐了。

## 5. 进入游戏试试看

保存文件，完整退出并重新打开游戏。在日志中找到 `[my_first_mod] ready` 后，打开“藏品”，看看二号硬币的图片、中文名和说明是否正确，再切换英文检查一遍。

为了方便拿到它进行测试，可以临时在 `init()` 中加上 `CS.Constants.CONSOLE_ENABLED = true`，重启游戏并开一个测试用的新局。按 F2 打开控制台，依次输入：

```text
give my_first_mod:coin
setHand 222m123p
```

第一条指令会获得二号硬币，第二条会把手牌换成测试用的牌。先打出二的刻子，看看每张二是否增加2金币；再打出另一组牌，之前已经落定的二应该不会再因为这个遗物赚钱。

如果你的游戏版本不支持 `Constants.CONSOLE_ENABLED`，先删掉这一行，再按[测试与排错](testing.md)里的方法检查。准备发布模组时，也要删掉这个临时调试开关。

效果能正常触发之后，就可以给它换个玩法了：把条件中的 `2` 改成 `9`，再把 `A.EarnMoneyEffect(2, artifact)` 改成 `A.ScoreEffect.AddFu(15, artifact)`，它就变成了“打出九时加15符”。一起改掉说明文字，再拿九来试一次。

## 6. 再了解一下模组怎么加载

游戏会先加载图片和 `lang` 中的文本，再执行 `script/init.lua`，最后调用里面的全局 `init()`。因此，把注册代码写进 `init()` 就可以了，不用在文件末尾再调用一次。

各个 Lua 模组使用的是同一个 Lua 环境。每个有脚本的模组都要定义自己的 `init()`，其他变量尽量加上 `local`。以后拆分脚本时，可以用 `require("my_first_mod.helpers")` 加载 `script/my_first_mod/helpers.lua`。给模块名加上自己的前缀，可以避免 `require("util")` 这样的通用名字读到另一个模组的缓存。

如果只想替换图片，可以做一个不带 `script` 的[材质包](textures.md)。想停用本地模组时，把整个模组文件夹移出 `mods`，再重启游戏即可；在 `modinfo.json` 里写 `enabled` 不会起作用。
