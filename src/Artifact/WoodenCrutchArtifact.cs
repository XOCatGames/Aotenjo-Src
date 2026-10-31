using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 仅幺九刻子触发面子加番，保留刻子判定和玩家幺九规则，不擅自扩大为所有相同牌面子。
    /// </summary>
    public sealed class WoodenCrutchArtifact : Artifact
    {
        public WoodenCrutchArtifact() : base("wooden_crutch", Rarity.COMMON)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.IsYaoJiu(player);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.IsAAA() && (block.All(a => a.IsYaoJiu(player))))
            {
                effects.Add(ScoreEffect.AddFan(8, this));
            }
        }
    }
}
