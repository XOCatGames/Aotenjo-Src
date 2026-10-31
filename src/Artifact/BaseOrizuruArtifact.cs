using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 在遗物自身计分阶段追加一次固定番数；不改为逐牌或逐面子触发。
    /// </summary>
    public sealed class BaseOrizuruArtifact : Artifact
    {
        public BaseOrizuruArtifact() : base("orizulu", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            effects.Add(ScoreEffect.AddFan(3, this));
        }
    }
}
