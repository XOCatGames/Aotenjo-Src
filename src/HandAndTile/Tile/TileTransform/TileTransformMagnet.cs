using System;
using Aotenjo;

[Serializable]
public class TileTransformMagnet : TileTransform
{
    public TileTransformMagnet() : base("magnet")
    {
    }

    public override int GetTransformedOrder(Tile t)
    {
        return (t.GetOrder() + 1) % 4 + 1;
    }

    public override int GetDisplayID(Tile tile, Player player, PlayerStats stats)
    {
        int res = 54 + GetTransformedOrder(tile) - 1;
        if (player.GetArtifacts().Contains(Artifacts.ThreeDGlasses) && Artifacts.ThreeDGlasses.IsActive(player))
        {
            res += 4;
        }

        return res;
    }

    public override bool ChangeBaseDisplay()
    {
        return true;
    }
}