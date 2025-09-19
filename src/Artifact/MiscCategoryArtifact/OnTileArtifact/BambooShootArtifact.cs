using System.Collections.Generic;

namespace Aotenjo
{
    public class BambooShootArtifact : Artifact
    {
        public BambooShootArtifact() : base("bamboo_shoot", Rarity.RARE)
        {
            SetHighlightRequirement((t, p) => t.ContainsGreen(p) && t.GetOrder() < 4);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.Selecting(tile)) return;
            if (tile.ContainsGreen(player) && tile.GetOrder() < 4 &&
                tile.properties.mask.GetRegName() != TileMask.Grow().GetRegName())
            {
                effects.Add(new CleanseEffect(this, tile));
                effects.Add(new GrowEffect(tile, this));
            }
        }
    }
}