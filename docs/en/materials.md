# Tile materials and material sets

[Contents](README.md) · [中文](../zh/materials.md) · [Complete Gemstones mod](../../example/mods/ex3_tile_material)

A TileMaterial changes gameplay; a [texture pack](textures.md) replaces images. The example registers Ruby (+20 Fu and 2 coins when its round-end effect is processed) and Sapphire (×2 Fan), then adds them to `tutorial_gemstones`.

## Distinguish the identifiers

| Purpose | Ruby example |
| --- | --- |
| `LuaTileMaterialBuilder.Create(nameKey)` | `tutorial_gems:ruby` |
| `material:GetRegName()` | `tutorial_gems:ruby_material`, with an automatic suffix |
| `TileMaterial.GetMaterial(id)` | Either form above; registration must already exist |
| PNG file | `texture/tile_material/ruby.png` |
| Sprite registration | `tile_material:tutorial_gems:ruby` |
| Name key | `tile_tutorial_gems:ruby_material_name` |
| Short-name key | `tile_tutorial_gems:ruby_material_name_short` |
| Description key | `tile_tutorial_gems:ruby_material_description` |

Do not add `_material` to `Create`. Sprite lookup currently removes every `_material` substring from the registration name, so avoid that substring in your namespace too. The example uses `tutorial_gems`.

## Complete LuaTileMaterialBuilder reference

`p=Player`, `m=current material`, `E=List<Effect>`, `AE=List<IAnimationEffect>`.

| Method | Argument or callback signature | Purpose |
| --- | --- | --- |
| `Create` | `nameKey: string` | Static entry point |
| `WithRarity` | `Rarity` | Defaults to COMMON |
| `WithData` | `(key: string, value: string)` | Initial string state; Build clones the data container |
| `WithDebuff` | `(m) → bool` | Whether the material is a debuff |
| `WithDescription` | `(loc, p, m) → string` | The localizer is the first argument |
| `OnScoringEffect` | `(p, perm, tile, E, m)` | Material scoring |
| `OnUnusedEffect` | `(p, perm, E, m)` | Unused effect, with no tile argument |
| `OnDerivedTileUnusedEffect` | `(p, perm, E, scoringTile, onEffectTile, m)` | Related scoring tile and effect-owning tile |
| `OnRoundEndEffect` | `(p, perm, AE, tile, m)` | Round-end animation effects; not every tile in the wall receives this callback |
| `OnDiscardEffect` | `(p, perm, AE, tile, withForce, isClone, m)` | Discard; differs from artifact argument order |
| `OnSubscribe` / `OnUnsubscribe` | `(p, m)` | Bind/unbind |
| `Build()` | Returns `LuaTileMaterial` | Create only |
| `BuildAndRegister()` | Returns `LuaTileMaterial` | Adds the construction factory to MaterialProviders |

Materials support string state through `GetDataOrDefault` / `SetData`. `Copy()` uses the registered factory and copies the data, so register first. Do not assign the same mutable instance to multiple tiles; use separate `GetMaterial(...)` calls or `.Copy()`. This does not guarantee Lua delegate restoration in every build's save deserialization. Test saves containing custom materials separately.

## Sets determine availability

| LuaMaterialSetBuilder method | Meaning |
| --- | --- |
| `Create(regName)` | Creates a set; custom sets internally use -5, so use names rather than numeric IDs |
| `WithAvailableMaterials(string[])` | Replaces the material-name list |
| `AddMaterial(string)` | Appends a registered material |
| `OnGenerateCommon(function(set) → LotteryPool<TileMaterial>)` | Overrides the common pool; default filters COMMON |
| `OnGenerateRare(function(set) → LotteryPool<TileMaterial>)` | Overrides the rare pool; default filters RARE and EPIC |
| `OnSubscribe(function(p, set))` / `OnUnsubscribe(...)` | Player binding lifecycle |
| `Build()` / `BuildAndRegister()` | Create only / add to MaterialSets; duplicate set names skip registration |

The default pools work for the example. To customize weights, create `CS.LotteryPool(CS.Aotenjo.TileMaterial)()` inside `OnGenerateCommon`, call `pool:Add(material, positiveWeight)`, then return the pool. `LotteryPool` is a global C# type. Provide candidates for both common and rare pools so shop draws have valid results.

The set name uses `material_set_tutorial_gemstones_name`. Select that set for a new run to encounter its materials naturally. Registration alone does not turn existing tiles into Ruby.

## Deterministic testing

In a new test run, use `setHand 222m333p`, then `setMat 0-2 tutorial_gems:ruby`, and play the first three tiles. Each Ruby should add 20 Fu. The range includes 0, 1, and 2. Use `setMat 0 tutorial_gems:sapphire` to check ×2 Fan. At round end, verify 2 coins for each Ruby actually processed by that pipeline.

Inside an executing effect with a real tile and player, apply a material with `tile:SetMaterial(A.TileMaterial.GetMaterial("tutorial_gems:ruby"), player)` instead of assigning `tile.properties.material`. Test independent copied state and save/resume. Do not continue a test save that depends on a custom material after uninstalling its mod.

Source: [material builder](../../src/API/TileProperties/LuaTileMaterialBuilder.cs), [material instance](../../src/API/TileProperties/LuaTileMaterial.cs), [set builder](../../src/API/TileProperties/LuaMaterialSetBuilder.cs).
