using System;

public class GainedMoneyRequirement : UnlockRequirement
{
    private int count;

    public GainedMoneyRequirement(int count)
    {
        this.count = count;
    }

    public override bool IsUnlocked(PlayerStats stats)
    {
        return stats.GetMoneyEarned() >= count;
    }

    public override int GetCurrent(PlayerStats stats)
    {
        return (int)stats.GetMoneyEarned();
    }

    public override int GetMax()
    {
        return count;
    }

    public override string GetDescription(Func<string, string> loc)
    {
        return string.Format(loc("unlock_requirement_gained_money"), count);
    }
}