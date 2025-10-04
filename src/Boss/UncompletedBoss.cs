using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using static Aotenjo.Tile;

public class UncompletedBoss : Boss
{
    private List<Category> playedCategories = new List<Category>();

    public UncompletedBoss() : base("Uncomplete")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent += CountTypes;
        player.PostRoundEndEvent += ClearList;
        playedCategories = new List<Category>();
    }

    private void ClearList(PlayerEvent eventData)
    {
        playedCategories.Clear();
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent -= CountTypes;
        player.PostRoundEndEvent -= ClearList;
    }

    private void CountTypes(Permutation perm, Player player, List<IAnimationEffect> lst)
    {
        List<Tile> tiles = perm.blocks.SelectMany(b => b.tiles).ToList();
        foreach (Tile t in tiles)
        {
            if (!playedCategories.Contains(t.GetCategory()))
            {
                playedCategories.Add(t.GetCategory());
                lst.Add(new SuppressTileGroupAnimationEffect(
                    perm.blocks.First(b => b.Any(t2 => t2 == t)).tiles.ToList(), t));
            }
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnSelfEffectArtifact($"{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, effects) =>
            {
                effects.Add(ScoreEffect.AddFu(40 * perm.ToTiles()
                    .Select(p => p.GetCategory())
                    .Distinct()
                    .Count(),
                    baseArtifact));
            });
    }
}