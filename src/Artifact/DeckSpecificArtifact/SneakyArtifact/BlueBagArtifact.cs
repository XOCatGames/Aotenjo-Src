using System.Collections.Generic;

namespace Aotenjo
{
    public class BlueBagArtifact : SneakyArtifact
    {
        private const float MUL = 1.2f;

        public BlueBagArtifact() : base("blue_bag", Rarity.EPIC)
        {
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            if (player is SneakyPlayer sneakyPlayer)
            {
                return (sneakyPlayer.SneakedLastRound(tile));
            }

            return false;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (player is SneakyPlayer sneakyPlayer)
            {
                if (sneakyPlayer.SneakedLastRound(tile))
                {
                    effects.Add(ScoreEffect.MulFan(MUL, this));
                }
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if (player is SneakyPlayer sneakyPlayer)
            {
                sneakyPlayer.PostSneakTileEvent += PostSneakTile;
            }
        }

        private void PostSneakTile(PlayerTileEvent tileEvent)
        {
            Tile tile = tileEvent.tile;
            tile.SetMask(TileMask.Corrupted(), tileEvent.player);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if (player is SneakyPlayer sneakyPlayer)
            {
                sneakyPlayer.PostSneakTileEvent -= PostSneakTile;
            }
        }
    }
}