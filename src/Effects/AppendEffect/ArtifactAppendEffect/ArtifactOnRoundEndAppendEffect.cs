using System.Collections.Generic;

namespace Aotenjo
{
    public class ArtifactOnRoundEndAppendEffect : ArtifactAppendEffectBase
    {
        public ArtifactOnRoundEndAppendEffect(Stack<IAnimationEffect> effectStack, Permutation permutation, Player player, int i) : base(effectStack, permutation, player, i)
        {
        }

        protected override IEnumerable<IAnimationEffect> GetAnimationEffects(Artifact artifact)
        {
            List<IAnimationEffect> effects = new List<IAnimationEffect>();
            artifact.AddOnRoundEndEffects(player, permutation, effects);
            return effects;
        }

        protected override ArtifactAppendEffectBase GetEffectOfNextArtifact()
        {
            return new ArtifactOnRoundEndAppendEffect(effectStack, permutation, player, i + 1);
        }
    }
}