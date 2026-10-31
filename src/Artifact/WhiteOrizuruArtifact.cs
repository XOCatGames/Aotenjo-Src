using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 在自身计分阶段检查雀头均非幺九，幺九判定继续使用当前玩家的兼容规则。
    /// </summary>
    public sealed class WhiteOrizuruArtifact : Artifact
    {
        public WhiteOrizuruArtifact() : base("white_orizulu", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.JiangFulfillAll(t => !t.IsYaoJiu(player)))
                effects.Add(ScoreEffect.AddFan(4, this));
        }
    }
}
