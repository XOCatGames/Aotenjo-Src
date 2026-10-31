using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class YingYangDeck : MahjongDeck
    {
        public YingYangDeck() : base("ying_yang", "ying_yang", "ore", "standard", 7)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            return new YingYangPlayer(Hand.PlainFullHand().tiles, seed, this, set, asensionLevel);
        }


        public override bool IsUnlocked(PlayerStats stats)
        {
            // 不借已退出抽选；旧击败记录继续保留解锁资格，新玩家通过和级以上胜利解锁。
            return stats.GetUnseededRunRecords().Any(rec => rec.acsensionLevel >= 5 && rec.won) || Constants.DEBUG_MODE
                || stats.GetCustomStats($"encounter_boss_{Bosses.Unaided.name}") > 0;
        }
    }

    [Serializable]
    public class YingYangPlayer : Player
    {
        public YingYangPlayer(List<Tile> tilePool, string randomSeed, MahjongDeck deck, MaterialSet set,
            int ascensionLevel) : base(tilePool, randomSeed, deck, set, ascensionLevel)
        {
            properties.GadgetLimit--;
        }

        public override BlockCombinator GetCombinator()
        {
            return BlockCombinator.YingYang;
        }
    }
}
