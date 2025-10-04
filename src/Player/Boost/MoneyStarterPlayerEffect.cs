using System;
using Aotenjo;

public class MoneyStarterPlayerEffect : StarterBoostEffect
{
    public int Amount { get; }

    public MoneyStarterPlayerEffect(int amount = 15, string name = "starter_money") : base(name)
    {
        Amount = amount;
    }


    public override string GetLocalizedDesc(Func<string, string> loc)
    {
        return string.Format(loc("player_effect_starter_money_desc"), Amount);
    }

    public override void Boost(Player player)
    {
        player.EarnMoney(Amount);
        MessageManager.Instance.OnSoundEvent("earn_coins");
    }
}