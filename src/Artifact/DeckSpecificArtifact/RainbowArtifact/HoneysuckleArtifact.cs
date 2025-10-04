using System;

namespace Aotenjo
{
    public class HoneysuckleArtifact : Artifact
    {
        private const int AMOUNT = 1;

        public HoneysuckleArtifact() : base("honeysuckle", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), AMOUNT);
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
            player.EarnMoney(AMOUNT);
            MessageManager.Instance.OnArtifactEarnMoney(AMOUNT, this);
        }
    }
}