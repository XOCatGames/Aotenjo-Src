-- Prefix module names: all mods share package.loaded. 模块名必须带模组前缀。
local artifacts = require("tutorial_artifact.artifacts")
function init()
    artifacts.register()
    CS.Aotenjo.Logger.Log("[tutorial_artifact] registered 2 artifacts")
end
