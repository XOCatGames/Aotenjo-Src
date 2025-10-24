using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class Love5mArtifact : Artifact
    {
        public Love5mArtifact() : base("heart_5m", Rarity.COMMON)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            if (tile.CompatWithCategory(Tile.Category.Wan))
            {
                if (tile.CompatWith("5m"))
                {
                    effects.Add(new UpgradeEffect(this, tile));
                }
                else
                {
                    if (0 == player.GenerateRandomInt(13))
                    {
                        effects.Add(new UpgradeEffect(this, tile));
                    }
                }
            }
        }

        private class UpgradeEffect : Effect
        {
            private readonly Love5mArtifact artifact;
            private readonly Tile tile;

            public UpgradeEffect(Love5mArtifact artifact, Tile tile)
            {
                this.artifact = artifact;
                this.tile = tile;
            }

            public override void Ingest(Player player)
            {
                tile.SetMaterial(TileMaterial.PINK_PORCELAIN, player);
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_hearted_name");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}