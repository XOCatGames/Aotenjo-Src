using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 一九必须同时出现在包含当前牌的同一面子内，且当前牌处于选中状态，才追加染色。
    /// </summary>
    public sealed class OuroborosNecklessArtifact : Artifact
    {
        public OuroborosNecklessArtifact() : base("ouroboros_neckless", Rarity.COMMON)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            int order = tile.GetOrder();
            if (order != 1 && order != 9) return;
            if (permutation.blocks.Any(b => b.tiles.Any(t => t.GetOrder() == 1) && b.tiles.Any(t => t.GetOrder() == 9) && b.tiles.Any(t => t == tile)))
                effects.Add(new TransformColorEffect(TileFont.RED, this, tile, "effect_dyed_name"));
        }
    }
}
