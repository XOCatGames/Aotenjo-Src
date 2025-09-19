using System;

namespace Aotenjo
{
    public class WoodenDragonBallArtifact : Artifact
    {
        public const int DISCARD_RETURN = 3;

        public WoodenDragonBallArtifact() : base("wooden_dragon_ball", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), DISCARD_RETURN);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostDiscardTileEvent += OnDiscardTile;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostDiscardTileEvent -= OnDiscardTile;
        }

        private void OnDiscardTile(PlayerDiscardTileEvent.Post discardTileEvent)
        {
            BambooDeckPlayer player = discardTileEvent.player as BambooDeckPlayer;
            if (player.DetermineDora(discardTileEvent.tile) >= 1)
            {
                player.DiscardLeft += DISCARD_RETURN;
            }
        }
    }
}