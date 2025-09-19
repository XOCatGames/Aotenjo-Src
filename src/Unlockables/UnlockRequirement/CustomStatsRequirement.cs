using System;

public class CustomStatsRequirement : UnlockRequirement
{
    private string key;
    private int count;

    public CustomStatsRequirement(string key, int count)
    {
        this.key = key;
        this.count = count;
    }

    public override bool IsUnlocked(PlayerStats stats)
    {
        return stats.GetCustomStats(key) >= count;
    }

    public override int GetCurrent(PlayerStats stats)
    {
        return stats.GetCustomStats(key);
    }

    public override int GetMax()
    {
        return count;
    }

    public override string GetDescription(Func<string, string> loc)
    {
        return string.Format(loc($"unlock_requirement_custom_{key}_format"), count);
    }
}