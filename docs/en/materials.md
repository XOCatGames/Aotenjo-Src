# Tile materials and material sets

[Contents](README.md) · [中文](../zh/materials.md) · [Complete Gemstones mod](../../example/mods/ex3_tile_material)

A TileMaterial changes gameplay; a [texture pack](textures.md) replaces images. The example registers Ruby (+20 Fu and 2 coins when its round-end effect is processed) and Sapphire (×2 Fan), then adds them to `tutorial_gemstones`.

**Version requirement: the four new dynamic tile-body appearance methods, `WithSpriteID`, `WithSpriteNamespaceID`, `WithOrnamentSpriteID`, and `WithShadowID`, are available only in 0.7.1.0 and later, not in 0.7.0.x.** This new requirement applies to those four methods, rather than the existing material mechanics and set APIs.

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
| `WithSpriteID` | `(p, m) → int` | Dynamic tile-body sprite index; **0.7.1.0+ only** |
| `WithSpriteNamespaceID` | `(p, m) → string` or `nil` | Dynamic full sprite key; a non-`nil` key takes priority over the numeric index; **0.7.1.0+ only** |
| `WithOrnamentSpriteID` | `(p, m) → int` | Dynamic ornament index; `-1` hides it; **0.7.1.0+ only** |
| `WithShadowID` | `(m) → int` | Dynamic shadow index, with no player argument; **0.7.1.0+ only** |
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

## Dynamic tile-body appearance (0.7.1.0+ only)

**All four appearance callbacks in this section require 0.7.1.0 or later.** To change a tile's body when its level reaches 5, register a callback that reads the current material's state and returns its appearance.

Create a mod with `modID` set to `tutorial_dynamic`, using the Gemstones example for its folder layout, and add the material's name and description language keys. Place two body PNGs at `texture/tile_material/base.png` and `texture/tile_material/grown.png`. Their full sprite keys are `tile_material:tutorial_dynamic:base` and `tile_material:tutorial_dynamic:grown`, without a file path or `.png` suffix.

Register the material in `script/init.lua`:

```lua
local A = CS.Aotenjo

local function level(material)
    return tonumber(material:GetDataOrDefault("level", "0")) or 0
end

function init()
    A.LuaTileMaterialBuilder.Create("tutorial_dynamic:base")
        :WithData("level", "0")
        :WithSpriteNamespaceID(function(player, self)
            if level(self) >= 5 then
                return "tile_material:tutorial_dynamic:grown"
            end
            return "tile_material:tutorial_dynamic:base"
        end)
        :WithOrnamentSpriteID(function(player, self)
            return level(self) >= 5 and 14 or -1
        end)
        :WithShadowID(function(self)
            return level(self) >= 5 and 17 or 19
        end)
        :BuildAndRegister()
end
```

`self` is the material instance currently being displayed. Read its data to keep each tile's appearance independent; avoid capturing one shared material instance. This sample chooses appearance without increasing the level. Your own gameplay effect should call `SetData("level", "5")` or similar on the current material when that effect executes.

If your mod already defines `init()`, merge the registration chain into that function and retain its other content. Do not overwrite it with a second definition.

To select existing sprites from the built-in `TileMaterials` atlas instead, replace `WithSpriteNamespaceID` above with this callback. Indices start at 0:

```lua
    :WithSpriteID(function(player, self)
        local currentLevel = tonumber(self:GetDataOrDefault("level", "0")) or 0
        return currentLevel >= 5 and 53 or 6
    end)
```

This fragment belongs in the same Builder chain; it is not a standalone Lua script. Ornament and shadow callbacks also select indices in the `TileMaterials` atlas. You can replace 14, 17, and 19 with valid indices in that atlas. Ornaments allow `-1` to hide them; shadows do not support `-1` for hiding. Without an override, the numeric body index defaults to 0, ornament to `-1`, shadow to 19, and the sprite key follows the material's existing mod-image rule.

If both body callbacks are configured, a non-`nil` key from `WithSpriteNamespaceID` takes priority. Returning Lua `nil` uses the built-in sprite-key/atlas lookup when `WithSpriteID` is configured; otherwise it uses this material's default mod sprite key. An empty string is not `nil` and does not request the default. A missing keyed sprite can fall through to numeric atlas lookup, so check that every key matches an installed PNG instead of treating missing artwork as a successful switch.

Appearance callbacks may run repeatedly. Only return appearance values; do not award coins, increase levels, or trigger gameplay effects in these getters. `player` can be `nil`, especially for a sprite-index lookup without a player; check `player ~= nil` before using it. `WithShadowID` receives only `self`. Calling `SetData` does not force an immediate redraw: the updated appearance is read during the game's next normal tile-display refresh.

Test on **0.7.1.0 or later**. Use `setMat 0 tutorial_dynamic:base` to apply the registered material and check its initial appearance. Change `WithData("level", "0")` to `WithData("level", "5")`, restart the game, and apply the material again in a new test run to check the grown body, ornament, and shadow. Separately test display refresh after your real growth effect changes state. `BuildAndRegister()` registers a factory; it does not change existing tiles.

Also test independent state on two tiles, copied data, and save/resume after restarting the game. Appearance delegates are not serialized. Copying reconstructs callbacks through the registered factory and clones data, which does not replace cross-process save testing on the intended build. Use this section for the tile body; face colors and gradients use [`TileFaceMaterialRegistry`](textures.md).

Source: [material builder](../../src/API/TileProperties/LuaTileMaterialBuilder.cs), [material instance](../../src/API/TileProperties/LuaTileMaterial.cs), [set builder](../../src/API/TileProperties/LuaMaterialSetBuilder.cs).
