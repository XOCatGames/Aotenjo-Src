using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 赠牌只在真正获得时由基类执行；构造仅配置数量和颜色，预览或读档不得直接染牌。
    /// </summary>
    public sealed class RedWoodArtifact : Artifact
    {
        public RedWoodArtifact() : base("red_wood", Rarity.EPIC)
        {
            SetHighlightRequirement(MatchesTile);
            SetFontGift(() => TileFont.RED, 5);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.properties.font.GetRegName() == TileFont.RED.GetRegName();
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.properties.font.GetRegName() == TileFont.RED.GetRegName()) effects.Add(ScoreEffect.MulFan(1.2f, this));
        }
    }
}
