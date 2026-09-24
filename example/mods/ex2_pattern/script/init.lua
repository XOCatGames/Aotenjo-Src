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
