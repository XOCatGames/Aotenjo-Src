using System;
using Aotenjo;

public class LoseMoeneyPlayerEffect : StarterBoostEffect
{
    private int v;

    public LoseMoeneyPlayerEffect(int v) : base("starter_lose_money")
    {
        this.v = v;
    }

    public override string GetLocalizedDesc(Func<string, string> loc)
    {
        return string.Format(base.GetLocalizedDesc(loc), v);
    }

    public override void Boost(Player player)
    {
        player.SpendMoney(v);
    }
}