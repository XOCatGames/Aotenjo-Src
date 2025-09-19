using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class FirePatternPouchArtifact : CraftableArtifact
    {
        public FirePatternPouchArtifact() : base("fire_pattern_pouch", Rarity.RARE)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);

            if (!(player is SneakyPlayer sneakyPlayer)) return;

            if (AreAllTilesSameCategory(sneakyPlayer.sneakedTiles))
            {
                effects.Add(new AddSameCategoryTileEffect(this, sneakyPlayer));
            }
        }

        // 检查所有牌是否为同一种类
        private bool AreAllTilesSameCategory(List<Tile> tiles)
        {
            if (tiles.Count == 0) return false;

            Tile.Category firstCategory = tiles[0].GetCategory();
            return tiles.All(tile => tile.GetCategory() == firstCategory);
        }

        private class AddSameCategoryTileEffect : Effect
        {
            private readonly FirePatternPouchArtifact artifact;
            private readonly SneakyPlayer sneakyPlayer;

            public AddSameCategoryTileEffect(FirePatternPouchArtifact artifact, SneakyPlayer player)
            {
                this.artifact = artifact;
                this.sneakyPlayer = player;
            }

            public override string GetEffectDisplay(Func<string, string> func)
                => func("effect_fire_pattern_pouch");

            public override Artifact GetEffectSource() => artifact;

            public override void Ingest(Player player)
            {
                if (sneakyPlayer.sneakedTiles.Count == 0) return;

                // 获取锦囊内牌的种类
                Tile.Category targetCategory = sneakyPlayer.sneakedTiles[0].GetCategory();
                int randomOrder;
                if (targetCategory == Tile.Category.Wan || 
                    targetCategory == Tile.Category.Bing || 
                    targetCategory == Tile.Category.Suo)
                {
                    randomOrder = player.GenerateRandomInt(9) + 1;
                }
                else if (targetCategory == Tile.Category.Feng)
                {
                    randomOrder = player.GenerateRandomInt(4) + 1;
                }
                else if (targetCategory == Tile.Category.Jian)
                {
                    randomOrder = player.GenerateRandomInt(3) + 5;
                }
                else
                {
                    randomOrder = 1;
                }

                Tile newTile = new Tile(targetCategory, randomOrder);
                sneakyPlayer.SneakTile(newTile);
            }
        }
    }
}