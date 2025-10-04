namespace Aotenjo
{
    public class ForbiddenAmuletArtifact : Artifact
    {
        public ForbiddenAmuletArtifact() : base("forbidden_amulet", Rarity.RARE)
        {
        }
        
        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Subscribe<OraclePlayer.PlayerSetOracleEvent.Post>(PostSetOracle);
        }

        private void PostSetOracle(OraclePlayer.PlayerSetOracleEvent.Post obj)
        {
            if (obj.player is OraclePlayer oraclePlayer)
            {
                //神谕面子转化为随机腐化牌体牌
                foreach (var tile in oraclePlayer.oracleBlock.tiles)
                {
                    TileProperties properties = oraclePlayer.GenerateRandomTileProperties(0, 90, 10, 0);
                    properties.mask = TileMask.Corrupted();
                    tile.properties = properties;
                }
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if (player is OraclePlayer)
                EventBus.Unsubscribe<OraclePlayer.PlayerSetOracleEvent.Post>(PostSetOracle);
        }
    }
}