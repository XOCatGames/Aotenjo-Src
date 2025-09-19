using System.Collections.Generic;

namespace Aotenjo
{
    public class ArtifactOnBlockAppendEffect : ArtifactAppendEffectBase
    {
        private readonly Block block;
        public ArtifactOnBlockAppendEffect(Stack<IAnimationEffect> effectStack, Permutation permutation, Player player, int i, Block block) : base(effectStack, permutation, player, i)
        {
            this.block = block;
        }

        protected override IEnumerable<IAnimationEffect> GetAnimationEffects(Artifact artifact)
        {
            List<Effect> effects = new List<Effect>();
            artifact.AddOnBlockEffects(player, permutation, block, effects);
            return effects.ConvertAll(e => e.OnBlock(block));
        }

        protected override ArtifactAppendEffectBase GetEffectOfNextArtifact()
        {
            return new ArtifactOnBlockAppendEffect(effectStack, permutation, player, i + 1, block);
        }
    }
}