# Quick start: an empty folder to Coin of Twos

[Contents](README.md) · [中文](../zh/quickstart.md) · Next: [Lua and game objects](lua-bridge.md)

## Run a complete example first

1. Open Aotenjo's installation folder from Steam (Manage → Browse local files), then close the game.
2. On Windows, find `Aotenjo_Data/StreamingAssets/mods/`; create `mods` if absent. The loader uses `Application.streamingAssetsPath/mods`, not the save directory. Other platforms use their platform-specific StreamingAssets location.
3. Download this repository with **Code → Download ZIP** and extract it. Copy `example/mods/00_hello` into `mods`. Its immediate child must be `modinfo.json`; avoid an extra repository folder level.
4. Launch the game and open its mod manager. Look for `Hello / 你好`.
5. Open the newest `AML_*.log` in `%USERPROFILE%/AppData/LocalLow/Aotenjo/Aotenjo/AML/` and find `[tutorial_hello] Hello / 你好`. If a build uses different company/product names, use its `Application.persistentDataPath/AML` location.

An entry in the mod manager does not prove that Lua succeeded. The loader catches Lua exceptions and still lists the mod. Check the success marker and search for `Failed to load Lua script`.

## Write your first artifact

Create this layout. Save files as UTF-8 and show file extensions so you do not accidentally create `init.lua.txt` or `modinfo.json.txt`.

```text
mods/
└─ my_first_mod/
   ├─ modinfo.json
   ├─ script/init.lua
   ├─ lang/zh-CN.json
   ├─ lang/en-US.json
   └─ texture/artifact/coin.png
```

`modinfo.json`:

```json
{
  "modID": "my_first_mod",
  "name": "My First Mod / 我的第一个模组",
  "version": "1.0.0",
  "author": "Your name",
  "description": "Played twos earn 2 coins / 打出的二获得2金币"
}
```

The complete `script/init.lua`:

```lua
local A = CS.Aotenjo
function init()
    A.LuaArtifactBuilder.Create("my_first_mod:coin", A.Rarity.COMMON)
        :OnTileEffect(function(player, perm, tile, effects, artifact)
            if player:Selecting(tile) and tile:IsNumbered(2) then
                effects:Add(A.EarnMoneyEffect(2, artifact))
            end
        end)
        :BuildAndRegister()
    A.Logger.Log("[my_first_mod] ready")
end
```

`lang/zh-CN.json`:

```json
{
  "artifact_my_first_mod:coin_name": "二号硬币",
  "artifact_my_first_mod:coin_description": "每张本次打出的二结算时获得2金币。"
}
```

`lang/en-US.json`:

```json
{
  "artifact_my_first_mod:coin_name": "Coin of Twos",
  "artifact_my_first_mod:coin_description": "Gain 2 coins whenever a newly played two scores."
}
```

Copy [coin_twos.png](../../example/mods/ex1_artifact/texture/artifact/coin_twos.png) to `texture/artifact/coin.png`. The loader registers it as `artifact:my_first_mod:coin`, matching the artifact ID. Your mod is now complete.

## Verify the result

Fully restart after each edit. Find `[my_first_mod] ready` in the log and check both language versions of the collection entry. For deterministic testing, temporarily add `CS.Constants.CONSOLE_ENABLED = true` inside `init()`, restart, enter a new test run, press F2, and enter:

```text
give my_first_mod:coin
setHand 222m123p
```

Play the triplet of twos. Each two should add 2 coins. Play the other group next: the already settled twos should not earn money again from this artifact. If your build lacks `Constants.CONSOLE_ENABLED`, remove that line and consult [compatibility troubleshooting](testing.md). Remove the debug switch before publishing.

Now change the condition from `2` to `9`, replace `A.EarnMoneyEffect(2, artifact)` with `A.ScoreEffect.AddFu(15, artifact)`, update both descriptions, and test nines. The structure is **trigger → condition → effect → registration**.

## Loading rules

Images and `lang` load first, followed by top-level code in `script/init.lua`, then its global `init()` function. Do not call it a second time yourself. All script mods share one Lua environment. Every script mod should define its own `init()`, keep other variables `local`, and use module names such as `require("my_first_mod.helpers")` for `script/my_first_mod/helpers.lua`. A generic `require("util")` may return another mod's cached module.

A pure texture pack can omit `script`; see [texture packs](textures.md). Disable a local mod by moving its folder out of `mods` and restarting. The loader does not read an `enabled` manifest field.
