# Texture packs and tile-face colors

[Contents](README.md) · [中文](../zh/textures.md) · [Installable texture pack](../../example/mods/ex5_texture_pack) · [Color example](../../example/mods/ex6_face_colors)

## Replace an image without Lua

The complete example replaces Origami Bear with an existing tutorial coin icon, making the change obvious. Real PNG files are included; no missing artwork needs to be supplied.

```text
ex5_texture_pack/
├─ modinfo.json
├─ img.png
├─ texture-pack.json
└─ texture_pack/origami_bear.png
```

The complete `texture-pack.json`:

```json
{
  "formatVersion": 1,
  "replacements": [
    {
      "target": "artifact:aotenjo:origami_bear",
      "file": "texture_pack/origami_bear.png",
      "filterMode": "Point",
      "pixelsPerUnit": 100
    }
  ]
}
```

Copy the folder into `StreamingAssets/mods` and fully restart. Look for `Loaded 1 texture pack replacement(s)`. Find Origami Bear in the collection, or use `give origami_bear` in a test run. Its icon should be the coin while its name, description, and mechanics remain unchanged. Remove the folder and restart to restore the original. A parsed manifest does not prove a visible target match; inspect the result.

## Manifest fields and targets

| Field | Type/default | Rule |
| --- | --- | --- |
| `formatVersion` | Integer; explicitly 1 in the example | Only 1 is supported |
| `replacements` | Object array | Entries are processed independently |
| `target` | Required nonempty string | An existing target in one of the forms below |
| `file` | Required nonempty string | Mod-relative PNG/JPG/JPEG; no absolute paths or escaping `../` |
| `filterMode` | Optional | `Point` for pixel art; `Bilinear` or `Trilinear`; omitted preserves whole-texture filtering and defaults standalone sprites to Point |
| `pixelsPerUnit` | Number, default 100 | New sprites only; nonpositive values fall back to 100 |

| Target form | Example | Size rule |
| --- | --- | --- |
| Artifact registration | `artifact:aotenjo:origami_bear` | Standalone image; arbitrary size, preferably matching aspect and pixel density |
| Built-in material registration | `tile_material:aotenjo:plain_material` | Standalone sprite; keep the built-in suffix |
| Another mod's registration | `artifact:tutorial_artifact:coin_twos` | The target mod must be installed |
| Resources path | An extensionless `resource_path` from the [texture catalog](../../reference/textures.csv) | Whole texture must match both dimensions |
| Texture2D name | A catalog `texture_name` | Whole texture, matching dimensions; duplicate names can affect several objects |
| Sprite name | Actual name, or `resourcePath/spriteName` to disambiguate | Standalone sprite; a target already matched as a whole texture is not also treated as a sprite |

Do not guess targets from translated display names. Use an artifact's `GetSpriteNamespaceID` and the documented material naming rule; individual types can override these. Whole-texture replacement preserves existing objects and slice coordinates, so **do not resize an atlas or move its cells**. Preserve untouched regions too. Standalone replacement sprites do not inherit nine-slice borders; use whole-texture replacement for stretchable UI. The catalog gives source PNG dimensions; if a platform downscales an import, use the runtime dimensions reported in the log.

Target matching is case-insensitive, but filesystem paths can be case-sensitive. Workshop mods load first, then local mods, with each group sorted by directory path. Later replacements of the same target win and log a conflict. A mixed mod can include `texture`, `lang`, `script`, and the manifest together. There is no hot reload.

## Tile-face colors without image files

The color example makes `plain` teal and `blue` rainbow. Start a run to see ordinary faces change; use `setFont 0 blue` for the rainbow. This changes appearance without altering the font's scoring effect. `plain` and `plain_font` are equivalent; the suffix is normalized and re-registering an ID replaces its style.

The following are static methods on `CS.Aotenjo.TileFaceMaterialRegistry`, taking strings, booleans, and numbers. Registration and unregistration methods return booleans.

| Method and arguments | Purpose |
| --- | --- |
| `RegisterHex(fontId, color)` | `#RGB`, `#RGBA`, `#RRGGBB`, or `#RRGGBBAA` |
| `RegisterColor(fontId, r, g, b, a)` | RGBA components from 0 to 1 |
| `RegisterOriginal(fontId)` | Original colors with default depth |
| `RegisterRainbow(fontId)` | Default spectrum gradient |
| `RegisterHexWithDepth(fontId, color, strength, pixels, threshold)` | Solid color and depth |
| `RegisterColorWithDepth(fontId, r, g, b, a, strength, pixels, threshold)` | The RGBA equivalent |
| `RegisterOriginalWithDepth(fontId, strength, pixels, threshold)` | Original colors and depth |
| `RegisterRainbowWithSettings(fontId, hueStart, hueRange, saturation, value, strength, pixels, threshold)` | Hue range, saturation, brightness, and depth |
| `SetSwapRedGreen(fontId, enabled)` | Combine red/green swapping with a style |
| `SetSwapRedGreenWithSaturation(fontId, enabled, saturation)` | Set swap target saturation |
| `SetSwapRedGreenAppearance(fontId, enabled, saturation, brightnessBoost)` | Also control green-to-red brightness boost |
| `Unregister(fontId)` | Remove the style and show the source sprite |

Use 0–1 for strength, threshold, saturation, and brightness; 1–4 for depth pixels; and -1–1 for hue range, with negative values reversing it. Gradients use each sprite's own UV range. Registering a `my_font` style does not make `TileFont.GetFont("my_font")` succeed; there is no new-font registration Builder. Visually test combinations with runtime effects such as 3D glasses.
