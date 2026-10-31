using System.Collections.Generic;

using System.Linq;

namespace Aotenjo
{
    public abstract class CountPairArtifact : Artifact
    {
        protected CountPairArtifact(string name, Rarity rarity) : base(name, rarity)
        {
        }

        protected List<(Block, Block)> GetPairsWithDiff(Permutation permutation, int i, Player player)
        {
            return Utils.FindPairs(permutation,
                (b1, b2) => player.DetermineShiftedPair(b1, b2, i, false));
        }

        protected void QueuePairScoringEffect(Player player, ScoreEffect effect, Block first, Block second)
        {
            // Pair scores use a multi-tile animation but still count as one block-scoring trigger.
            if (!effect.HasTag(EffectTag.BlockScoring))
                effect.tags.Add(EffectTag.BlockScoring);
            player.playHandEffectStack.Push(
                effect.OnMultipleTiles(first.tiles.Union(second.tiles).ToList(), first.tiles[1]));
        }
    }
}