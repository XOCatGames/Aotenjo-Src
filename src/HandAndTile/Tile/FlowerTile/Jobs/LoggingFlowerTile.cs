using System.Collections.Generic;
using Aotenjo;

public class LoggingFlowerTile : OneTimeUseFlowerTile
{
    public LoggingFlowerTile() : base(Category.SiYe, 2)
    {
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        if (used) return;
        used = true;
        foreach (Tile t in player.GetSelectedTilesCopy())
        {
            if (t.ContainsGreen(player))
            {
                effects.Add(new OnTileAnimationEffect(t, new FractureEffect(null, t)));
            }
        }
    }
}