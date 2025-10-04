using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class ChinaRoseArtifact : Artifact
    {
        private const int MONEY_AMOUNT = 2;
        private const int FAN_AMOUNT = 3;
        private const int FU_AMOUNT = 40;

        public ChinaRoseArtifact() : base("china_rose", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MONEY_AMOUNT, FAN_AMOUNT, FU_AMOUNT);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            RainbowDeck.RainbowPlayer rainbowPlayer = (RainbowDeck.RainbowPlayer)player;
            rainbowPlayer.PostPlayFlowerTileEvent += OnPostPlayFlowerTileEvent;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            RainbowDeck.RainbowPlayer rainbowPlayer = (RainbowDeck.RainbowPlayer)player;
            rainbowPlayer.PostPlayFlowerTileEvent -= OnPostPlayFlowerTileEvent;
        }

        private void OnPostPlayFlowerTileEvent(Player player, FlowerTile flowerTile)
        {
            if (!player.IsPlayerWind(flowerTile.GetOrder()))
            {
                return;
            }

            player.EarnMoney(MONEY_AMOUNT);
            EventManager.Instance.OnArtifactEarnMoney(MONEY_AMOUNT, this);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile is FlowerTile flowerTile)
            {
                if (player.IsPlayerWind(flowerTile.GetOrder()))
                {
                    effects.Add(ScoreEffect.AddFan(FAN_AMOUNT, this));
                    effects.Add(ScoreEffect.AddFu(FU_AMOUNT, this));
                }
            }
        }
    }
}