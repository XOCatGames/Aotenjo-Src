using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 红色字体的收益仅在本次打出时产生，旧面子重算不能重复获得金钱。
    /// </summary>
    public sealed class ShuugiArtifact : Artifact
    {
        public ShuugiArtifact() : base("shuugi", Rarity.RARE)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.properties.font.GetRegName() == TileFont.RED.GetRegName();
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile)) return;
            if (tile.properties.font.GetRegName().Equals(TileFont.RED.GetRegName()))
            {
                effects.Add(new EarnMoneyEffect(2, this));
            }
        }
    }
}
