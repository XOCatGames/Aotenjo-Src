using System;

namespace Aotenjo
{
    public class CoinAmuletArtifact : Artifact
    {
        private const int MONEY = 3;

        public CoinAmuletArtifact() : base("coin_amulet", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MONEY);
        }
        
        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Subscribe<OraclePlayer.PlayerSetOracleEvent.Draw>(DrawOracle);
        }

        private void DrawOracle(OraclePlayer.PlayerSetOracleEvent.Draw obj)
        {
            if (obj.block.GetCategory() == Tile.Category.Bing || obj.player.GetMoney() < MONEY) return;
            
            while (!obj.block.IsNumbered())
            {
                obj.block = obj.player.GenerateRandomBlock();
            }

            obj.player.SpendMoney(MONEY);
            foreach (var blockTile in obj.block.tiles)
            {
                blockTile.ModifyCategory(Tile.Category.Bing, obj.player);
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Unsubscribe<OraclePlayer.PlayerSetOracleEvent.Draw>(DrawOracle);
        }
        
    }
}