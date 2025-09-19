using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class GoldenBellArtifact : Artifact
    {
        public GoldenBellArtifact() : base("golden_bell", Rarity.RARE)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation perm, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, perm, block, effects);

            if (!block.IsYinSeq() || block.GetCategory() != Tile.Category.Bing ||
                !block.Any(t => player.Selecting(t))) return;

            var skipped = player.GetHandDeckCopy()
                .Where(t => block.Jumped(t, player))
                .ToList();

            if (skipped.Count == 0) return;

            foreach (var tile in skipped)
            {
                effects.Add(
                    new TransformMaterialEffect(
                        TileMaterial.GOLDEN,
                        this,
                        tile,
                        "effect_golden",
                        "Agate"
                    )
                );
            }
        }
    }
}