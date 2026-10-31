using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 铅块只覆盖抽到的点数，不省略抽取；保留随机流以及提示先于倍率的结算顺序。
    /// </summary>
    public sealed class IvoryD3DiceArtifact : Artifact
    {
        public IvoryD3DiceArtifact() : base("ivory_d3_dice", Rarity.EPIC)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            int num = player.GenerateRandomInt(3) + 1;
            if (player.HasArtifact(Artifacts.LeadBlock.Id))
            {
                effects.Add(new TextEffect("effect_leaded_name", this));
                num = 3;
            }
            effects.Add(ScoreEffect.MulFan(num, this));
        }
    }
}
