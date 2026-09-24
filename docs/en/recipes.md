# Artifact recipes

[Contents](README.md) · [中文](../zh/recipes.md) · [Complete example](../../example/mods/ex4_recipe)

The example combines Copper Token (+8 Fu) and Silver Token (+2 Fan) into Double Token (+5 Fan). It includes all three scripts, translations, and icons. Install its folder independently; it does not depend on the artifact example.

Register the ordinary inputs first, create the output with `BuildAndRegisterCraftable()`, and then register the recipe:

```lua
-- copper, silver, and combined are Artifact objects registered earlier.
local inputs = CS.System.Collections.Generic.List(CS.Aotenjo.Artifact)()
inputs:Add(copper)
inputs:Add(silver)
local recipe = CS.Aotenjo.LuaArtifactRecipeBuilder.BuildAndRegister(
    "tutorial_recipe:tokens", inputs, combined)
assert(recipe ~= nil, "Recipe registration failed")
```

`Build(id, List<Artifact>, Artifact)` only creates a recipe. `BuildAndRegister(...)` also adds it to `ArtifactRecipes.recipes`, returning an `ArtifactRecipe` on success or nil with an error log on conflict. Craftable outputs return false from `IsAvailableInShops`; an ordinary `BuildAndRegister()` output does not automatically gain that rule.

## Current matching semantics

Inputs check whether the player owns each ID, not how many copies they own. Do not represent “two Copper Tokens” by repeating the same ID. Avoid duplicate and empty inputs. The current conflict check also rejects a recipe whose inputs are contained in an existing recipe's input set; it is not merely an exact-list equality check.

Obtaining artifacts and changing their order can trigger recipe checks. A fulfilled recipe consumes its inputs and grants its output. Register all referenced artifacts first. Avoid recipe cycles and competing recipes that can simultaneously consume the same inputs.

## Verification

In a fresh run, enter `give tutorial_recipe:copper_token`, then `give tutorial_recipe:silver_token`. The inputs should disappear and Double Token should appear automatically. Ordinary shops should not sell the result directly. Score a hand and check +5 Fan. Also test owning only one input, duplicate-registration errors, full inventory slots, and save/resume. Use a fresh run for repeated trials.

Source: [builder](../../src/API/Artifact/LuaArtifactRecipeBuilder.cs), [matching and consumption](../../src/ArtifactRecipe/ArtifactRecipe.cs), [craftable artifact](../../src/Artifact/CraftableArtifact/CraftableArtifact.cs).
