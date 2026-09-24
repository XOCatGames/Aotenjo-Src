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

            List<YakuType> availableYakus = player.deck.GetAvailableYakus().Select(y => y.GetYakuType()).ToList();
            availableYakus.RemoveAll(yaku => yaku == original || yaku == FixedYakuType.Base);
            int rarity = (int)YakuTester.InfoMap[original].rarity;
            // Prefer an upgrade; if none is available, reroll within the original rarity.
            bool drawn = data.pack.TryDraw(player.GenerateRandomInt, availableYakus, player.Level / 4,
                out DrawYakuResult drawResult, rarity + 1)
                || data.pack.TryDraw(player.GenerateRandomInt, availableYakus, player.Level / 4,
                    out drawResult, rarity);
            data.target = drawn ? drawResult.yaku : original;
        }
    }
}
