using System;
using Aotenjo;

public class LessenTargetPlayerEffect : StarterBoostEffect
{
    private int v;

    public LessenTargetPlayerEffect(int v) : base("starter_lessen_target")
    {
        this.v = v;
    }

    public override string GetLocalizedDesc(Func<string, string> loc)
    {
        return string.Format(base.GetLocalizedDesc(loc), v);
    }

    public override void Boost(Player player)
    {
        player.IncreaseTargetMultiplier(-0.01D * v);
        player.levelTarget *= 1D + (-0.01D * v);
    }
}