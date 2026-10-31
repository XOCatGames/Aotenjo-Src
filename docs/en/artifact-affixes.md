# Custom artifact affixes

[中文](../zh/artifact-affixes.md) · [Contents](README.md)

This API is being developed with artifact affixes on `codex/artifact-runtime-instances`. Check that the installed game includes `LuaArtifactAffixBuilder`; a version number alone does not establish availability.

## Register and apply

Register a definition in your mod's `script/init.lua`. Registration does not generate enchanted artifacts or add them to shops or reward pools.

```lua
local A = CS.Aotenjo
local id = "example.enchants:ember"
assert(A.LuaArtifactAffixBuilder.Create(id)
    :WithNameKey("example_enchants_ember_name")
    :WithDescriptionKey("example_enchants_ember_description")
    :OnSelfEffect(function(player, perm, effects, artifact)
        effects:Add(A.ScoreEffect.AddFu(7, artifact))
    end)
    :BuildAndRegister())
```

Add the translation keys to `lang/en-US.json` (see [Images and bilingual text](assets.md) for the language file format):

```json
{
  "example_enchants_ember_name": "Ember",
  "example_enchants_ember_description": "When this artifact scores: +7 Fu."
}
```

In an existing artifact's `OnObtain` callback or a mod gameplay callback, use the supplied player and an artifact they already hold:

```lua
local applied = player:SetCustomArtifactAffix(artifact, "example.enchants:ember")
```

Each artifact has one affix. Custom and builtin affixes replace each other. Broken, temporary, or pending evolution hosts reject assignment. Replacing Lightweight also returns `false` if removing its extra capacity would overflow inventory; failure preserves the previous affix. Reapplying the same ID preserves the instance and growth without triggering host subscription again. Add a pure eligibility predicate with `WithCanApply(function(player, artifact) ... end)`.

IDs use `namespace:name`, such as `example.enchants:ember`. Both parts start with a lowercase letter, followed by lowercase letters, digits, `_`, `.`, or `-`; the total limit is 128 characters. A name key and either a description key or callback are required. `BuildAndRegister()` returns `true` on success and `false` for invalid configurations or duplicate IDs. Existing definitions are never overwritten. Keep published IDs stable.

## Effect callbacks

| Builder method | Lua callback arguments | List element type |
| --- | --- | --- |
| `OnSelfEffect` | `player, perm, effects, artifact` | `Effect` |
| `OnTileEffect` | `player, perm, tile, effects, artifact` | `Effect` |
| `OnBlockEffect` | `player, perm, block, effects, artifact` | `Effect` |
| `OnRoundEndEffect` | `player, perm, effects, artifact` | `IAnimationEffect` |

Effects follow the host artifact's effects at the same timing through the existing scoring and animation pipeline. Custom callbacks stop while the host is broken, temporary, suppressed, or disabled by rules such as the Travel Bag. This API does not supply custom sale prices, inventory capacity rules, or natural generation weights.

Each artifact owns an independent affix instance through `artifact.Affix`. The host's `SubscribeToPlayer/UnsubscribeToPlayer` binds and unbinds its `ArtifactEffectEvent.Self/Tile/Block/RoundEnd` handlers. Builder signatures stay unchanged and mods do not need extra subscriptions; host subscription overrides must continue calling `base`. Each host publishes its event after collecting its own effects, and its affix responds only to that host, following inventory subscription order. Loading restores the property before host activation binds it. Handler failures propagate to the caller and stop the current collection.

Builtin capacity, sale restrictions and snapshot transfers use synchronous instance query events. Querying a prospective output does not obtain or remove artifacts or change live subscriptions. Lucky responds to a weight event carrying the draw source. Custom affixes continue using the Builder callbacks listed here; these internal queries do not add Lua capacity or draw callbacks.

Builtin Lightweight adds one Artifact slot and halves its host's Fu and Fan effects; it no longer removes a Gadget slot. `ScoreEffect` publishes `ArtifactScoreValueEvent` for display and execution. Lightweight modifies only its host's value: additive Fu/Fan values are divided by two, while multipliers become `1 + (multiplier - 1) / 2` (X2 → X1.5). Mod artifacts should also produce Fu/Fan effects through `ScoreEffect`; directly changing the accumulated score bypasses this query. Money and stored growth are unaffected.

Round preparation uses the separate `PlayerRoundEvent.Start.Prepare` phase. The game publishes this event; mods should not publish it or invoke the preparation entry point themselves. The current save format retains the preparation marker to prevent duplicate execution; migration of older affix settlement markers is not provided. Round-end affixes append effects through the host's `ArtifactEffectEvent.RoundEnd` and follow the regular round-end pipeline.

## Growth and saves

`player:GetArtifactAffixId(artifact)` returns a custom ID, a builtin affix's lowercase English name, or an empty string for no affix. The original `GetArtifactAffix` still returns only the builtin enum.

The affix instance owns its ID, name, description and growth data. Existing Player methods and Builder signatures remain available. Writes through `player:SetArtifactAffixData` still check ownership, crafting and pending evolution restrictions, then invalidate the scoring cache. Missing-content checks only read saved snapshots; they do not initialize or rewrite absent legacy fields.

Store string data with `GetArtifactAffixData(artifact, key, fallback)` and `SetArtifactAffixData(artifact, key, value)`. Pass `nil` to remove a key. Data belongs to each player's held artifact; never keep growth on the Builder or in global Lua variables. `player:ClearArtifactAffix(artifact)` removes the affix and its data.

Effect collection and descriptions can run repeatedly for previews. Append effects without changing growth during collection. Put changes in a `SimpleEffect` and check that the affix has not been replaced. For example, append this inside `OnRoundEndEffect`:

```lua
effects:Add(CS.Aotenjo.SimpleEffect("example_enchants_growth", artifact, function(p)
    if p:GetArtifactAffixId(artifact) ~= "example.enchants:ember" then return end
    local n = tonumber(p:GetArtifactAffixData(artifact, "growth", "0"))
    p:SetArtifactAffixData(artifact, "growth", tostring(n + 1))
end))
```

Use `WithDescription(function(player, loc, artifact) ... end)` for a dynamic description reading the stored growth. `loc(key)` resolves the current language; descriptions must have no side effects. The example's `example_enchants_growth` is an effect text key and also needs a translation.

The affix ID, owning mod, and growth data are saved with the player. Evolution preserves an independent snapshot and rejects affix/data edits while waiting. Removal and sale clear the data; reacquisition starts without the affix. Crafting excludes unaffixed materials and uniformly selects one distinct affix ID: A + none gives A, A + A gives A without stacking, and A + B gives each a 50% chance. If the same custom ID has different growth values, the first source in recipe material order supplies a deep copy; growth is neither combined nor added. Consumed materials lose their old data. The selected source is frozen before admission and saved, so capacity/admission failures preserve materials, output state and pending rewards, and retrying or reloading does not reroll the same combination. Capacity includes the inherited Lightweight affix, temporary slots and reserved artifact/gadget rewards. A missing definition or changed owner blocks resuming and writing the affected run with the missing ID shown. Restore the correct mod before loading again; data is never silently removed. Legacy saves without these fields have no custom affixes.
