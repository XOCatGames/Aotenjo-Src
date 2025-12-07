using System;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class SevenPairsPermutation : Permutation
    {
        public SevenPairsPermutation(string representation) : base(representation)
        {
        }

        public SevenPairsPermutation(Block[] array, Block.Jiang jiang) : base(array, jiang)
        {
            if (array.Length != 6 || array.Any(b => b is not PairBlock))
            {
                throw new ArgumentException("SevenPairsPermutation must have 6 blocks and a Pair");
            }

            Debug.Log("SEVEN PAIRS FORMED");
        }

        public override PermutationType GetPermType()
        {
            return PermutationType.SEVEN_PAIRS;
        }

        public override bool IsFullHand(Player player)
        {
            return true;
        }
    }
}