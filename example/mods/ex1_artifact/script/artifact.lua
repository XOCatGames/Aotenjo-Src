local M = {}
-- "Import" statement, reduce redundent namespace calls
local Aotenjo = CS.Aotenjo

-- Event Handler Map
local kongHandlers = {}

function M.register()
    -- Example Item 1, simple behaivior
    Aotenjo.LuaArtifactBuilder.Create("example_mod_ex1:coin_number_2", Aotenjo.Rarity.RARE)
        :WithName(function(player, loc, artifact) -- Fixed Name
            return "Coin Number 2"
        end)
        :WithDescription(function(player, loc, artifact) -- Fixed Description
            return "This is an example item. Gaining <style=\"money\">2</style> for each 2 played."
        end)
        :OnTileEffect(function(player, perm, tile, effects, artifact) -- Tile Effect Function, called when a tile is scored

            -- Determine whether the tile is numbered 2, and whether it is in the PLAYING TILES
            if (player:Selecting(tile) and tile:IsNumbered(2)) then

                -- Adds EarnMoneyEffect, tell the game that this is related to this artifact so it will play animation on the artifact along with the tile
                effects:Add(Aotenjo.EarnMoneyEffect(2, artifact))
                
            end
        end)
        :BuildAndRegister() -- Register the artifact

    -- Second Item, with data storage
    Aotenjo.LuaArtifactBuilder.Create("example_mod_ex1:kong_jade", Aotenjo.Rarity.EPIC)
        :WithName(function(player, loc, artifact)
            return "Kong Jade"
        end)
        :WithDescription(function(player, loc, artifact) -- Dynamic Description
            
            -- Get Data `level` from the artifact's data storage, defaults to 0
            local level = artifact:GetDataOrDefault("level", 0)
            
            -- Calculates Fan Mult from `level`
            local mult = 1 + level * 0.1 
            
            -- Changes the description as the artifact grows in level
            return "This is an item with custom data. Fan X" .. mult .. " , increase by 0.1 for each kong added." 
        end)
        :OnSubscribeToPlayer(function(player, artifact)
            local artifactId = artifact:GetRegName() -- Retrieve the RegName of the artifact

            -- Event Handler
            local handler = function(eventData) -- Define the behaivior when a tile is added to form a kong

                -- Get the level data first
                local level = artifact:GetDataOrDefault("level", 0)

                -- Increase level by 1 for each Kong added and set back the data to artifact
                artifact:SetData("level", level + 1)

            end

            -- Store the Handler's reference inside kongHandlers(which is a local map defined earlier) so it can be detached when the artifact is removed from player's inventory
            kongHandlers[artifactId] = handler 

            -- Subscribe to player's PreKongTileEvent with the handler we created
            player:PreKongTileEvent("+", handler)
        end)
        :OnUnsubscribeToPlayer(function(player, artifact)
            -- Get the reference of the function we defined in `OnSubscribeToPlayer`
            local artifactId = artifact:GetRegName()
            local handler = kongHandlers[artifactId]
            if handler ~= nil then
                -- Unsubscribe from player
                player:PreKongTileEvent("-", handler)
                kongHandlers[artifactId] = nil
            end
        end)
        :OnSelfEffect(function(player, perm, effects, artifact)

            -- Retrieve `level`
            local level = artifact:GetDataOrDefault("level", 0)
            local mult = 1 + level * 0.1

            -- Fan Mult!
            effects:Add(Aotenjo.ScoreEffect.MulFan(mult, artifact))
        end)
        :BuildAndRegister()
end

return M
