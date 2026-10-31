using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 在自身计分阶段检查雀头均为幺九，保留玩家对幺九判定的扩展。
    /// </summary>
    public sealed class BlackOrizuruArtifact : Artifact
    {
        public BlackOrizuruArtifact() : base("black_orizulu", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.JiangFulfillAll(t => t.IsYaoJiu(player)))
                effects.Add(ScoreEffect.AddFan(6, this));
        }
    }
}
