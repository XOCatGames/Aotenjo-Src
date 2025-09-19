using System;
using System.IO;

namespace Aotenjo
{
    [Serializable]
    public class PlaceHolderDeck : MahjongDeck
    {
        public PlaceHolderDeck(int numID, string name, string range, string augments, string difficulty) :
            base(name, range, augments, difficulty, numID)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            throw new InvalidDataException();
        }

        public override MaterialSet[] GetAvailableMaterialSets()
        {
            return new[] { MaterialSet.Basic };
        }

        public override bool IsUnlocked(PlayerStats _)
        {
            return false;
        }
    }
}