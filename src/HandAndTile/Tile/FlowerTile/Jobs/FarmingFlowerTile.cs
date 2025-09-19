using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class FarmingFlowerTile : OneTimeUseFlowerTile
{
    private const int FU = 10;

    public FarmingFlowerTile() : base(Category.SiYe, 3)
    {
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), FU);
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        if (used) return;
        used = true;

        if (!perm.jiang.All(t => t.IsNumbered())) return;

        foreach (Tile t in player.GetHandDeckCopy()
                     .Where(o => o.IsNumbered() && perm.jiang.All(pairT => pairT.GetOrder() == o.GetOrder())))
        {
            effects.Add(new OnTileAnimationEffect(t, new GrowFuEffect(null, t, FU, "grow_bamboo_segment")));
        }
    }
}