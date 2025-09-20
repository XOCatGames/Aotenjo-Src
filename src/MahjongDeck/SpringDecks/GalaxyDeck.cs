using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class GalaxyDeck : MahjongDeck
    {
        public GalaxyDeck() : base("galaxy", "galaxy", "galaxy", "standard", 2)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int ascensionLevel)
        {
            Player player = new GalaxyDeckPlayer(seed, this, set, ascensionLevel);
            return player;
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return Constants.DEBUG_MODE ||
                   YakuTester.InfoMap.Keys.Where(yt => stats.UnlockedYakuDetail(yt)).Count() >= 50;
        }

        private class GalaxyDeckPlayer : Player
        {
            public GalaxyDeckPlayer(string seed, MahjongDeck deck, MaterialSet materialSet, int ascensionLevel) : base(
                Hand.PlainFullHand().tiles, seed,
                deck, PlayerProperties.DEFAULT, 0, SkillSet.StandardSkillSet(), materialSet, ascensionLevel)
            {
            }

            public override BlockCombinator GetCombinator()
            {
                return BlockCombinator.Apollo;
            }
        }
    }
}