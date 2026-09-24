# Turn your ideas into rules

[Contents](README.md) · [中文](../zh/cookbook.md)

These snippets belong inside an artifact Builder chain using `local A = CS.Aotenjo`; they are not standalone mods. Copy the [complete artifact mod](../../example/mods/ex1_artifact), replace a callback, and update both descriptions.

## “Each newly played nine adds 15 Fu”

```lua
:OnTileEffect(function(player, perm, tile, effects, artifact)
    if player:Selecting(tile) and tile:IsNumbered(9) then
        effects:Add(A.ScoreEffect.AddFu(15, artifact))
    end
end)
```

Use `tile:GetCategory() == A.Tile.Category.Suo` for bamboo; `Wan` means characters and `Bing` means circles. `tile:IsYaoJiu(player)` checks terminals/honors using player-aware rules. Change one condition at a time and write both positive and negative test cases.

## “Ordinary discards refund one coin; forced discards do not”

```lua
:OnDiscardTileEffect(function(player, tile, effects, withForce, isClone, artifact)
    if not withForce and not isClone then
        effects:Add(A.EarnMoneyEffect(1, artifact):OnTile(tile))
    end
end)
```

There is no perm argument here. This animation-effect list associates the reward with its tile. Excluding clone triggers is a design choice made explicitly in this example.

## “A 10% chance to earn three coins, with a saved win count”

```lua
:OnSelfEffect(function(player, perm, effects, artifact)
    effects:Add(A.SimpleEffect("my_mod_roll", artifact, function(p)
        if p:GenerateRandomInt(10, "my_mod:reward") == 0 then
            p:EarnMoney(3)
            local wins = tonumber(artifact:GetDataOrDefault("wins", "0")) or 0
            artifact:SetData("wins", tostring(wins + 1))
        end
    end))
end)
```

Translate `my_mod_roll` and reset `wins` in `ResetArtifactState`. `GenerateRandomInt(n, category)` returns 0 through n-1; n must be positive. A dedicated category avoids sharing the default stream. Never consume randomness in descriptions, predicates, or highlights. This SimpleEffect displays its own text even on a losing roll. For conditional presentation, examine [MaybeEffect](../../src/Effects/MaybeEffect.cs) rather than moving randomness into a UI query.

## “Turn this tile into Ruby after scoring”

With the Gemstones mod installed and registered, queue this from a callback that supplies a tile:

```lua
effects:Add(CS.TransformMaterialEffect(
    A.TileMaterial.GetMaterial("tutorial_gems:ruby"), artifact, tile, "my_mod_transform"))
```

The type is global `CS.TransformMaterialEffect`, not `A.TransformMaterialEffect`. Add both translations for the text key. For discard or round-end animation lists, append `:OnTile(tile)`. GetMaterial throws if the dependency is absent. Before publishing, include and register your own material or clearly declare/check the dependency. Use a fresh material instance for each target.

## Useful objects

| Object | Entry point | Return value and notes |
| --- | --- | --- |
| Player | `Selecting(tile)` | Boolean: newly selected semantics |
| Player | `GetHandDeckCopy()`, `GetTilePool()` | C# Lists; copied containers still reference real tiles |
| Player | `GetArtifacts()` | Held artifacts; do not edit the registry to simulate obtaining one |
| Player | `ObtainArtifact(artifact, forced=false)` | Boolean; normal acquisition pipeline |
| Player | `AddNewTileToPool(tile)` | Boolean; rules can cancel the addition |
| Player | `EarnMoney(int)`, `GenerateRandomInt(int, string)` | Use during effect execution |
| Tile | `GetOrder()`, `GetCategory()` | Values after transformations |
| Tile | `IsNumbered()` / `IsNumbered(int)` | Numbered tile / matching number |
| Tile | `Copy()` | Independent mutable state; prefer this virtual method for special tiles |
| Tile | `SetMaterial(mat, player)`, `SetFont(font, player)`, `SetMask(mask, player)` | Apply properties through notifications and subscriptions |
| Permutation | `ToTiles()` | All combination tiles, potentially including previously settled ones |
| Block | `tiles`, `IsAAA()` | Tile collection and triplet query; see source for more |
| Artifact | `GetRegName()` | Stable ID, not translated display text |

See [API signatures](../../reference/api-signatures.md). Prefer existing effects: `ScoreEffect.AddFu/AddFan/MulFan(number, source)`, `EarnMoneyEffect(int, source)`, `TextEffect(key, source)`, and `SimpleEffect(key, source, function(player))`. The source is the artifact object; pure material effects can pass nil.

## Advanced events and limitations

Current Player code publishes [typed events](../../reference/events.md) through [EventBus](../../src/Event/NewEventSystem/EventBus.cs). The old instance-event add/remove syntax has been migrated. Prefer a Builder callback when one already covers the trigger.

The C# contract is `Subscribe<T>(player, Action<T>, priority=0, once=false)` / `Unsubscribe<T>(player, Action<T>)`. T must derive from PlayerEvent. Dispatch matches the **exact published type**, not all derived events of a subscribed base type. Higher priority runs first; once subscriptions are removed after triggering. Retain the same delegate for removal, avoid duplicate rebinding, and do not award rewards from query events.

`EventBus.Subscribe<T>` is not Lua syntax. xLua must close the generic method and create the matching Action delegate; reflection/AOT support varies by platform. There is currently no non-generic Lua event adapter, so this handbook does not present an unverified universal Lua subscriber. For a successful kong, the relevant event is `PostKongTilesEvent`, not the cancelable `PlayerEvents.PreKongTileEvent`. Verify the target build's binding before implementing an adapter.

Direct gadget calls have also changed: `UseOnTiles` returns `GadgetUseResult`; check `.Success`, not the truthiness of the result object. `Player.PostUsedGadget` handles successful-use consumption. There is no current LuaGadgetBuilder or complete Lua boss registration API.
