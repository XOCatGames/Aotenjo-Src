# Testing, troubleshooting, and publishing

[Contents](README.md) · [中文](../zh/testing.md) · [Validation record](../VALIDATION.md)

## Version and log checks

This handbook targets a [source snapshot](../SOURCE.md), not every historical release. Installing a mod cannot upgrade the game. Start with only `00_hello`, then add one example at a time. A source sync does not mean Steam has shipped that code. An older client may even ignore a texture-pack manifest silently.

Temporarily probe a feature from your `init()`:

```lua
local ok, result = pcall(function()
    return typeof(CS.Aotenjo.TileFaceMaterialRegistry)
end)
CS.Aotenjo.Logger.Log("TileFaceMaterialRegistry available: " .. tostring(ok and result ~= nil))
```

A type probe does not establish every method's platform binding. Execute the example next. The color example checks registration return values with `assert`; inspect the detailed Lua error if it fails.

Find AML logs as described in [quick start](quickstart.md). Localization failures use Unity Debug and may appear only in `Player.log` under the same persistentDataPath, so inspect both. `No textures directory found` can be harmless for a script-only mod; focus on Lua exceptions and missing success markers.

## Enable a test console

Temporarily add `CS.Constants.CONSOLE_ENABLED = true` inside the test mod's `init()`, fully restart, enter a fresh test run, and press F2. Constants is a global runtime type absent from this public snapshot. Older builds or other platforms may lack this switch. Remove the line if it errors; test available features through normal gameplay/collections and record the build version. Do not modify a real save to work around this. Remove the switch from release packages.

| Command | Example | Exact meaning |
| --- | --- | --- |
| `give` | `give tutorial_artifact:coin_twos` | Obtain a registered artifact; normal slot/rule checks apply |
| `addSlot` | `addSlot 3` | Add three artifact slots for combination tests |
| `setHand` | `setHand 222m333p` | Replace hand; m=characters, p=circles, s=bamboo, z=honors; do not run while scoring |
| `setMat` | `setMat 0-2 tutorial_gems:ruby` | Set indices 0, 1, 2; both endpoints are included |
| `setFont` | `setFont 0 blue` | Change the font of hand tile 0 |
| `earn` | `earn 20` | Add 20 coins |
| `upgradeYaku` | `upgradeYaku custom_yaku:tutorial_yaku:all_twos 1` | Add one level, not set level to one |
| `seed` | `seed` | Show the current seed |
| `help` | `help` | List commands available in this build |

Use the installed build's help and errors as the final authority. Legacy commands may have different index/range behavior; do not apply setMat's inclusive range rule to an older setCorrupted implementation.

## Acceptance matrix for the seven example mods

Test each mod independently and then together, restarting between changes. These are expected results to verify, **not a claim that repository tests have already completed in-game acceptance**.

| Example | Action | Expected result |
| --- | --- | --- |
| Hello | Launch; inspect AML | One `[tutorial_hello] Hello / 你好` per launch |
| Coin of Twos | Give it; setHand 222m123p; play each group | Newly played twos earn 2 coins each; other tiles and previously settled twos do not |
| Practice Jade | give tutorial_artifact:practice_jade; score twice | ×1.0, then ×1.1; repeated description queries do not grow it |
| All Twos | Upgrade the custom yaku; play 222m, then include non-twos | All-twos combination qualifies; mixed combination fails; collection and Fire pack include it |
| Gemstones | setMat; score; finish round; select Tutorial Gemstones for a new run | Ruby adds 20 Fu, Sapphire multiplies Fan by 2; processed round-end Rubies earn 2 coins; set pools offer the materials |
| Recipe | Give Copper then Silver Token | Inputs disappear, Double Token appears and adds 5 Fan; a single input does not craft |
| Texture pack | Inspect/obtain Origami Bear | Coin icon, unchanged name/mechanics; removal plus restart restores it |
| Face colors | Start a run; setFont 0 blue | Plain faces are teal and blue faces rainbow; removal plus restart restores them |

Then check matching Chinese/English text, no duplicate subscriptions, intended forced/ordinary discard behavior, growth after save/exit/resume, fresh-run reset, independent material state on different tiles, and coexistence with another mod. Both init functions should execute once. Back up a save that does not depend on new content and keep test saves separate.

## Common failures

| Symptom | Check and remedy |
| --- | --- |
| Mod not listed | Extra directory level, invalid modinfo.json, hidden file extensions |
| Listed but no content | AML Lua errors; global init function; missing BuildAndRegister |
| Another mod's behavior appears | Shared require cache; replace generic util modules with unique prefixes |
| Duplicate dictionary key | Local and Workshop copies both installed, repeated IDs, calling init twice |
| Wrong icon | Match modID, Create ID, and filename; direct PNG child in the correct folder |
| Description shows a key | Locale filename, exact key, custom_yaku prefix; inspect Player.log |
| Too many triggers | Missing Selecting check, confusing scoring stages with runs, state mutation in queries |
| Nil player or shifted arguments | Collection player may be nil; compare the exact callback signature |
| Yaku absent from packs/scoring | Initialization, groups, pack index, rarity, unlock level, predicate |
| Material registered but unseen | Register/select a set, populate both pools, test directly with setMat |
| Texture log succeeds without visible change | Actual target match, dimension errors, later overrides, full restart |
| Save/resume loses state or behavior | Stable IDs, string SetData values, installed dependencies; test material delegate restoration separately |

## ZIP packages and Workshop

Share a ZIP containing your mod's root folder. After extraction, the layout should be `mods/your_folder/modinfo.json`. Do not include the game executable, src, Unity Library, logs, or personal saves. Each example includes `img.png`; replace it with your preview. Keeping it within 512×512 is this tutorial's recommendation, not a server limit verified from this source.

After validating the local mod, select it in the game's mod manager and use Upload to Workshop. The uploader reads name, description, the complete mod folder, and root-level `img.png`. Steam must be connected; complete any creator agreements Steam requests. New items are created with Private visibility; review the item page before making it public.

Record the first upload's item ID. For updates, add `"workshopId": "your item ID"` to the same manifest and upload again. Omitting it creates a new item. Use only your own item ID. Uploading does not validate Lua.

Before publishing, complete the matrix, remove debug switches, and describe the tested game build, mod version, dependencies, save compatibility, and changes. Finally, extract the exact ZIP you will share into a clean mods directory and test once more.
