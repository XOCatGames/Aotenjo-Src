using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class MaliciousSprayArtifact : Artifact
    {
        public MaliciousSprayArtifact() : base("malicious_spray", Rarity.RARE)
        {
            

            SetHighlightRequirement((t, p) => p.DetermineMaterialCompatibility(t, TileMaterial.Nest()));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreSetMaskEvent += OnPreSetMask;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreSetMaskEvent -= OnPreSetMask;
        }

        private void OnPreSetMask(PlayerSetAttributeEvent evt)
        {
            if (evt.player.DetermineMaterialCompatibility(evt.tile, TileMaterial.Nest()) &&
                evt.attributeToReceive.GetRegName() == TileMask.Fractured().GetRegName())
            {
                evt.canceled = true;
                List<Tile> tiles = evt.player.GetHandDeckCopy().Where(t => t != null && t != evt.tile)
                    .Except(evt.player.GetSelectedTilesCopy()).ToList();
                if (tiles.Count > 0)
                {
                    Tile tile = tiles[evt.player.GenerateRandomInt(tiles.Count)];
                    tile.SetMask(TileMask.Corrupted(), evt.player);
                }
            }
        }
    }
}