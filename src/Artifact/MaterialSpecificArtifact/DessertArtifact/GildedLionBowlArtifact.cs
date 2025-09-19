using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class GildedLionBowlArtifact : Artifact
    {
        public GildedLionBowlArtifact() : base("gilded_lion_bowl", Rarity.EPIC)
        {
            SetHighlightRequirement((tile, player) => tile.properties.material is TileMaterialDessert);
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);

            // 检查是否为甜品牌
            if (!(tile.properties.material is TileMaterialDessert dessert || tile.properties.material.GetRegName() == TileMaterial.IceCream().GetRegName())) return;

            // 添加甜品的计分效果（包括特效如MulFan等）
            effects.Add(new TileScoringEffectAppendEffect(player, tile, perm, player.playHandEffectStack));
        }
    }
} 