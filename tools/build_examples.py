"""Maintainer tool: rebuild the self-contained tutorial mods from checked-in sources.

Uses only the Python standard library. Artwork is reused from this repository's
original examples; no game installation or proprietary game assets are needed.
"""
from pathlib import Path
import json
import shutil

ROOT = Path(__file__).resolve().parents[1]
MODS = ROOT / 'example' / 'mods'

def write(folder, relative, text):
    path = MODS / folder / relative
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text.strip() + '\n', encoding='utf-8', newline='\n')

def manifest(folder, mod_id, name, description):
    write(folder, 'modinfo.json', json.dumps(dict(modID=mod_id, name=name,
          version='2.0.0', author='Aotenjo tutorial', description=description), ensure_ascii=False, indent=2))

def langs(folder, entries):
    for lang, column in [('zh-CN', 0), ('en-US', 1)]:
        write(folder, f'lang/{lang}.json', json.dumps({k: v[column] for k, v in entries.items()}, ensure_ascii=False, indent=2))

def copy(folder, relative, source):
    target = MODS / folder / relative
    target.parent.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(MODS / source, target)

manifest('00_hello', 'tutorial_hello', 'Hello / 你好', 'Startup log / 启动日志')
write('00_hello', 'script/init.lua', '''
-- The loader calls this once. 加载器会调用一次，不要在文件末尾再调用。
function init()
    CS.Aotenjo.Logger.Log("[tutorial_hello] Hello / 你好")
end
''')

manifest('ex1_artifact', 'tutorial_artifact', 'Artifacts / 藏品', 'Played twos earn 2 coins; a jade grows after scoring. / 打出的二获得2金币，练习玉在计分后成长。')
write('ex1_artifact', 'script/init.lua', '''
-- Prefix module names: all mods share package.loaded. 模块名必须带模组前缀。
local artifacts = require("tutorial_artifact.artifacts")
function init()
    artifacts.register()
    CS.Aotenjo.Logger.Log("[tutorial_artifact] registered 2 artifacts")
end
''')
write('ex1_artifact', 'script/tutorial_artifact/artifacts.lua', '''
local A = CS.Aotenjo
local M = {}

function M.register()
    A.LuaArtifactBuilder.Create("tutorial_artifact:coin_twos", A.Rarity.COMMON)
        :WithHighlight(function(tile, player, artifact)
            return tile:IsNumbered(2)
        end)
        :OnTileEffect(function(player, perm, tile, effects, artifact)
            -- Only newly played tiles, not previously settled ones. 只处理本次打出的牌。
            if player:Selecting(tile) and tile:IsNumbered(2) then
                effects:Add(A.EarnMoneyEffect(2, artifact))
            end
        end)
        :BuildAndRegister()

    A.LuaArtifactBuilder.Create("tutorial_artifact:practice_jade", A.Rarity.RARE)
        :ResetArtifactState(function(player, artifact)
            artifact:SetData("plays", "0")
        end)
        :WithDescription(function(player, loc, artifact)
            -- player can be nil in the collection UI. 图鉴里 player 可能是 nil。
            local plays = tonumber(artifact:GetDataOrDefault("plays", "0")) or 0
            return string.format(loc("artifact_tutorial_artifact:practice_jade_description"), 1 + plays * 0.1)
        end)
        :OnSelfEffect(function(player, perm, effects, artifact)
            local plays = tonumber(artifact:GetDataOrDefault("plays", "0")) or 0
            effects:Add(A.ScoreEffect.MulFan(1 + plays * 0.1, artifact))
            -- State changes when the queued effect executes, not while building the list.
            -- 排队完成后，真正执行效果时才修改存档状态。
            effects:Add(A.SimpleEffect("tutorial_artifact_grow", artifact, function(p)
                local current = tonumber(artifact:GetDataOrDefault("plays", "0")) or 0
                artifact:SetData("plays", tostring(current + 1))
            end))
        end)
        :BuildAndRegister()
end
return M
''')
langs('ex1_artifact', {
 'artifact_tutorial_artifact:coin_twos_name': ('二号硬币', 'Coin of Twos'),
 'artifact_tutorial_artifact:coin_twos_description': ('每张本次打出的二结算时获得2金币。', 'Gain 2 coins whenever a newly played two scores.'),
 'artifact_tutorial_artifact:practice_jade_name': ('练习玉', 'Practice Jade'),
 'artifact_tutorial_artifact:practice_jade_description': ('计分时番数×%.1f；随后倍率增加0.1。', 'Multiply Fan by %.1f when scoring; then increase the multiplier by 0.1.'),
 'tutorial_artifact_grow': ('熟能生巧！', 'Practice makes progress!')})
copy('ex1_artifact', 'texture/artifact/coin_twos.png', 'ex1_artifact/texture/artifact/coin_number_2.png')
copy('ex1_artifact', 'texture/artifact/practice_jade.png', 'ex1_artifact/texture/artifact/kong_jade.png')

manifest('ex2_pattern', 'tutorial_yaku', 'All Twos / 全二', 'A custom yaku with a pure predicate. / 使用纯判定函数的自定义番种。')
write('ex2_pattern', 'script/init.lua', '''
local A = CS.Aotenjo
local function array(t, values)
    local result = CS.System.Array.CreateInstance(typeof(t), #values)
    for i, v in ipairs(values) do result[i - 1] = v end
    return result
end

function init()
    A.CustomYakuBuilder.RegisterCustomYaku(
        "tutorial_yaku:all_twos", -- Do not add custom_yaku: here. 此处不加 custom_yaku:。
        8, 1.5, 3,
        function(perm, player)
            local tiles = perm:ToTiles()
            if tiles.Count == 0 then return false end
            for i = 0, tiles.Count - 1 do
                if not tiles[i]:IsNumbered(2) then return false end
            end
            return true
        end,
        array(CS.System.String, {}), -- No unproven inheritance. 不声明额外继承。
        array(CS.System.String, {"standard", "galaxy"}),
        array(CS.System.Int32, {2}), -- Fire / 火: index 2, NOT Forest / 不是林。
        A.Rarity.COMMON,
        "222m222p222s222m22p")
    A.Logger.Log("[tutorial_yaku] registered all_twos")
end
''')
langs('ex2_pattern', {
 'yaku_custom_yaku:tutorial_yaku:all_twos_name': ('全二', 'All Twos'),
 'yaku_custom_yaku:tutorial_yaku:all_twos_description': ('组合中至少有一张牌，且所有牌都是数牌二。', 'The combination contains at least one tile and every tile is a numbered two.'),
 'yaku_custom_yaku:tutorial_yaku:all_twos_romaji_name': ('Quan Er', 'Quan Er')})

manifest('ex3_tile_material', 'tutorial_gems', 'Gemstones / 宝石牌体', 'Ruby +20 Fu and 2 coins at round end; sapphire x2 Fan. / 红宝石加20符、关末加2金币，蓝宝石乘2番。')
write('ex3_tile_material', 'script/init.lua', '''
local A = CS.Aotenjo
function init()
    A.LuaTileMaterialBuilder.Create("tutorial_gems:ruby")
        :WithRarity(A.Rarity.COMMON)
        :OnScoringEffect(function(player, perm, tile, effects, material)
            effects:Add(A.ScoreEffect.AddFu(20, nil))
        end)
        :OnRoundEndEffect(function(player, perm, effects, tile, material)
            -- This list contains animation effects. 此处需要带牌目标的动画效果。
            effects:Add(A.EarnMoneyEffect(2, nil):OnTile(tile))
        end)
        :BuildAndRegister()

    A.LuaTileMaterialBuilder.Create("tutorial_gems:sapphire")
        :WithRarity(A.Rarity.RARE)
        :OnScoringEffect(function(player, perm, tile, effects, material)
            effects:Add(A.ScoreEffect.MulFan(2, nil))
        end)
        :BuildAndRegister()

    A.LuaMaterialSetBuilder.Create("tutorial_gemstones")
        :AddMaterial("tutorial_gems:ruby")
        :AddMaterial("tutorial_gems:sapphire")
        :AddMaterial("golden")
        :AddMaterial("crystal")
        :BuildAndRegister()
    A.Logger.Log("[tutorial_gems] registered 2 materials and tutorial_gemstones")
end
''')
langs('ex3_tile_material', {
 'tile_tutorial_gems:ruby_material_name': ('红宝石牌', 'Ruby Tile'),
 'tile_tutorial_gems:ruby_material_name_short': ('红宝石', 'Ruby'),
 'tile_tutorial_gems:ruby_material_description': ('计分时加20符；此牌参与关末效果时获得2金币。', 'Add 20 Fu when scoring. Gain 2 coins when this tile receives its round-end effect.'),
 'tile_tutorial_gems:sapphire_material_name': ('蓝宝石牌', 'Sapphire Tile'),
 'tile_tutorial_gems:sapphire_material_name_short': ('蓝宝石', 'Sapphire'),
 'tile_tutorial_gems:sapphire_material_description': ('计分时番数乘2。', 'Multiply Fan by 2 when scoring.'),
 'material_set_tutorial_gemstones_name': ('教学宝石', 'Tutorial Gemstones')})

manifest('ex4_recipe', 'tutorial_recipe', 'Token Recipe / 代币合成', 'Two tokens combine into a stronger artifact. / 两枚代币合成为更强的藏品。')
write('ex4_recipe', 'script/init.lua', '''
local A = CS.Aotenjo
function init()
    local copper = A.LuaArtifactBuilder.Create("tutorial_recipe:copper_token", A.Rarity.COMMON)
        :OnSelfEffect(function(player, perm, effects, artifact)
            effects:Add(A.ScoreEffect.AddFu(8, artifact))
        end):BuildAndRegister()
    local silver = A.LuaArtifactBuilder.Create("tutorial_recipe:silver_token", A.Rarity.RARE)
        :OnSelfEffect(function(player, perm, effects, artifact)
            effects:Add(A.ScoreEffect.AddFan(2, artifact))
        end):BuildAndRegister()
    local combined = A.LuaArtifactBuilder.Create("tutorial_recipe:double_token", A.Rarity.RARE)
        :OnSelfEffect(function(player, perm, effects, artifact)
            effects:Add(A.ScoreEffect.AddFan(5, artifact))
        end):BuildAndRegisterCraftable()
    -- A real C# List<Artifact>, not a Lua table. 真正的 C# List<Artifact>。
    local inputs = CS.System.Collections.Generic.List(A.Artifact)()
    inputs:Add(copper)
    inputs:Add(silver)
    local recipe = A.LuaArtifactRecipeBuilder.BuildAndRegister("tutorial_recipe:tokens", inputs, combined)
    assert(recipe ~= nil, "tutorial_recipe: recipe registration failed")
    A.Logger.Log("[tutorial_recipe] registered tokens recipe")
end
''')
langs('ex4_recipe', {
 'artifact_tutorial_recipe:copper_token_name': ('铜代币', 'Copper Token'),
 'artifact_tutorial_recipe:copper_token_description': ('计分时加8符。与银代币合成。', 'Add 8 Fu when scoring. Combines with Silver Token.'),
 'artifact_tutorial_recipe:silver_token_name': ('银代币', 'Silver Token'),
 'artifact_tutorial_recipe:silver_token_description': ('计分时加2番。与铜代币合成。', 'Add 2 Fan when scoring. Combines with Copper Token.'),
 'artifact_tutorial_recipe:double_token_name': ('双代币', 'Double Token'),
 'artifact_tutorial_recipe:double_token_description': ('计分时加5番。', 'Add 5 Fan when scoring.')})
for icon in ['copper_token', 'silver_token', 'double_token']:
    copy('ex4_recipe', f'texture/artifact/{icon}.png', 'ex1_artifact/texture/artifact/coin_number_2.png')

manifest('ex5_texture_pack', 'tutorial_texture', 'Texture Pack / 材质包', 'Replace Origami Bear with the tutorial coin icon. / 将折纸熊换成教学硬币图标。')
write('ex5_texture_pack', 'texture-pack.json', json.dumps({'formatVersion': 1, 'replacements': [
    {'target': 'artifact:aotenjo:origami_bear', 'file': 'texture_pack/origami_bear.png', 'filterMode': 'Point', 'pixelsPerUnit': 100}
]}, indent=2))
copy('ex5_texture_pack', 'texture_pack/origami_bear.png', 'ex1_artifact/texture/artifact/coin_number_2.png')

manifest('ex6_face_colors', 'tutorial_colors', 'Face Colors / 牌面配色', 'Change the appearance of existing fonts. / 修改已有字体的外观。')
write('ex6_face_colors', 'script/init.lua', '''
function init()
    local faces = CS.Aotenjo.TileFaceMaterialRegistry
    assert(faces.RegisterHex("plain", "#24CFA6"), "plain color registration failed")
    assert(faces.RegisterRainbow("blue"), "blue rainbow registration failed")
    CS.Aotenjo.Logger.Log("[tutorial_colors] plain=teal, blue=rainbow")
end
''')

for folder in sorted(MODS.iterdir()):
    if (folder / 'modinfo.json').exists():
        copy(folder.name, 'img.png', 'ex1_artifact/texture/artifact/coin_number_2.png')

# Remove obsolete, unreferenced entry modules from the previous tutorial.
for relative in ['ex1_artifact/script/artifact.lua', 'ex1_artifact/script/util.lua']:
    (MODS / relative).unlink(missing_ok=True)

readmes = {
 '00_hello': ('quickstart', '启动后在 AML 日志搜索 `[tutorial_hello] Hello / 你好`。', 'Launch and find `[tutorial_hello] Hello / 你好` in the AML log.', 'script/init.lua'),
 'ex1_artifact': ('artifacts', '测试新局中输入 `give tutorial_artifact:coin_twos`，用 `setHand 222m123p` 打出二应每张得2金币。`give tutorial_artifact:practice_jade` 后两次计分应分别乘1.0、1.1番。', 'In a test run, use `give tutorial_artifact:coin_twos` and `setHand 222m123p`; newly played twos earn 2 coins each. Use `give tutorial_artifact:practice_jade`; the first two scoring triggers multiply Fan by 1.0 and 1.1.', 'script/tutorial_artifact/artifacts.lua'),
 'ex2_pattern': ('yakus', '标准范围套组中新局输入 `upgradeYaku custom_yaku:tutorial_yaku:all_twos 1`，全为二的组合成立，加入非二不成立；火包索引为2。', 'In a standard-range deck run, use `upgradeYaku custom_yaku:tutorial_yaku:all_twos 1`; all-twos combinations qualify, mixed ones fail. Fire pack index is 2.', 'script/init.lua'),
 'ex3_tile_material': ('materials', '新局选教学宝石组，或 `setHand 222m333p` 后 `setMat 0-2 tutorial_gems:ruby`。红宝石计分加20符，经过关末效果时加2金币；蓝宝石计分乘2番。', 'Select Tutorial Gemstones for a new run, or use `setHand 222m333p`, then `setMat 0-2 tutorial_gems:ruby`. Ruby adds 20 Fu and earns 2 coins when its round-end effect runs; Sapphire doubles Fan.', 'script/init.lua'),
 'ex4_recipe': ('recipes', '依次 `give tutorial_recipe:copper_token` 与 `give tutorial_recipe:silver_token`，应消耗输入并获得双代币，计分加5番。无需安装其他示例。', 'Give `tutorial_recipe:copper_token`, then `tutorial_recipe:silver_token`; the inputs are consumed and Double Token grants +5 Fan when scoring. No other example is required.', 'script/init.lua'),
 'ex5_texture_pack': ('textures', '图鉴中的折纸熊应显示为硬币，机制不变。日志出现 Loaded 1 texture pack replacement(s)。此包不需要 Lua。', 'Origami Bear should display the coin icon with unchanged mechanics. Find Loaded 1 texture pack replacement(s) in the log. This pack requires no Lua.', 'texture-pack.json'),
 'ex6_face_colors': ('textures', '普通字体改为青绿，蓝字体改为彩虹；测试指令 `setFont 0 blue`。这是字体外观修改，不添加新字体机制。', 'Plain faces become teal; blue faces become rainbow. Test with `setFont 0 blue`. This changes existing font appearance, without registering new font mechanics.', 'script/init.lua')
}
for folder, (chapter, zh, en, entry) in readmes.items():
    write(folder, 'README.md', f'''
# {folder}

## 安装与验证

退出游戏，把本文件夹直接放到 `Aotenjo_Data/StreamingAssets/mods/`，使下一层就是 `modinfo.json`，完整重启。{zh}

主入口：`{entry}`。修改后完整重启。控制台开启方法、期望结果与存档测试请阅读在线手册；示例默认不启用调试控制台。

[中文教程](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/{chapter}.md) · [测试与排错](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/zh/testing.md)

## Install and verify

Close the game. Place this folder directly in `Aotenjo_Data/StreamingAssets/mods/`, with `modinfo.json` immediately inside it, then restart. {en}

Main entry: `{entry}`. Fully restart after edits. The online handbook explains the test console, expected outcomes, and save testing. Debug console access is disabled by default in these examples.

[English guide](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/{chapter}.md) · [Testing and troubleshooting](https://github.com/XOCatGames/Aotenjo-Src/blob/main/docs/en/testing.md)

## 修改为自己的模组 / Make it yours

修改 modID、Lua内容ID、模块前缀、双语键和图片文件名，保持相互对应。使用稳定ID，不要把版本号写进ID。更改机制后同步更新说明。

Change modID, Lua content IDs, module prefixes, bilingual keys, and sprite filenames together. Keep IDs stable and version-free. Update descriptions whenever mechanics change.

图片复用本仓库既有公开示例；并非新许可授权。Artwork is reused from this repository's original public examples; no new license is implied.
''')

print('Built 7 self-contained tutorial mods.')
