using System.Collections.Generic;

namespace Aotenjo
{
    public class GoldenThumbRingArtifact : Artifact
    {
        private const int MONEY = 1;

        public GoldenThumbRingArtifact() : base("golden_thumb_ring", Rarity.RARE)
        {
            SetHighlightRequirement((tile, _) =>
                tile.GetCategory() == Tile.Category.Bing &&
                tile.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName());
        }

        public override void AppendDiscardTileEffects(Player player, Tile tile,
            List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AppendDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            if (tile.GetCategory() == Tile.Category.Bing &&
                tile.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName())
            {
                onDiscardTileEffects.Add(new EarnMoneyEffect(-MONEY, this).OnTile(tile));
                onDiscardTileEffects.Add(
                    new TransformMaterialEffect(TileMaterial.NanmuWood(), this, tile, "effect_golden").OnTile(tile));
                if (withForce)
                {
                    onDiscardTileEffects.Add(new EarnMoneyEffect(MONEY, this).OnTile(tile));
                }
            }
        }
    }
}