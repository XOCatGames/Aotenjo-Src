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
