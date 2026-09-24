# 藏品合成

[目录](README.md) · [English](../en/recipes.md) · [完整示例](../../example/mods/ex4_recipe)

示例将铜代币（加8符）与银代币（加2番）合成为双代币（加5番）。三件藏品的脚本、双语文本和图片都已提供；单独安装该文件夹即可，不依赖藏品示例。

步骤是先注册两件普通藏品，再用 `BuildAndRegisterCraftable()` 注册结果，最后注册配方：

```lua
-- copper、silver、combined 是前面注册得到的 Artifact 对象。
local inputs = CS.System.Collections.Generic.List(CS.Aotenjo.Artifact)()
inputs:Add(copper)
inputs:Add(silver)
local recipe = CS.Aotenjo.LuaArtifactRecipeBuilder.BuildAndRegister(
    "tutorial_recipe:tokens", inputs, combined)
assert(recipe ~= nil, "Recipe registration failed")
```

`Build(id, List<Artifact>, Artifact)` 只创建，`BuildAndRegister(...)` 还加入 `ArtifactRecipes.recipes`，成功返回 `ArtifactRecipe`，冲突返回 nil 并记录错误。合成结果的 `IsAvailableInShops` 返回 false；普通 `BuildAndRegister()` 创建的结果不会自动获得该规则。

## 当前配方语义

输入检查的是玩家是否拥有每个 ID，不计算相同 ID 的数量。因此不要用 `{铜, 铜}` 表达“两枚铜”。重复输入或空输入都不适合。当前冲突检测还会拒绝其输入被已有配方输入集合包含的配方；不要假定它只检查完全相等的列表。

获得藏品和调整藏品顺序等路径会检查合成。满足时消耗输入藏品，获得输出。注册顺序很重要：结果和输入都必须存在；不要设置合成循环，也不要让多个同时满足的配方争抢同一组输入。

## 验证

新局中按顺序输入 `give tutorial_recipe:copper_token`、`give tutorial_recipe:silver_token`。应自动得到双代币，输入藏品消失；正常商店不应直接出售双代币。出牌检查加5番。再分别测试仅拥有一个输入、重复加载错误、满槽获取和退出续局。每次重复试验用新局，避免旧输入影响结果。

源码：[Builder](../../src/API/Artifact/LuaArtifactRecipeBuilder.cs)、[配方判定和消耗](../../src/ArtifactRecipe/ArtifactRecipe.cs)、[合成藏品](../../src/Artifact/CraftableArtifact/CraftableArtifact.cs)。
