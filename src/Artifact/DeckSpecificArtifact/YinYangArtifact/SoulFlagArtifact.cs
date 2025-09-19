using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class SoulFlagArtifact : Artifact
    {
        public SoulFlagArtifact() : base("soul_flag", Rarity.COMMON)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation perm, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, perm, block, effects);

            if (!block.IsYinSeq()) return;
            if (!block.Any(tile => player.Selecting(tile))) return;

            effects.Add(new AddGhostTileEffect(block.GetCategory(), this));
        }

        private class AddGhostTileEffect : Effect
        {
            private readonly Tile.Category category;
            private readonly SoulFlagArtifact artifact;

            public AddGhostTileEffect(Tile.Category cat, SoulFlagArtifact artifact)
            {
                category = cat;
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                int order = player.GenerateRandomInt(9) + 1;

                Tile ghost = new Tile(category, order);

                ghost.SetMaterial(
                    TileMaterial.Ghost(),
                    player,
                    isCopy: true);

                player.AddNewTileToPool(ghost);
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_soul_flag");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}