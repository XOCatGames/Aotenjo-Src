using System;
using System.Linq;
using Aotenjo;

public class UnlockRequirement
{
    public virtual string GetDescription(Func<string, string> loc)
    {
        return loc("unlock_requirement_default");
    }

    public virtual bool IsUnlocked(PlayerStats stats)
    {
        return true;
    }

    public virtual int GetMax()
    {
        return 1;
    }

    public virtual int GetCurrent(PlayerStats stats)
    {
        return IsUnlocked(stats) ? 1 : 0;
    }

    public virtual UnlockRequirement And(UnlockRequirement other)
    {
        return new AndRequirement(this, other);
    }

    public virtual UnlockRequirement Or(UnlockRequirement other)
    {
        return new OrRequirement(this, other);
    }

    public static UnlockRequirement UnlockedByDefault()
    {
        return new UnlockRequirement();
    }

    public static UnlockRequirement UnlockByLevel(int acsensionLevel, int level, MahjongDeck deck = null,
        MaterialSet materialSet = null)
    {
        return new LevelRequirement(level, deck, acsensionLevel, materialSet);
    }

    public static UnlockRequirement UnlockByPlayYaku(YakuType type, int count = 1, bool needFull = false)
    {
        return new PlayedYakuRequirement(type, count, needFull);
    }

    public static UnlockRequirement UnlockByPlayTileAttribute(TileAttribute att, int count)
    {
        return new PlayedTileRequirement(att, count);
    }

    public static UnlockRequirement UnlockByMoneyEarned(int count)
    {
        return new GainedMoneyRequirement(count);
    }

    public static UnlockRequirement UnlockByCustomStats(string key, int count)
    {
        return new CustomStatsRequirement(key, count);
    }

    public static NotDemoRequirement NotDemoRequirement = new NotDemoRequirement();

    public static UnlockRequirement UnlockByNotDemo()
    {
        return NotDemoRequirement;
    }

    public static UnlockRequirement UnlockedByUnlockSets(int count)
    {
        return new UnlockByUnlockSetsRequirement(count);
    }
}

public class UnlockByUnlockSetsRequirement : UnlockRequirement
{
    private readonly int count;
    public UnlockByUnlockSetsRequirement(int count)
    {
        this.count = count;
    }

    public override int GetCurrent(PlayerStats stats)
    {
        return MaterialSet.MaterialSets.Count(s => s.IsUnlocked(stats));
    }

    public override int GetMax()
    {
        return count;
    }

    public override string GetDescription(Func<string, string> loc)
    {
        return string.Format(loc("unlock_requirement_unlock_material_set"), count);
    }
}