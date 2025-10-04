using System.Collections.Generic;

namespace Aotenjo
{
    public class ArtifactOnSelfAppendEffect : ArtifactAppendEffectBase
    {
        public ArtifactOnSelfAppendEffect(Stack<IAnimationEffect> effectStack, Permutation permutation, Player player, int i) : base(effectStack, permutation, player, i)
        {
        }

        protected override IEnumerable<IAnimationEffect> GetAnimationEffects(Artifact artifact)
        {
            var effects = new List<Effect>();
            artifact.AppendOnSelfEffects(player, permutation, effects);
            return effects;
        }

        protected override ArtifactAppendEffectBase GetEffectOfNextArtifact()
        {
            return new ArtifactOnSelfAppendEffect(effectStack, permutation, player, i + 1);
        }
    }
}