using System.Collections.Generic;

namespace Aotenjo
{
    public class GoldenD4DiceArtifact : Artifact
    {
        public GoldenD4DiceArtifact() : base("golden_d4_dice", Rarity.COMMON)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            int num = 1 + player.GenerateRandomInt(4);
            if (player.GetArtifacts().Contains(Artifacts.LeadBlock))
            {
                effects.Add(new TextEffect("effect_leaded_name", this));
                num = 4;
            }

            effects.Add(new EarnMoneyEffect(num, this));
        }
    }
}