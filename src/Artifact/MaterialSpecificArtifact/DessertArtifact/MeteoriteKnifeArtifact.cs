using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class MeteoriteKnifeArtifact : Artifact
    {
        private const double FAN_MULTIPLIER = 1.5; // 1.5倍番数

        public MeteoriteKnifeArtifact() : base("meteorite_knife", Rarity.EPIC)
        {
            SetHighlightRequirement((tile, _) => 
                tile.properties.mask.GetRegName() == TileMask.Frozen().GetRegName() ||
                (tile.properties.material is TileMaterialDessert dessert &&
                 dessert.GetTotalUsesConsumed() == 0));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN_MULTIPLIER);
        }

        public override void AppendOnTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, perm, tile, effects);

            // 检查是否为完全新鲜的甜品牌
            if (tile.properties.material is TileMaterialDessert dessert)
            {
                // 完全新鲜：从未消耗过使用次数
                if (dessert.GetTotalUsesConsumed() == 0)
                {
                    effects.Add(ScoreEffect.MulFan(FAN_MULTIPLIER, this));
                }
            }
        }
    }
} 