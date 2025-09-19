using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class CopperLionArtifact : CraftableArtifact
    {
        private const int FU = 40;

        public CopperLionArtifact() : base("copper_lion", Rarity.RARE)
        {
            SetHighlightRequirement((t, _) => t.GetOrder() > 6);
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> lst)
        {
            base.AddOnBlockEffects(player, permutation, block, lst);
            if ((block.IsAAA()) && block.All(t => t.IsNumbered()))
                lst.Add(ScoreEffect.AddFu(FU, this));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.Selecting(tile)) return;
            if (tile.IsNumbered() && tile.GetOrder() >= 7 &&
                tile.properties.material.GetRegName() != TileMaterial.COPPER.GetRegName())
            {
                effects.Add(new TransformMaterialEffect(TileMaterial.COPPER, this, tile,
                    "effect_transform_copperize_name"));
            }
        }
    }
}