using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 即使持有铅块也先消耗原有随机抽取，再覆盖点数；提示与计分效果的顺序保持不变。
    /// </summary>
    public sealed class D10DiceArtifact : Artifact
    {
        public D10DiceArtifact() : base("d10_dice", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            int num = player.GenerateRandomInt(10) + 1;
            if (player.HasArtifact(Artifacts.LeadBlock.Id))
            {
                effects.Add(new TextEffect("effect_leaded_name", this));
                num = 10;
            }
            effects.Add(ScoreEffect.AddFan(num, this));
        }
    }
}
