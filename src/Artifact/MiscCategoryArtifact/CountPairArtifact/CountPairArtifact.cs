using System.Collections.Generic;

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
    }
}