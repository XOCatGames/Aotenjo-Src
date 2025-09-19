namespace Aotenjo
{
    public class UnknownAmuletArtifact : Artifact
    {
        public UnknownAmuletArtifact() : base("unknown_amulet", Rarity.EPIC)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Subscribe<OraclePlayer.PlayerSetOracleEvent.Draw>(DrawOracle);
        }

        private void DrawOracle(OraclePlayer.PlayerSetOracleEvent.Draw obj)
        {
            int order = obj.player.GenerateRandomInt(7) + 1;
            obj.block = new Block($"{order}{order}{order}z");
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Unsubscribe<OraclePlayer.PlayerSetOracleEvent.Draw>(DrawOracle);
        }
    }
}