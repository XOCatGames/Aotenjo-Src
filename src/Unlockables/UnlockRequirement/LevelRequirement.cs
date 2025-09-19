using System;
using System.Linq;
using Aotenjo;

public class LevelRequirement : UnlockRequirement
{
    private readonly int level;
    private readonly MahjongDeck deck;
    private readonly int ascensionLevel;
    private readonly MaterialSet materialSet;

    public LevelRequirement(int level, MahjongDeck deck, int ascensionLevel, MaterialSet materialSet)
    {
        this.level = level;
        this.deck = deck;
        this.ascensionLevel = ascensionLevel;
        this.materialSet = materialSet;
    }

    public override bool IsUnlocked(PlayerStats stats)
    {
        return stats.GetUnseededRunRecords().Any(record =>
            (deck == null || record.deckName == deck.regName)
            && record.acsensionLevel >= ascensionLevel
            && (record.roundStats.maxLevel > level || (level == 16 && record.won))
            && (materialSet == null || record.materialSet == materialSet.regName));
    }

    public override string GetDescription(Func<string, string> loc)
    {
        if (ascensionLevel == 0)
            return string.Format(loc("unlock_requirement_level"), Player.GetLevelTitle(loc, level),
                loc($"deck_{(deck == null ? "any" : deck.regName)}_name"));
        return string.Format(loc("unlock_requirement_level_with_ascension"), loc($"ascension_{ascensionLevel}"),
            Player.GetLevelTitle(loc, level), loc($"deck_{(deck == null ? "any" : deck.regName)}_name"));
    }
}