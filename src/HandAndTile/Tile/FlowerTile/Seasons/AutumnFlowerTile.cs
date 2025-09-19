using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class AutumnFlowerTile : FlowerTile
{
    private const float FU_PER_TILE = 3;

    public AutumnFlowerTile() : base(Category.SiJi, 3)
    {
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), FU_PER_TILE);
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        effects.Add(new OnTileAnimationEffect(this, new TextEffect("effect_autumn_name")));
        foreach (Tile tile in perm.ToTiles().OrderBy(t => player.TileSettlingOrder(t, perm)).Where(t => t.IsNumbered()))
            effects.Add(new OnTileAnimationEffect(tile, ScoreEffect.AddFu(FU_PER_TILE, null)));
    }
}