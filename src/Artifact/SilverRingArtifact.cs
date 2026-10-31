using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 寻找相邻的两副顺子，通过玩家的组合器判断衔接；满足多组时也只追加一次番数。
    /// </summary>
    public sealed class SilverRingArtifact : Artifact
    {
        public SilverRingArtifact() : base("silver_ring", Rarity.RARE)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.blocks.Any(a => a.IsABC() && permutation.blocks.Any(b => a != b && b.IsABC() && player.GetCombinator().ASuccB(b.tiles[0], a.tiles[2], false))))
                effects.Add(ScoreEffect.AddFan(12, this));
        }
    }
}
