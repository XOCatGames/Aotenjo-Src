using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 按原始万子类别与大于四的点数逐牌加番，兼容类别不扩大此条件。
    /// </summary>
    public sealed class HatOfFortuneArtifact : Artifact
    {
        public HatOfFortuneArtifact() : base("hat_of_fortune", Rarity.RARE)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.GetCategory() == Tile.Category.Wan && tile.GetOrder() > 4;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.GetOrder() > 4 && tile.GetCategory() == Tile.Category.Wan)
            {
                effects.Add(ScoreEffect.AddFan(5, this));
            }
        }
    }
}
