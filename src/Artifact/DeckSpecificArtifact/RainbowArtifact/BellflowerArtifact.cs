namespace Aotenjo
{
    public class BellflowerArtifact : Artifact
    {
        public BellflowerArtifact() : base("bellflower", Rarity.COMMON)
        {
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
            Tile wind = new Tile($"{flowerTile.GetOrder()}z");
            wind.properties.mask = TileMask.Fractured();
            player.AddNewTileToPool(wind);
        }
    }
}