using System;
using System.Linq;

public class AndRequirement : UnlockRequirement
{
    private UnlockRequirement[] requirements;

    public AndRequirement(params UnlockRequirement[] requirements)
    {
        this.requirements = requirements;
    }

    public override bool IsUnlocked(PlayerStats stats)
    {
        foreach (UnlockRequirement requirement in requirements)
        {
            if (!requirement.IsUnlocked(stats))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetCurrent(PlayerStats stats)
    {
        return requirements.Max(r => r.GetCurrent(stats));
    }

    public override int GetMax()
    {
        return requirements.Max(r => r.GetMax());
    }

    public override string GetDescription(Func<string, string> loc)
    {
        return string.Join($" \n{loc("ui_and")}\n ", requirements.Select(r => r.GetDescription(loc)));
    }
}