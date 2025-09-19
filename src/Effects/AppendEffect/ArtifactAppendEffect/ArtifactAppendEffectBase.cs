using System.Collections.Generic;

namespace Aotenjo
{
    public abstract class ArtifactAppendEffectBase : ScoringEffectAppendEffect
    {
        protected readonly Player player;
        protected readonly Permutation permutation;
        protected readonly int i;

        protected ArtifactAppendEffectBase(Stack<IAnimationEffect> effectStack, Permutation permutation, Player player, int i) : base(effectStack)
        {
            this.permutation = permutation;
            this.player = player;
            this.i = i;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            if (i >= player.GetArtifacts().Count) return new List<IAnimationEffect>();
            
            List<IAnimationEffect> effects = new();
            
            var artifact = player.GetArtifacts()[i];
            var animationEffects = GetAnimationEffects(artifact);
            effects.AddRange(animationEffects);
            effects.Add(GetEffectOfNextArtifact());
            return effects;
        }

        protected abstract IEnumerable<IAnimationEffect> GetAnimationEffects(Artifact artifact);
        protected abstract ArtifactAppendEffectBase GetEffectOfNextArtifact();
    }
}