using System;
using Aotenjo;

public class PlayedTileRequirement : UnlockRequirement
{
    private TileAttribute att;
    private int count;

    public PlayedTileRequirement(TileAttribute att, int count)
    {
        this.att = att;
        this.count = count;
    }

    public override bool IsUnlocked(PlayerStats stats)
    {
        return GetCurrent(stats) >= GetMax();
    }

    public override int GetCurrent(PlayerStats stats)
    {
        if (att is TileMaterial mat)
        {
            return stats.GetMaterialPlayedCount(mat);
        }

        if (att is TileFont font)
        {
            return stats.GetFontPlayedCount(font);
        }

        if (att is TileMask mask)
        {
            return stats.GetMaskPlayedCount(mask);
        }

        return 0;
    }

    public override int GetMax()
    {
        return count;
    }

    public override string GetDescription(Func<string, string> loc)
    {
        return string.Format(loc("unlock_requirement_played_tile_format"), loc(att.GetLocalizeKey()), count);
    }
}