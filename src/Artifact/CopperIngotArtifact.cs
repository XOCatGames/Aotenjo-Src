using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 整副牌缺少任意一种数字牌类别便触发一次；保留十三幺以及无累计牌型时的高亮边界。
    /// </summary>
    public sealed class CopperIngotArtifact : Artifact
    {
        public CopperIngotArtifact() : base("copper_ingot", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            Permutation perm = player.GetAccumulatedPermutation();
            if (tile.IsHonor(player) || perm == null) return true;
            if (perm.GetPermType() == PermutationType.THIRTEEN_ORPHANS) return false;
            return perm.blocks.Where(b => b.IsNumbered()).Select(b => b.GetCategory())
                .Union(new[] { tile.GetCategory() })
                .Distinct()
                .Count() < 2;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            List<Tile> tiles = permutation.ToTiles();
            for(int i = 0; i < 3; i++)
            {
                if(tiles.All(t => t.GetCategory() != (Tile.Category)i))
                {
                    effects.Add(ScoreEffect.AddFan(4, this));
                    return;
                }
            }
        }
    }
}
