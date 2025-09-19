using System;
using System.Collections.Generic;
using Aotenjo;

public class NumberlessBoss : Boss
{
    public NumberlessBoss() : base("numberless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent += PostAddOnTileAnimation;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent -= PostAddOnTileAnimation;
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnTileEffectArtifact($"artifact_{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, tile, effects) =>
            {
                throw new NotImplementedException();
            });
    }
    
    private void PostAddOnTileAnimation(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        List<Tile> tiles = new(player.GetSelectedTilesCopy());
        foreach (var tile in tiles)
        {
            if (tile.GetCategory() == Tile.Category.Wan)
            {
                list.Add(ScoreEffect.AddFan(-2, null).OnTile(tile));
            }
            else if (tile.GetCategory() == Tile.Category.Bing)
            {
                list.Add(new EarnMoneyEffect(-1).OnTile(tile));
            }
            else if (tile.GetCategory() == Tile.Category.Suo)
            {
                list.Add(ScoreEffect.AddFu(-5, null).OnTile(tile));
            }
        }
    }
}