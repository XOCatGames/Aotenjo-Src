using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class SoulScytheArtifact : Artifact
    {
        public SoulScytheArtifact() : base("soul_scythe", Rarity.RARE)
        {
        }

        public override void AppendPostBlockAnimationEffects(Player player, Permutation perm, Block block,
            List<IAnimationEffect> effects)
        {
            base.AppendPostBlockAnimationEffects(player, perm, block, effects);
            if (!block.IsYinSeq()) return;
            if (block.Any(t => !player.Selecting(t))) return;

            effects.Add(new TextEffect("effect_soul_scythe", this).OnBlock(block));

            IEnumerable<Tile> jumpedTiles = player
                .GetUnusedTilesInHand()
                .Where(tile => block.Jumped(tile, player));

            foreach (Tile t in jumpedTiles)
                effects.Add(new FractureEffect(this, t).OnTile(t));
        }
    }
}