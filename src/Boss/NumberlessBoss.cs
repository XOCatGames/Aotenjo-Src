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
        EventBus.Subscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, PostAddOnTileAnimation);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, PostAddOnTileAnimation);
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new NumberlessReversedArtifact(name);
    }

    /// <summary>
    /// 旧版未实现的反转能力仍显式抛出异常，保留兼容身份；类拆分不擅自新增玩法。
    /// </summary>
    private sealed class NumberlessReversedArtifact : Artifact
    {
        public NumberlessReversedArtifact(string bossName) : base($"artifact_{bossName}_reversed", Rarity.COMMON)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            throw new NotImplementedException();
        }
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