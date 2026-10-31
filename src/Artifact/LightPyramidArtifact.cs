using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 保留原先按点数寻找对位牌的规则及选中条件；此迁移不收紧对位牌的类别判断。
    /// </summary>
    public sealed class LightPyramidArtifact : Artifact
    {
        public LightPyramidArtifact() : base("light_pyramid", Rarity.COMMON)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            if (tile.GetCategory() != Tile.Category.Feng) return;
            if(permutation.ToTiles().Any(t2 => t2.GetOrder() == ((tile.GetOrder() - 1 + 2) % 4) + 1))
            {
                effects.Add(new FractureEffect(this, tile));
            }
        }
    }
}
