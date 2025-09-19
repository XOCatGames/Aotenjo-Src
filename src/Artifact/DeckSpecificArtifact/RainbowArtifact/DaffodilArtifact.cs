using System;

namespace Aotenjo
{
    public class DaffodilArtifact : Artifact
    {
        private const int DISCARD_RETURN = 3;

        public DaffodilArtifact() : base("daffodil", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), DISCARD_RETURN);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostDiscardTileEvent += OnPostDiscardTileEvent;
        }

        private void OnPostDiscardTileEvent(PlayerDiscardTileEvent.Post discardTileEvent)
        {
            if (discardTileEvent.tile is FlowerTile)
                discardTileEvent.player.DiscardLeft += DISCARD_RETURN;
            AudioSystem.PlayAddSwapChanceSound();
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostDiscardTileEvent -= OnPostDiscardTileEvent;
        }
    }
}