function init()
    local faces = CS.Aotenjo.TileFaceMaterialRegistry
    assert(faces.RegisterHex("plain", "#24CFA6"), "plain color registration failed")
    assert(faces.RegisterRainbow("blue"), "blue rainbow registration failed")
    CS.Aotenjo.Logger.Log("[tutorial_colors] plain=teal, blue=rainbow")
end
