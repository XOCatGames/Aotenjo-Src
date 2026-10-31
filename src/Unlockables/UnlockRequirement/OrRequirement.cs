using System;
using System.Linq;

public class OrRequirement : UnlockRequirement
{
    private UnlockRequirement[] requirements;
    private readonly bool hideOtherDescription;

    public OrRequirement(params UnlockRequirement[] requirements)
    {
        this.requirements = requirements;
    }

    public OrRequirement(UnlockRequirement first, UnlockRequirement other, bool hideOtherDescription)
        : this(first, other)
    {
        this.hideOtherDescription = hideOtherDescription;
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
        if (hideOtherDescription) return requirements[0].GetDescription(loc);
        return string.Join($" \n{loc("ui_or")}\n ", requirements.Select(r => r.GetDescription(loc)));
    }
}