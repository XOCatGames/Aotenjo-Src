# Register glossary terms with Lua

[Contents](README.md) · [中文](../zh/terms.md) · [Images and localization](assets.md)

**Version requirement: `LuaTermBuilder.Register` is available only in Aotenjo 0.7.1.0 and later. It is not supported in 0.7.0.x.**

Terms can appear with a color in artifact, yaku, and tile-material descriptions. Hovering a term shows its title and explanation. This guide adds an “Overload” term to an existing mod without creating Unity `.asset` files.

## Register in init.lua

Add the registration inside your existing `init()` in `script/init.lua`. If the file does not have an `init()` yet:

```lua
function init()
    local ok = CS.Aotenjo.LuaTermBuilder.Register("tutorial_terms_overload", "purple")
    if not ok then
        print("Overload registration failed; check [LuaTermBuilder] in the log")
    end
end
```

Replace the `tutorial_terms` prefix with your own mod prefix. Term IDs use underscores; unlike other Builders, this API does not accept the `modID:name` notation with a colon. Available colors are `yellow`, `purple`, `green`, `blue`, and `red`, using the current UI's palette.

## Add the title and explanation

Merge these keys into `lang/zh-CN.json`, keeping existing names and descriptions:

```json
{
  "tooltip_tutorial_terms_overload_header": "过载",
  "tooltip_tutorial_terms_overload_content": "本次激活效果翻倍。"
}
```

Add the corresponding translations to `lang/en-US.json`:

```json
{
  "tooltip_tutorial_terms_overload_header": "Overload",
  "tooltip_tutorial_terms_overload_content": "Double this activation's effect."
}
```

These remain flat JSON dictionaries. The loader reads language files before executing `init.lua`. Missing keys follow existing localization behavior and may appear as raw key names; registration does not generate translations.

## Use the term in a description

```text
Gain <style="tutorial_terms_overload">Overload</style>.
```

Escape the quotes when storing this description in JSON:

```json
{
  "artifact_tutorial_terms:token_description": "Gain <style=\"tutorial_terms_overload\">Overload</style>."
}
```

Replace the sample description key with your own artifact or material key. Registration supplies a color and hover link; the displayed word still comes from the description in each language. The text object must already have the term-hover component used by the game's relevant descriptions. The API does not add interaction to arbitrary new UI.

## Parameters and registration rules

`bool CS.Aotenjo.LuaTermBuilder.Register(string id, string color)` is **available only in 0.7.1.0 and later**.

| Parameter or result | Rule |
| --- | --- |
| `id` | Starts with an ASCII letter, contains only ASCII letters, digits, and underscores, and is at most 128 characters. Normalized to lowercase; use lowercase style tags and localization keys. |
| `color` | `yellow`, `purple`, `green`, `blue`, or `red`; case-insensitive. |
| Return value | `true` on success; `false` on failure with a `[LuaTermBuilder]` warning. |

Duplicate IDs, existing styles, and base color names are rejected, preserving the existing definition. The `pattern_`, `tilemat_`, `tilefont_`, and `boss_reversed_effect_` prefixes, and the ID `jixiepai`, are reserved for specialized tooltips.

Register in `init.lua` before displaying descriptions using the new style. Terms exist only for the current load session and do not modify saves or source style assets. Mod reinitialization, game exit, Play Mode exit, and editor reload clean up the runtime style copy. This API does not provide dynamic explanation callbacks or hot unloading.

## Check after restarting

1. Confirm **0.7.1.0 or later**, then fully restart to reload the mod.
2. Open the description and check the term color and absence of raw markup.
3. Hover the term and verify the title and explanation.
4. Switch between Simplified Chinese and English, reopening the description and tooltip.
5. Check text after the term and outer emphasis for restored fonts, colors, and link boundaries.

Source: [LuaTermBuilder](../../src/API/Terms/LuaTermBuilder.cs). See [provenance](../SOURCE.md) for the version-specific source supplement.
