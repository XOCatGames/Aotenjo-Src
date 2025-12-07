using System.Collections.Generic;
using System.Linq;

namespace Aotenjo.FallDecks
{
    public class ExtendedDeck : MahjongDeck
    {
        public ExtendedDeck() : base("extended", "extended", "all", "intermediate", 10)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            return new ExtendedDeckPlayer(seed, set, asensionLevel, this);
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return decks.Count(d => d != this && d.IsUnlocked(stats)) >= 6;
        }
    }

    public class ExtendedDeckPlayer : Player
    {
        public ExtendedDeckPlayer(string seed, MaterialSet set, int asensionLevel, MahjongDeck deck) : 
            base(Hand.PlainFullHand().tiles, seed, deck, PlayerProperties.DEFAULT, asensionLevel, SkillSet.ExtendedSkillSet(), set, asensionLevel)
        {
            properties.HandLimit += 3;
        }

        public override int GetMaxPlayingStage()
        {
            return 5;
        }
        
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            levelTarget *= 3;
        }
        
        // protected override bool CanSettleMoreTile(bool firstHand)
        // {
        //     Permutation perm = GetAccumulatedPermutation();
        //     if (playMode == 0 || perm == null) return true;
        //     return perm.ToTiles().Count <= 14;
        // }
    }
}