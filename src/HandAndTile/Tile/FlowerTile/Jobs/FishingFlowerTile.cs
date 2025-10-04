using System;
using Aotenjo;

[Serializable]
public class FishingFlowerTile : FlowerTile
{
    private const int MONEY = 3;

    public FishingFlowerTile() : base(Category.SiYe, 1)
    {
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), MONEY);
    }

    public override void OnPlayed(Player player, Permutation perm)
    {
        base.OnPlayed(player, perm);
        player.EarnMoney(MONEY);
        EventManager.Instance.OnSoundEvent("earn_coins");
    }
}