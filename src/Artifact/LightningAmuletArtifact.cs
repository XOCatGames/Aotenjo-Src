using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class LightningAmuletArtifact : Artifact
    {
        public LightningAmuletArtifact() : base("lightning_amulet", Rarity.EPIC)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreSetMaterialEvent += OnSetMaterial;
            player.PreSetFontEvent += OnSetFont;
            player.PreSetMaskEvent += OnSetMask;
            player.PreSetPropertiesEvent += OnSetProperties;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreSetMaterialEvent -= OnSetMaterial;
            player.PreSetFontEvent -= OnSetFont;
            player.PreSetMaskEvent -= OnSetMask;
            player.PreSetPropertiesEvent -= OnSetProperties;
        }

        private void OnSetProperties(PlayerSetPropertiesEvent evt)
        {
            if (evt.isCopy) return;
            Tile tile = evt.tile;

            if (!tile.IsNumbered())
            {
                return;
            }

            Player player = evt.player;
            TileProperties pToChange = evt.propertiesToChange;
            Tile mirroredTile = player.GetAllTiles().FirstOrDefault(t =>
                t.IsNumbered() && t.GetCategory() != tile.GetCategory() && t.GetOrder() == tile.GetOrder());

            mirroredTile?.SetProperties(new(pToChange), player, true);
        }

        private void OnSetMask(PlayerSetAttributeEvent evt)
        {
            if (evt.isCopy) return;
            Tile tile = evt.tile;

            if (!tile.IsNumbered())
            {
                return;
            }

            Player player = evt.player;

            TileMask maskToChange = (TileMask)evt.attributeToReceive;

            Tile mirroredTile = FindMirroredTile(player, tile);

            if (maskToChange == TileMask.NONE)
            {
                List<Tile> tiles = player.GetAllTiles().Where(t =>
                    t != null && t.properties.mask.IsDebuff() && t.IsNumbered() &&
                    t.GetCategory() != tile.GetCategory()).ToList();
                if (tiles.Count != 0)
                {
                    mirroredTile = tiles[player.GenerateRandomInt(tiles.Count)];
                }
            }

            mirroredTile?.SetMask(maskToChange.Copy(), player, true);
        }

        private void OnSetFont(PlayerSetAttributeEvent evt)
        {
            if (evt.isCopy) return;
            Tile tile = evt.tile;

            if (!tile.IsNumbered())
            {
                return;
            }

            Player player = evt.player;

            TileFont fontToChange = (TileFont)evt.attributeToReceive;

            Tile mirroredTile = FindMirroredTile(player, tile);

            mirroredTile?.SetFont(fontToChange.Copy(), player, true);
        }

        private void OnSetMaterial(PlayerSetAttributeEvent evt)
        {
            if (evt.isCopy) return;
            Tile tile = evt.tile;

            if (!tile.IsNumbered())
            {
                return;
            }

            Player player = evt.player;

            TileMaterial matToChange = (TileMaterial)evt.attributeToReceive;

            Tile mirroredTile = FindMirroredTile(player, tile);

            mirroredTile?.SetMaterial(matToChange.Copy(), player, true);
        }

        private static Tile FindMirroredTile(Player player, Tile tile)
        {
            List<Tile> cands = player.GetAllTiles().Where(t =>
                    t != null && t.IsNumbered() && t.GetCategory() != tile.GetCategory() &&
                    t.GetOrder() == tile.GetOrder())
                .ToList();
            return cands.Count == 0 ? null : cands[player.GenerateRandomInt(cands.Count)];
        }
    }
}