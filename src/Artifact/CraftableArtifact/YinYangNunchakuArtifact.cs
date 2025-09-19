namespace Aotenjo
{
    public class YinYangNunchakuArtifact : CraftableArtifact
    {
        public YinYangNunchakuArtifact() : base("yin_yang_nunchaku", Rarity.EPIC)
        {
            SetHighlightRequirement((t, _) => t.GetOrder() % 2 == 1 && t.IsNumbered());
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.DetermineYaojiuTileEvent += OnDetermineYaojiu;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            player.DetermineYaojiuTileEvent -= OnDetermineYaojiu;
        }

        private void OnDetermineYaojiu(PlayerTileEvent evt)
        {
            Tile t = evt.tile;
            if (t.GetOrder() % 2 == 1 && t.IsNumbered())
                evt.canceled = false;
        }
    }
}