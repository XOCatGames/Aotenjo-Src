using System.Linq;

namespace Aotenjo
{
    public class DrawstringArtifact : SneakyArtifact
    {
        public DrawstringArtifact() : base("drawstring", Rarity.RARE)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if (player is SneakyPlayer sneakyPlayer)
            {
                sneakyPlayer.DetermineSneakabilityEvent += DetermineSneakability;
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if (player is SneakyPlayer sneakyPlayer)
            {
                sneakyPlayer.DetermineSneakabilityEvent -= DetermineSneakability;
            }
        }

        private void DetermineSneakability(PlayerTileEvent tileEvent)
        {
            Tile tile = tileEvent.tile;
            if (((SneakyPlayer)tileEvent.player).SneakedLastRound(tile))
            {
                tileEvent.canceled = true;
            }
        }

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            if (player is SneakyPlayer && player.GetGadgets().Any(g => g is CloudGlovesGadget))
            {
                ((CloudGlovesGadget)player.GetGadgets().First(g => g is CloudGlovesGadget)).maxUseCount += 2;
            }
        }

        public override void OnRemoved(Player player)
        {
            base.OnRemoved(player);
            if (player is SneakyPlayer && player.GetGadgets().Any(g => g is CloudGlovesGadget))
            {
                ((CloudGlovesGadget)player.GetGadgets().First(g => g is CloudGlovesGadget)).maxUseCount -= 2;
            }
        }
    }
}