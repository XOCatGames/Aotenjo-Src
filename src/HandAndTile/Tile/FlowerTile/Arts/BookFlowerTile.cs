using System;
using Aotenjo;

[Serializable]
public class BookFlowerTile : OneTimeUseFlowerTile
{
    public BookFlowerTile() : base(Category.SiYi, 3)
    {
    }

    public override void OnPlayed(Player player, Permutation perm)
    {
        base.OnPlayed(player, perm);
        EventManager.Instance.OnOpenYakuPack(player.GenerateRandomInt(4));
    }
}