using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class PlayedYakuRequirement : UnlockRequirement
{
    private YakuType type;
    private int count;
    private bool needFull;

    public PlayedYakuRequirement(YakuType type, int count, bool needFull)
    {
        this.type = type;
        this.count = count;
        this.needFull = needFull;
    }

    public override bool IsUnlocked(PlayerStats stats)
    {
        return stats
                   .GetPlayedHands()
                   .Count(r => r.activatedYakuTypes.Contains(type)
                               && (!needFull || (Math.Abs(YakuTester.InfoMap[type].growthFactor - 1) < 0.01f || r.AllTiles.Count == 14))) >=
               count;
    }

    public override string GetDescription(Func<string, string> loc)
    {
        //TODO: 需要解耦
        var yaku = YakuTester.InfoMap[type];
        string yakuName = YakuLocalizationManager.GetYakuName(yaku);
        string formattedName = $"<style=\"{yaku.rarity.ToString().ToLower()}\"><link=\"pattern_{type.ToString()}\">{yakuName}</link></style>";
        return string.Format(loc("unlock_requirement_played_yaku_format"),
            formattedName, count);
    }

    public override int GetCurrent(PlayerStats stats)
    {
        return stats
            .GetPlayedHands().Count(r => r.activatedYakuTypes.Contains(type)
                                         && (!needFull || (Math.Abs(YakuTester.InfoMap[type].growthFactor - 1) < 0.01f || r.AllTiles.Count == 14)));
    }

    public override int GetMax()
    {
        return count;
    }
}