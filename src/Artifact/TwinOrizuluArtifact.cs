using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class TwinOrizuluArtifact : Artifact
    {
        public TwinOrizuluArtifact() : base("twin_orizulu", Rarity.RARE)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation perm, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, perm, block, effects);
            if (block.Any(t => !player.Selecting(t))) return;
            if (block.GetCategory() != perm.jiang.GetCategory() && block.IsNumbered() &&
                perm.JiangFulfillAll((t => t.IsNumbered())))
                effects.Add(
                    new TwinEffect(block, this, perm.jiang.GetCategory()));
        }

        private class TwinEffect : Effect
        {
            private readonly Block block;
            private readonly Artifact artifact;
            private readonly Tile.Category category;

            public TwinEffect(Block block, Artifact artifact, Tile.Category category)
            {
                this.block = block;
                this.artifact = artifact;
                this.category = category;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_copied");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                List<Tile> tiles = new(block.tiles);
                List<Tile> tilesToAdd = new List<Tile>();
                foreach (var item in tiles)
                {
                    Tile copy = new Tile(item);
                    copy.ClearTransform(player);
                    copy.SetCategoryForced(category);
                    copy.properties = new(item.properties.CopyWithMask(TileMask.Fractured()));
                    tilesToAdd.Add(copy);
                    player.AddNewTileToPool(copy);
                }
            }
        }
    }
}