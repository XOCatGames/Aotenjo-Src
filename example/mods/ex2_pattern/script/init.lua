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
        { CS.Aotenjo.FixedYakuType.DuanYao, CS.Aotenjo.FixedYakuType.QuanXiao, CS.Aotenjo.FixedYakuType.QuanShuangKe }, -- 继承的其他番种
        { "standard", "galaxy" }, -- 所属分组
        { 2 },                    -- 所属类别
        CS.Aotenjo.Rarity.LEGENDARY,
        "222s222p222m222m22s"
    )
end
