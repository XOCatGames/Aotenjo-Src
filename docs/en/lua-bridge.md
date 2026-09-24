# Lua and game objects

[Contents](README.md) · [中文](../zh/lua-bridge.md)

`CS` is xLua's entry point to C# types. `local A = CS.Aotenjo` is only an abbreviation. Some types live outside that namespace: `Yaku` and `TransformMaterialEffect` are global types, accessed as `CS.Yaku` and `CS.TransformMaterialEffect`. Check the namespace in the [source signature index](../../reference/api-signatures.md).

| Syntax | Meaning |
| --- | --- |
| `A.LuaArtifactBuilder.Create(id, A.Rarity.COMMON)` | Static method: use a dot |
| `builder:OnSelfEffect(callback)` | Instance method: use a colon, which supplies self |
| `A.EarnMoneyEffect(2, artifact)` | C# constructor: no `new` keyword |
| `player.Level`, `tile.properties.material` | Field or property access |
| `loc("some_key")` | Call a supplied C# delegate as a function |
| `nil` | A null C# reference, not an empty array |

## Collections and callbacks

Lua tables normally start at 1. C# `List<T>` starts at 0 and uses `.Count`; arrays start at 0 and use `.Length`. Do not use `ipairs`, `#list`, or `table.insert` on a C# List.

```lua
local tiles = player:GetHandDeckCopy()
for i = 0, tiles.Count - 1 do
    local tile = tiles[i]
    CS.Aotenjo.Logger.Log(tile:ToString())
end
```

This copies the container, not its Tile objects. Prefer a snapshot when iterating a player collection. Change properties with methods such as `tile:SetMaterial(material, player)` to preserve subscription and bookkeeping behavior.

For explicitly typed arrays and lists:

```lua
local function array(t, values)
    local result = CS.System.Array.CreateInstance(typeof(t), #values)
    for i, value in ipairs(values) do result[i - 1] = value end
    return result
end
local names = array(CS.System.String, {"standard"})
local packs = array(CS.System.Int32, {2})
local empty = array(CS.System.String, {})
local artifacts = CS.System.Collections.Generic.List(CS.Aotenjo.Artifact)()
```

A callback is a function the engine calls later. The API determines its exact parameter order. `WithName` takes `(player, artifact)`; `WithDescription` takes `(player, loc, artifact)`. Collection views may supply a nil player. Boolean callbacks must return a boolean. Action callbacks add to the supplied `effects` list and do not return a replacement list.

## Queued effects

The engine collects effects, then executes their `Ingest` methods during presentation. Adding a `ScoreEffect` queues work; it does not immediately change the player. A `List<Effect>` accepts ordinary effects. A `List<IAnimationEffect>` can also accept wrappers such as `effect:OnTile(tile)` or `:OnBlock(block)`. Do not add an animation wrapper to a list requiring an `Effect`.

Conditions, highlights, names, and descriptions can be queried repeatedly and must be read-only. Perform growth, rewards, and random draws when an effect actually executes; see the [cookbook](cookbook.md). Do not fetch a current player in `init()`; the game is usually still in its main menu.

## Types and persistent state

`LuaArtifact.GetDataOrDefault(key, defaultValue)` and `SetData(key, value)` accept strings. Read numbers with `tonumber(...) or 0` and write them with `tostring(value)`. Locals captured in closures, global tables, and delegates are not automatically saved.

`Rarity` values are `COMMON`, `RARE`, `EPIC`, `LEGENDARY`, and `ANCIENT`. Shop and pool eligibility still depends on each system's filtering; use the first three for beginner artifacts.

If a source method exists but your game reports signature/delegate errors, the installed build or its xLua/AOT bindings may differ. Reproduce with a complete example before changing arguments. For generic EventBus restrictions, see the [cookbook](cookbook.md). The old `player:PreKongTileEvent("+", handler)` example does not apply to this snapshot.
