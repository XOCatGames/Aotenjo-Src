using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 在自身计分阶段追加一次固定番数，效果来源直接使用当前本局对象。
    /// </summary>
    public sealed class TuningBianzhongArtifact : Artifact
    {
        public TuningBianzhongArtifact() : base("tuning_bianzhong", Rarity.RARE)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            effects.Add(ScoreEffect.AddFan(8, this));
        }
    }
}
