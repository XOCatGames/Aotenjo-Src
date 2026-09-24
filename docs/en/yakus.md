# Custom yakus

[Contents](README.md) · [中文](../zh/yakus.md) · [Complete All Twos example](../../example/mods/ex2_pattern) · [Implementation](../../src/API/Yaku/CustomYakuBuilder.cs)

A pure predicate decides whether a yaku applies; the game calculates its Fan. The example checks that `perm:ToTiles()` contains at least one tile and every tile is a numbered two. Empty combinations return false. The predicate does not mutate the player, draw randomness, or award rewards.

## The ten registration arguments

`CustomYakuBuilder.RegisterCustomYaku(...)` returns a `YakuType`.

| Position | Argument | Example | Meaning |
| --- | --- | --- | --- |
| 1 | `id: string` | `tutorial_yaku:all_twos` | Unique raw ID; omit `custom_yaku:` |
| 2 | `baseFan: int` | 8 | Full-size base Fan |
| 3 | `growthFactor: double` | 1.5 | Size scaling for incomplete combinations, not a level-up multiplier |
| 4 | `levelingFan: double` | 3 | Level growth increment |
| 5 | `predicate(perm, player): bool` | Check all tiles | Read-only; can be evaluated frequently |
| 6 | `includedYakus: string[]` | Empty array | Yakus inherited by this yaku |
| 7 | `groups: string[]` | standard, galaxy | Available yaku ranges, not arbitrary deck aliases |
| 8 | `yakuCategories: int[]` | 2 | Existing pack list indices, starting at zero |
| 9 | `rarity: Rarity` | COMMON | The pool within each selected pack |
| 10 | `exampleTiles: string` | `222m222p222s222m22p` | Collection example, not proof that the predicate is correct |

See [Lua bridging](lua-bridge.md) for typed arrays. Use an empty `string[]` for no inheritance, not nil. Registration requires an initialized `RegisterManager`; run it from the mod's `init()`. Duplicate IDs throw. Restart fully between edits.

The first four packs are **0 Wind, 1 Forest, 2 Fire, 3 Mountain**. See the [pack catalog](../../reference/catalogs.md) for the full list. Ranges and pack indices are separate; for example, the extended range inherits standard. A deck's range compatibility still determines whether a yaku is available and scores.

## IDs and translations

The returned object's `ToString()` is `custom_yaku:tutorial_yaku:all_twos`. Its three translation keys are:

```text
yaku_custom_yaku:tutorial_yaku:all_twos_name
yaku_custom_yaku:tutorial_yaku:all_twos_description
yaku_custom_yaku:tutorial_yaku:all_twos_romaji_name
```

Do not pass that full ID back to `RegisterCustomYaku` or `YakuType(string)`, which would add the prefix again. Reference a registered custom yaku using its returned object or full string. Built-in yakus use the enum names in [FixedYakuType](../../src/HandAndTile/Yaku/FixedYakuType.cs), such as `PingHu`.

## Inheritance does not call the parent predicate

`A.includedYakus = [B]` means A inherits B. When A applies, normal scoring display removes qualifying direct and transitive descendants. Each predicate still runs independently; A's condition must actually imply B. `SkillSet.CalculateInheritedFan` handles upgrade inheritance. Do not simply add B's full Fan to A.baseFan.

Register prerequisites first, or append relationships after registration:

```lua
-- advanced and prerequisite are already registered YakuType objects.
local inherited = CS.System.Array.CreateInstance(typeof(CS.Aotenjo.YakuType), 1)
inherited[0] = prerequisite
CS.Aotenjo.CustomYakuBuilder.AddInheritanceRelation(advanced:ToString(), inherited)
```

The first argument identifies the inheritor to modify; built-in and full custom IDs work. This merges and deduplicates direct relationships without registering anything. Avoid self-reference, cycles, and multiple paths to the same ancestor: recursive scoring has no general cycle/ancestor deduplication protection. Check direction with `YakuTester.IncludeYaku(advanced, prerequisite)`.

## Verification

Start a run with a standard-range deck. In the test console, `upgradeYaku custom_yaku:tutorial_yaku:all_twos 1` **adds** one level rather than setting it to one. Play a block of twos and look for All Twos; include a non-two or an honor for a negative case. An incomplete combination may award less than 8 Fan because of size scaling. Also verify Fire-pack draws, the collection example, both languages, and inherited hiding/upgrade contributions. See the [test matrix](testing.md).
