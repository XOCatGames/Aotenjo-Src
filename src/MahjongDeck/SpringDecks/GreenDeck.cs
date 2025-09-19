using System;

namespace Aotenjo
{
    [Serializable]
    public class GreenDeck : MahjongDeck
    {
        public GreenDeck() : base("green_deck", "shortened", "basic", "beginner", 0)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int ascensionLevel)
        {
            Player player = new GreenDeckPlayer(seed, this, set, ascensionLevel);
            return player;
        }

        public override MaterialSet[] GetAvailableMaterialSets()
        {
            return new[] { MaterialSet.Basic };
        }

        public override bool IsUnlocked(PlayerStats _)
        {
            return true;
        }

        private class GreenDeckPlayer : Player
        {
            public GreenDeckPlayer(string seed, MahjongDeck deck, MaterialSet materialSet, int ascensionLevel) : base(
                Hand.PlainFullHand().tiles, seed,
                deck, PlayerProperties.DEFAULT, 0, SkillSet.ShortenedSkillSet(), materialSet, ascensionLevel)
            {
            }

            public override void OnRoundStart()
            {
                base.OnRoundStart();
                DiscardLeft += 5;
            }
        }
    }
}