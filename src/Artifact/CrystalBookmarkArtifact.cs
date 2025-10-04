using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CrystalBookmarkArtifact : Artifact
    {
        public CrystalBookmarkArtifact() : base("crystal_bookmark", Rarity.RARE)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if (player is BlueDeck.BlueDeckPlayer blueDeckPlayer)
            {
                blueDeckPlayer.RerollYakuEvent += OnRerollPattern;
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if (player is BlueDeck.BlueDeckPlayer blueDeckPlayer)
            {
                blueDeckPlayer.RerollYakuEvent -= OnRerollPattern;
            }
        }

        private void OnRerollPattern(PlayerYakuEvent.Reroll data)
        {
            Player player = data.player;
            YakuType original = data.yakuType;

            List<YakuType> availableYakus = player.deck.GetAvailableYakus().Select(y => y.yakuTypeID).ToList();
            availableYakus.Remove(original);
            DrawYakuResult drawResult = data.pack.Draw(player.GenerateRandomInt, availableYakus, player.Level / 4,
                (int)YakuTester.InfoMap[original].rarity + 1);


            data.target = drawResult.yaku;
        }
    }
}