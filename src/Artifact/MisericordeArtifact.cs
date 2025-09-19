namespace Aotenjo
{
    public class MisericordeArtifact : Artifact
    {
        public MisericordeArtifact() : base("misericorde", Rarity.RARE)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreSetMaskEvent += PreSetMask;
            player.PreSetPropertiesEvent += PreSetProperties;
            player.PostAddTileEvent += AddNewTile;
        }

        private void PreSetProperties(PlayerSetPropertiesEvent evt)
        {
            if (evt.canceled) return;
            if (evt.propertiesToChange.mask.IsDebuff() &&
                evt.propertiesToChange.font.GetRegName() != TileFont.COLORLESS.GetRegName())
            {
                evt.propertiesToChange.font = TileFont.COLORLESS;
                evt.propertiesToChange.mask = TileMask.NONE;
            }
        }

        private void PreSetMask(PlayerSetAttributeEvent evt)
        {
            if (evt.canceled) return;
            if (evt.attributeToReceive is TileMask && evt.attributeToReceive.IsDebuff() &&
                evt.tile.properties.font.GetRegName() != TileFont.COLORLESS.GetRegName())
            {
                evt.canceled = true;
                evt.tile.SetFont(TileFont.COLORLESS, evt.player);
            }
        }

        private void AddNewTile(PlayerTileEvent evt)
        {
            if (evt.tile.properties.IsDebuffed() &&
                evt.tile.properties.font.GetRegName() != TileFont.COLORLESS.GetRegName())
            {
                evt.tile.SetFont(TileFont.COLORLESS, evt.player);
                evt.tile.SetMask(TileMask.NONE, evt.player);
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreSetMaskEvent -= PreSetMask;
            player.PreSetPropertiesEvent -= PreSetProperties;
            player.PostAddTileEvent -= AddNewTile;
        }


        //private void OnRemoveTileFromDiscarded(PlayerTileEvent tileEvent)
        //{
        //    Player player = tileEvent.player;
        //    Tile tile = tileEvent.tile;

        //    if(tile is FlowerTile) return;

        //    if (tileEvent.message != "fractured") return;
        //    if (player.DetermineFontCompactbility(tile, TileFont.COLORLESS)) return;

        //    tileEvent.canceled = true;
        //    tile.SetFont(TileFont.COLORLESS, player);
        //    tile.SetMask(TileMask.NONE, player);
        //}
    }
}