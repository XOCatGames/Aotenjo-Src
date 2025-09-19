using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class WinterFlowerTile : FlowerTile
{
    private const float FAN_PER_TILE = 1;

    public WinterFlowerTile() : base(Category.SiJi, 4)
    {
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), FAN_PER_TILE);
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);

        effects.Add(new TextEffect("effect_winter_name").OnTile(this));
        foreach (Tile tile in perm.ToTiles().OrderBy(t => player.TileSettlingOrder(t, perm))
                     .Where(t => t.IsYaoJiu(player)))
            effects.Add(ScoreEffect.AddFan(FAN_PER_TILE, null).OnTile(tile));
    }
}