# Images, metadata, and bilingual text

[Contents](README.md) · [中文](../zh/assets.md)

## modinfo.json

Provide five strings: `modID`, `name`, `version`, `author`, and `description`. Use a unique ASCII prefix such as `alice_lucky_tiles`. The modID participates in sprite namespaces; when changing it, update Lua IDs and localization keys too. The folder name does not automatically become the modID. Keep version numbers out of stable content IDs.

`workshopId` is an optional string for updating your existing Workshop item; omit it on first upload. `modDir`, `isFromWorkshop`, and `itemUrl` are runtime data. There is no manifest dependency resolver, version constraint, or enable switch. Document and check dependencies explicitly rather than inventing a `dependencies` feature.

## Custom icons

The ordinary asset loader scans direct `*.png` children of exactly four directories, without recursion:

```text
texture/artifact/
texture/tile_material/
texture/tile_style/
texture/tile_mask/
```

Registration is `type:modID:filename_without_extension`. Icons use Point filtering, Clamp wrapping, 100 pixels-per-unit, and a centered pivot. Preserve transparency and use the example dimensions and silhouette as a starting point. The directory is `texture`, not `textures`. Loading a `tile_style` image does not register new font mechanics.

Do not share modIDs between mods. Change the object ID, image path, and translation keys together. A mismatch produces missing translations or fallback artwork. Example artwork comes from this repository's original public examples; see [artwork provenance](../../example/ARTWORK.md).

## Language files

`lang/zh-CN.json` and `lang/en-US.json` are flat key-to-string dictionaries, not nested objects. Save UTF-8 JSON without comments or trailing commas. The loader currently recognizes `zh-CN`, `en-US`, `zh-TW`, `ja-JP`, `ko-KR`, `de-DE`, `fr-FR`, `es-ES`, and `ru-RU`. Do not rename the Chinese file to `zh-Hans.json`.

Each file merges into both GameTable and YakuInfo, overriding existing values with matching keys. Prefix all custom keys. Translating existing built-in locales is the reliable path; adding a file for a new locale does not establish language-selection UI support.

| Content | Default keys |
| --- | --- |
| Artifact | `artifact_<full artifact ID>_name`, `artifact_<ID>_description` |
| Shop description | `artifact_<ID>_description_inshop`, falling back to the ordinary description |
| Tile material | `tile_<regName including _material>_name`, `..._description`, `..._name_short` |
| Material set | `material_set_<set regName>_name` |
| Custom yaku | `yaku_custom_yaku:<raw ID>_name`, `..._description`, `..._romaji_name` |
| Custom TextEffect/SimpleEffect | A unique key such as `tutorial_artifact_grow` |

`loc(key)` returns localized text. For dynamic descriptions use `string.format(loc(key), number)` with Lua placeholders such as `%.1f` or `%d`, not C# `{0}` placeholders. Prefer default name localization over `WithName`, which has no localizer argument.

Start with the Chinese and English files. An untranslated active language may fall back or show the key; do not assume automatic English fallback. Actually switch between Simplified Chinese and English and check matching meaning, values, and punctuation. Fully restart after language-file edits.
