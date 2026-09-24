# Artifacts: conditions, effects, and growth

[Contents](README.md) · [中文](../zh/artifacts.md) · [Complete mod](../../example/mods/ex1_artifact) · [Implementation](../../src/API/Artifact/LuaArtifactBuilder.cs)

Run Coin of Twos first, then inspect Practice Jade in the same mod. The first filters newly played twos; the second demonstrates formatted descriptions, effect queues, and string state. All callbacks are optional; omitted callbacks retain base behavior.

## Creation and registration

`LuaArtifactBuilder.Create("your_mod_id:item_name", rarity)` returns a builder. Chained setters return that same builder.

| Method | Return value and purpose |
| --- | --- |
| `Build()` | Creates a `LuaArtifact` without registering it |
| `BuildAndRegister()` | Creates and adds to `Artifacts.ArtifactList`; use for ordinary artifacts |
| `BuildCraftable()` | Creates an unregistered `LuaCraftableArtifact` |
| `BuildAndRegisterCraftable()` | Creates and registers a craft result; also register its [recipe](recipes.md) |

Register each ID once per startup. Registration does not replace duplicate IDs. Do not register every round or scoring step. `WithName` changes display text, not identity. Omitting it uses `artifact_ID_name`, which is preferable for translations.

## Display and availability callbacks

The parameter order below is the exact order for Lua `function(...)`.

| Method | Callback arguments → return | Purpose |
| --- | --- | --- |
| `WithHighlight` | `(tile, player, artifact) → bool` | Hover highlight, not an automatic effect trigger |
| `WithAvailability` | `(player, artifact) → bool` | Overrides the Player overload of global availability, not every deck/set overload |
| `WithDescription` | `(player, loc, artifact) → string` | Description; player may be nil |
| `WithInShopDescription` | `(player, loc, artifact) → string` | Shop description |
| `WithAdditionalInfo` | `(player, artifact) → (string,double)` | C# ValueTuple; return `A.Artifact.ToMulFanFormat(1.2)` directly |
| `WithName` | `(player, artifact) → string` | Name; no loc argument, currently receives nil player |
| `WithNameWithColor` | `(player, artifact) → string` | Styled name; also has no loc argument |
| `WithSubHeader` | `(player, artifact) → string` | Subtitle |
| `WithSpriteID` | `(player, artifact) → string` | Full registration name such as `artifact:tutorial_artifact:coin_twos`, not an integer |

`WithDeckIn(string[])` and `WithDeckBlocked(string[])` use deck regNames. `WithSetBlocked(string[])` excludes sets. `WithMaterialRequired(string[])` requires all listed materials in the current set. In the deck+set query, `WithSetIn(string[])` checks material overlap with each listed set, rather than being a simple set-ID whitelist. Beginners should prefer explicit material requirements. A custom `WithAvailability` overrides the Player overload's default checks; do not assume it automatically combines with these restrictions.

## Complete lifecycle and effect callbacks

`p=Player`, `perm=Permutation`, `a=current artifact`. `E` means `List<Effect>`; `AE` means `List<IAnimationEffect>`.

| Method | Argument order | When it runs |
| --- | --- | --- |
| `OnObtain` | `(p, a)` | On obtaining; base subscription has already run |
| `OnRemoved` | `(p, a)` | On removal, after base unsubscription handling |
| `PreGameInitialized` | `(p, a)` | New-run initialization visits all registered artifacts, including unowned ones |
| `ResetArtifactState` | `(p, a)` | Reset paths include a new run, removal requesting reset, and a failed run; not every round start |
| `OnSubscribeToPlayer` | `(p, a)` | Player binding, including possible load/rebind paths |
| `OnUnsubscribeToPlayer` | `(p, a)` | Player unbinding; remove matching subscriptions |
| `OnTileEffect` | `(p, perm, tile, E, a)` | Collect a tile's scoring effects |
| `OnTilePostEffect` | `(p, perm, tile, E, a)` | Collect post-tile effects |
| `OnUnusedTileEffect` | `(p, perm, tile, E, a)` | Collect unused-tile effects |
| `OnDiscardTileEffect` | `(p, tile, AE, withForce, isClone, a)` | Discard effects; no perm argument |
| `OnBlockEffect` | `(p, perm, block, E, a)` | Collect block effects |
| `OnBlockAnimEffect` | `(p, perm, block, AE, a)` | Collect block animation effects |
| `OnSelfEffect` | `(p, perm, E, a)` | Artifact's own scoring stage |
| `OnRoundEndEffect` | `(p, perm, AE, a)` | Collect round-end effects; tolerate an absent perm |

For “newly played,” check `p:Selecting(tile)`. Checking only the tile's number can also include existing tiles. Retrigger mechanics affect callback counts; `OnSelfEffect` is not a once-per-run callback.

## Growth and saves

The example stores `plays` as a string. Its description only reads state. `OnSelfEffect` queues the current multiplier and a `SimpleEffect` that increments the count when executed. `Serialize()` and `Deserialize(string)` on this Lua artifact write/read that dictionary. A full cross-process save/resume still needs testing in the game.

The artifact registry holds shared objects, rather than cloning a separate Lua artifact on every obtain. Do not use one ID for several independently stateful instances or assume isolated state for multiple players. Never reset saved state unconditionally in `OnSubscribeToPlayer`; rebinding is not a new run.

When adding fields, provide defaults and migrate older values when reading, for example `tonumber(a:GetDataOrDefault("level", "0")) or 0`. Renaming a published ID affects saved references. Changing the displayed name usually needs only a language-file edit.

Acceptance: test twos/non-twos and selected/unselected tiles; Practice Jade should use ×1.0 then ×1.1; collection views must not grow it; verify save/resume, removal, and a fresh-run reset. Successful registration does not establish these gameplay results.
