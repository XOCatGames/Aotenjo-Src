using System;
using System.Linq;

public class OrRequirement : UnlockRequirement
{
    private UnlockRequirement[] requirements;

    public OrRequirement(params UnlockRequirement[] requirements)
    {
        this.requirements = requirements;
    }

    public override bool IsUnlocked(PlayerStats stats)
    {
        foreach (UnlockRequirement requirement in requirements)
        {
            if (requirement.IsUnlocked(stats))
            {
                return true;
            }
        }

        return false;
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
        return string.Join($" \n{loc("ui_or")}\n ", requirements.Select(r => r.GetDescription(loc)));
    }
}