using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class ExtendedThirteenOrphansPermutation : Permutation
    {
        public ExtendedThirteenOrphansPermutation(string representation) : base(representation)
        {
        }

        public ExtendedThirteenOrphansPermutation(Block[] array, Block.Jiang jiang) : base(array, jiang)
        {
            if (array.Length != 2 || array.Count(b => b is not ThirteenOrphansBlock) != 1)
            {
                throw new ArgumentException("ExtendedThirteenOrphansPermutation must have 2 blocks and a Pair");
            }
        }

        public override PermutationType GetPermType()
        {
            return PermutationType.EXTENDED_THIRTEEN_ORPHANS;
        }

        public override bool IsFullHand(Player player)
        {
            return true;
        }
    }
}