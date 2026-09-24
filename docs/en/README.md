# From an idea to a working mod

[中文](../zh/README.md) · [Repository home](../../README.md)

You need Lua variables, functions, conditions, loops, and tables. The seven examples do not require Unity, a C# compiler, or shader knowledge. We explain C# objects and xLua conversions when they become relevant.

## Learning path

1. [Quick start](quickstart.md): find the installation directory, copy files, read the log, and create an artifact.
2. [Lua and game objects](lua-bridge.md): understand `CS`, dots and colons, arrays, callbacks, and queued effects.
3. Choose a feature: [artifacts](artifacts.md), [yakus](yakus.md), [tile materials](materials.md), [recipes](recipes.md), or [textures and colors](textures.md).
4. [Images and bilingual text](assets.md): connect names, descriptions, and icons.
5. [Gameplay cookbook](cookbook.md): combine conditions, values, randomness, and persistent state.
6. [Test, troubleshoot, publish](testing.md): verify deterministic inputs before sharing.

## Supported capabilities

| Idea | Current entry point | Boundary |
| --- | --- | --- |
| Tile scoring, money, discard/round-end effects, growing artifacts | `LuaArtifactBuilder` | Callback signatures differ; use the supplied player |
| Custom yaku conditions, inheritance, membership in existing packs | `CustomYakuBuilder` | Conditions, ranges, pack indices, and inheritance are separate |
| Tile material mechanics and selectable material sets | `LuaTileMaterialBuilder`, `LuaMaterialSetBuilder` | Registering a material does not apply it to a tile |
| Combine several distinct artifacts | `LuaArtifactRecipeBuilder` | Inputs represent item presence, not item quantities |
| Replace existing images | `texture-pack.json` | Appearance only; whole textures require matching dimensions |
| Recolor existing fonts, add gradients or depth | `TileFaceMaterialRegistry` | A visual style does not create a new font's gameplay rules |
| New bosses, gadgets, decks, locations, or yaku pack types | No corresponding Lua Builder | A C# class alone does not supply Lua registration, copying, and save support; this handbook does not invent those APIs |

Public C# methods can be called from Lua, but implementation details are not a permanent modding contract. Prefer the smallest supported extension point. Consult the [source signature index](../../reference/api-signatures.md) and its source links. An idea outside the table may need a new game API; renaming a Lua function cannot create one.

## A practical graduation task

Copy the artifact example, change all IDs to your own prefix, and change “played twos earn 2 coins” into “played nines add 15 Fu.” Update both translations. Use `setHand 999m123p` and your `give` command to check that nines trigger, other tiles do not, and previously settled tiles do not trigger again. Restart the game, install another example alongside it, and complete the [acceptance checklist](testing.md). This covers the entire path from an idea through conditions and effects to a shareable mod.
