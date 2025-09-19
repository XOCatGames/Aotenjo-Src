using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class WaterPatternPouchArtifact : SneakyArtifact
    {
        public WaterPatternPouchArtifact() : base("water_pattern_pouch", Rarity.COMMON)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);

            if (!(player is SneakyPlayer sneakyPlayer)) return;

            // 找到锦囊中的所有甜品牌（有消耗过使用次数的）
            var dessertTilesInBag = sneakyPlayer.sneakedTiles
                .Where(tile => tile.properties.material is TileMaterialDessert dessert && 
                               dessert.GetTotalUsesConsumed() > 0)
                .ToList();

            foreach (var tile in dessertTilesInBag)
            {
                effects.Add(new RestoreFreshnessEffect(this, tile));
            }
        }

        private class RestoreFreshnessEffect : Effect
        {
            private readonly WaterPatternPouchArtifact artifact;
            private readonly Tile targetTile;

            public RestoreFreshnessEffect(WaterPatternPouchArtifact artifact, Tile tile)
            {
                this.artifact = artifact;
                this.targetTile = tile;
            }

            public override string GetEffectDisplay(Func<string, string> func)
                => func("effect_restore_freshness");

            public override Artifact GetEffectSource() => artifact;

            public override void Ingest(Player player)
            {
                if (targetTile.properties.material is TileMaterialDessert dessert)
                {
                    // 使用新的恢复新鲜度方法
                    dessert.RestoreFreshness();
                }
            }
        }
    }
} 