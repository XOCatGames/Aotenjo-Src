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

        effects.Add(new OnTileAnimationEffect(this, new TextEffect("effect_spring_name")));
        foreach (Tile tile in player.GetSelectedTilesCopy().Where(t => t.IsNumbered()))
        {
            if (player.GenerateRandomInt(4) == 0)
                effects.Add(new OnTileAnimationEffect(tile, new GrowEffect(tile, null)));
        }
    }

    public override void AppendRoundEndEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendRoundEndEffect(effects, player, perm);
    }
}