namespace Aotenjo
{
    public abstract class CountPairArtifact : Artifact
    {
        protected CountPairArtifact(string name, Rarity rarity) : base(name, rarity)
        {
        }

        protected int CountPairsWithDiff(Permutation permutation, int diff, Player player)
        {
            bool pred(Block b1, Block b2) => player.DetermineShiftedPair(b1, b2, diff, false);
            return Utils.CountPairs(permutation, pred);
        }
    }
}