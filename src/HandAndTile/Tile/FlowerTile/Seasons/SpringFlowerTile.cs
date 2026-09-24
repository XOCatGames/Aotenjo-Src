using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class SpringFlowerTile : FlowerTile
{
    public SpringFlowerTile() : base(Category.SiJi, 1)
    {
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);

        effects.Add(new TextEffect("effect_spring_name").OnTile(this));
        foreach (Tile tile in player.GetSelectedTilesCopy().Where(t => t.IsNumbered()))
        {
            if (player.GenerateRandomInt(4) == 0)
                effects.Add(new GrowEffect(tile, null).OnTile(tile));
        }
    }

    public override void AppendRoundEndEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendRoundEndEffect(effects, player, perm);
    }
}
