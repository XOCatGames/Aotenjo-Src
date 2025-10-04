using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class BlueDeck : MahjongDeck
    {
        public BlueDeck() : base("blue_deck", "standard", "standard", "standard", 1)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            Player player = new BlueDeckPlayer(seed, this, set, asensionLevel);
            return player;
        }

        public override MaterialSet[] GetAvailableMaterialSets()
        {
            return new[] { MaterialSet.Ore, MaterialSet.Porcelain, MaterialSet.Monsters, MaterialSet.Wood, MaterialSet.Dessert };
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return true;
        }

        public class BlueDeckPlayer : Player
        {
            public event Action<PlayerYakuEvent.Reroll> RerollYakuEvent;

            public BlueDeckPlayer(string seed, MahjongDeck deck, MaterialSet materialSet, int ascensionLevel)
                : base(Hand.PlainFullHand().tiles, seed, deck, materialSet, ascensionLevel)
            {
            }

            public YakuType OnRerollYaku(YakuPack pack, Player player, YakuType exceptedYaku)
            {
                List<YakuType> availableYakus = player.deck.GetAvailableYakus().Select(y => y.yakuTypeID).ToList();
                availableYakus.Remove(exceptedYaku);
                DrawYakuResult drawResult = pack.Draw(player.GenerateRandomInt, availableYakus, player.Level / 4);

                PlayerYakuEvent.Reroll eventData =
                    new PlayerYakuEvent.Reroll(this, pack, exceptedYaku, drawResult.yaku);
                RerollYakuEvent?.Invoke(eventData);

                return eventData.target;
            }
        }
    }
}