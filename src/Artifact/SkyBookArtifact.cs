using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 整副牌没有玩家规则认定的字牌时，每个面子分别加番，保持原有的触发阶段。
    /// </summary>
    public sealed class SkyBookArtifact : Artifact
    {
        public SkyBookArtifact() : base("sky_book", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsNumbered();
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (permutation.ToTiles().All(t => !t.IsHonor(player)))
            {
                effects.Add(ScoreEffect.AddFan(2, this));
            }
        }
    }
}
