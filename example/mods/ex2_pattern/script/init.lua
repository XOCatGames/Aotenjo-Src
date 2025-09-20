function init()
    CS.Aotenjo.Logger.Log("Creating Pattern All Twos")

    CS.Aotenjo.CustomYakuBuilder.RegisterCustomYaku(
        "example_mod_ex2:all_twos", -- RegName
        222,                        -- Base Fan
        2,                          -- Growth Factor
        122,                        -- Scaling Fan (per level)
        function (perm, player)
            local tiles = perm:ToTiles()
            for i = 0, tiles.Count - 1 do
                local tile = tiles[i]
                if not tile:IsNumbered(2) then
                    return false
                end
            end
            return true
        end,
        { CS.Aotenjo.FixedYakuType.DuanYao, CS.Aotenjo.FixedYakuType.QuanXiao, CS.Aotenjo.FixedYakuType.QuanShuangKe }, -- Inherited Yakus
        { "standard", "galaxy" }, -- Yaku Groups
        { 2 },                    -- Pattern Packs that this is in. 1: Wind, 2: Forest, 3: Fire, 4: Mountain
        CS.Aotenjo.Rarity.LEGENDARY, -- Rarity
        "222s222p222m222m22s" -- Example Hand, using standard Riichi notation
    )
end
