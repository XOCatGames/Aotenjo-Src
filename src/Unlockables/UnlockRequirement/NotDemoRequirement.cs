using System;

public class NotDemoRequirement : UnlockRequirement
{
    public override bool IsUnlocked(PlayerStats stats)
    {
        return !Constants.IS_DEMO;
    }

    public override int GetCurrent(PlayerStats stats)
    {
        return 0;
    }

    public override int GetMax()
    {
        return 1;
    }

    public override string GetDescription(Func<string, string> loc)
    {
        return loc("unlock_requirement_custom_unavailable_format");
    }
}