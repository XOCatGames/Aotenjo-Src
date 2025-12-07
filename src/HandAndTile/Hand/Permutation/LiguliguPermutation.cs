using System;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class LiguliguPermutation : Permutation
    {
        public LiguliguPermutation(string representation) : base(representation)
        {
        }

        public LiguliguPermutation(Block[] array, Block.Jiang jiang) : base(array, jiang)
        {
            if (array.Length != 7 || array.Count(b => b is not PairBlock) != 1)
            {
                throw new ArgumentException("LiguliguPermutation must have 6 pair blocks, 1 regular block and a Pair");
            }

            Debug.Log("LIGULIGU FORMED");
        }

        public override PermutationType GetPermType()
        {
            return PermutationType.LIGULIGU;
        }

        public override bool IsFullHand(Player player)
        {
            return true;
        }
    }
}