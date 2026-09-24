# 自定义遗物合成配方

[目录](README.md) · [English](../en/recipes.md) · [完整示例](../../example/mods/ex4_recipe)

有了自己的遗物之后，还可以给它们做一张合成配方。这一篇我们用铜代币和银代币，合成一个更强的双代币：铜代币加8符，银代币加2番，合成后的双代币加5番。

完整示例中已经放好了三件遗物的脚本、图片和中英文文本。把 `ex4_recipe` 整个文件夹装进去，就可以单独运行，不需要先安装前面的遗物示例。

## 1. 准备合成需要的遗物

打开示例的 `script/init.lua`，先看前面创建三件遗物的部分。铜代币和银代币按普通遗物注册，双代币则使用 `BuildAndRegisterCraftable()`，表示它是合成结果。

这种合成遗物的 `IsAvailableInShops` 会返回 false，因此正常商店不会直接出售它。如果仍然使用普通的 `BuildAndRegister()`，就不会自动有这条限制。

## 2. 注册合成配方

三件遗物都准备好之后，把两个输入遗物放进一个 C# 列表，再交给 `LuaArtifactRecipeBuilder`：

```lua
-- copper、silver、combined 是前面注册得到的 Artifact 对象。
local inputs = CS.System.Collections.Generic.List(CS.Aotenjo.Artifact)()
inputs:Add(copper)
inputs:Add(silver)
local recipe = CS.Aotenjo.LuaArtifactRecipeBuilder.BuildAndRegister(
    "tutorial_recipe:tokens", inputs, combined)
assert(recipe ~= nil, "Recipe registration failed")
```

这里的 `inputs` 存放合成所需的铜代币和银代币，`combined` 是前面创建的双代币。`BuildAndRegister(...)` 会创建配方，并加入 `ArtifactRecipes.recipes`。成功时返回一个 `ArtifactRecipe`；如果配方冲突，就返回 nil 并记录错误，所以示例用 `assert` 检查了一下。

另一个方法 `Build(id, List<Artifact>, Artifact)` 只创建配方，不加入注册表。一般制作模组时，用示例中的 `BuildAndRegister(...)` 即可。

## 3. 合成是怎样判断的

游戏检查的是“玩家是否拥有每个输入 ID 对应的遗物”。因此，这个配方可以表达“铜代币加银代币”，但不能把铜代币写两遍，来表达“需要两枚铜代币”。输入列表要有内容，也不要重复填写同一个遗物。

注册多个配方时，还要留意输入之间的关系。当前冲突检查会拒绝这样的新配方：它所需的全部输入，都已经包含在某张已有配方的输入里。也就是说，即使两张配方的输入不完全相同，也可能发生冲突。

玩家获得遗物、调整遗物顺序等操作时，游戏会检查是否满足配方。满足条件后，就消耗输入遗物，再获得合成结果。为了让游戏能找到这些物品，记得先注册输入和结果，再注册配方。设计多张配方时，也要避免循环合成，或让几张同时成立的配方争抢同一组输入。

## 4. 进入游戏合成一次

重启游戏并开一个新局，依次输入 `give tutorial_recipe:copper_token`、`give tutorial_recipe:silver_token`。拿到第二件遗物后，铜代币和银代币应该被消耗，变成双代币。再打一手牌，检查双代币是否加5番，同时确认正常商店不会直接出售它。

还可以分别试试只拿一件输入遗物、遗物槽已满、保存后退出续局等情况，并查看有没有重复加载的错误。每轮测试都从新局开始，可以避免上一次留下的遗物影响结果。

源码：[Builder](../../src/API/Artifact/LuaArtifactRecipeBuilder.cs)、[配方判定和消耗](../../src/ArtifactRecipe/ArtifactRecipe.cs)、[合成遗物](../../src/Artifact/CraftableArtifact/CraftableArtifact.cs)。
