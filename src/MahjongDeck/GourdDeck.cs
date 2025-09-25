using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class GourdDeck : MahjongDeck
    {
        public GourdDeck() : base("gourd", "standard", "all", "intermediate", 8)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            Player player = new GourdPlayer(Hand.PlainFullHand().tiles, seed, this, set, asensionLevel);
            player.ObtainArtifact(Artifacts.PurpleGourd);
            return player;
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return Constants.DEBUG_MODE ||
                   stats.GetUnseededRunRecords().Any(rec => rec.acsensionLevel >= 3 && rec.won);
        }

        private class GourdPlayer : Player
        {
            public GourdPlayer(List<Tile> tilePool, string randomSeed, MahjongDeck deck, MaterialSet set, int ascensionLevel = 0) : base(tilePool, randomSeed, deck, set, ascensionLevel)
            {
            }

            public override void OnRoundStart()
            {
                base.OnRoundStart();
                if (Level % 4 == 0)
                {
                    levelTarget *= 2;
                }
            }
        }
    }
}