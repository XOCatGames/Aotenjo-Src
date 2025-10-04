using System.Linq;

namespace Aotenjo
{
    public class WitherAmuletArtifact : Artifact
    {
        public WitherAmuletArtifact() : base("wither_amulet", Rarity.COMMON)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Subscribe<OraclePlayer.PlayerSetOracleEvent.Pre>(PreSetOracle);
        }
        
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Unsubscribe<OraclePlayer.PlayerSetOracleEvent.Pre>(PreSetOracle);
        }

        private void PreSetOracle(OraclePlayer.PlayerSetOracleEvent.Pre evt)
        {
            if (evt.block.IsNumbered())
            {
                int diff = evt.block.tiles.Min(t => t.GetOrder() - 1);
                foreach (var tile in evt.block.tiles)
                {
                    tile.ModifyOrder(tile.GetOrder() - diff, evt.player);
                }
            }
        }
    }
}