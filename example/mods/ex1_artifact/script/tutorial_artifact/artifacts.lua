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
