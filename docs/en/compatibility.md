# Declare supported game versions

[Contents](README.md) · [中文](../zh/compatibility.md) · [Quick start](quickstart.md)

Add `supportedGameVersions` to the `modinfo.json` in your mod's root folder. Each entry declares a range using `minInclusive` and `maxInclusive`, including both endpoints. Set both endpoints to the same value to support one version.

## Complete example: one version

This example uses `0.8.0.0-PRE1`. Replace it with the game version you have tested for your own mod.

```json
{
  "modID": "example.content",
  "name": "Example Content",
  "version": "1.2.0",
  "artifactApiVersion": 2,
  "supportedGameVersions": [
    { "minInclusive": "0.8.0.0-PRE1", "maxInclusive": "0.8.0.0-PRE1" }
  ],
  "author": "Your name",
  "description": "Example mod"
}
```

| Field | Value |
| --- | --- |
| `version` | Your mod's own version; increment it when updating the mod |
| `supportedGameVersions` | Supported game version ranges; at least one entry is required |
| `artifactApiVersion` | Artifact interface version; use integer `2` for the current runtime-instance API, as described in the [API v2 migration guide](../ArtifactRuntimeApiV2.md) |

The downloadable examples are pinned to this source snapshot's configured version, `0.8.0.0-PRE1`. If your installed game version differs, follow [Testing and publishing](testing.md) to check the required interfaces and effects before changing the range.

## Continuous and separate ranges

This syntax example includes `0.7.1.0-PRE8` through the `0.7.1.0` release in the first range, and only `0.8.0.0-PRE1` in the second. Merge this field into your `modinfo.json` and choose ranges based on your testing.

```json
{
  "supportedGameVersions": [
    { "minInclusive": "0.7.1.0-PRE8", "maxInclusive": "0.7.1.0" },
    { "minInclusive": "0.8.0.0-PRE1", "maxInclusive": "0.8.0.0-PRE1" }
  ]
}
```

The current game version only needs to match one range, but every entry must be valid. This example skips intermediate versions such as `0.7.2.0-PRE1`.

## Version format and comparison

- Three or four numeric components are supported: `0.7.1` equals `0.7.1.0`.
- Numeric components are compared first, then preview numbers. For the same numeric version, `PRE8 < PRE9 < PRE10 < release`; `0.7.1.0-EA` equals `0.7.1.0`.
- A range containing only a release version excludes its `PRE` previews. Set the lower endpoint to the appropriate `PRE` version when supporting previews.
- Build suffixes `-debug` and `-console-only` do not affect comparison: `0.8.0.0-PRE1-debug` equals `0.8.0.0-PRE1`. Normally declare the base game version.
- Both endpoints require concrete versions. Wildcards, `>=`, `^`, `~`, a `v` prefix, empty endpoints, and other custom suffixes are unsupported.

## Apply changes

After saving `modinfo.json`, fully exit and restart the game, then check the mod manager. Missing or empty `supportedGameVersions`, invalid versions, reversed ranges, an unsupported current version, and duplicate `modID` values block loading before scripts, translations, and textures run.

Compatibility checks preserve the player's enable preference. When the version becomes compatible again, that preference applies. Manually enabling or disabling a mod also requires a restart.

Implementation: [manifest fields](../../src/API/Mod/Mod.cs) · [version parsing and range comparison](../../src/API/Mod/GameVersion.cs) · [checks before loading](../../src/API/Mod/ModCatalog.cs).
