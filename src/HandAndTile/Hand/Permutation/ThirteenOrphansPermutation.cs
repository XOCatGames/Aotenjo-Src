using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class ThirteenOrphansPermutation : Permutation
    {
        public ThirteenOrphansPermutation(string representation) : base(representation)
        {
        }

        public ThirteenOrphansPermutation(Block[] array, Block.Jiang jiang) : base(array, jiang)
        {
            if (array.Length != 1 || array.Any(b => b is not ThirteenOrphansBlock))
            {
                throw new ArgumentException("SevenPairsPermutation must have 6 blocks and a Pair");
            }
        }

        public override PermutationType GetPermType()
        {
            return PermutationType.THIRTEEN_ORPHANS;
        }

        public override bool IsFullHand(Player player)
        {
            return true;
        }
    }
}