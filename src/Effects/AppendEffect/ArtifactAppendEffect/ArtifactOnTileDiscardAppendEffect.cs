using System.Collections.Generic;

namespace Aotenjo
{
    public class ArtifactOnTileDiscardAppendEffect : ArtifactAppendEffectBase
    {
        private readonly Tile tile;
        private readonly bool forced;

        public ArtifactOnTileDiscardAppendEffect(Stack<IAnimationEffect> stack, Permutation getAccumulatedPermutation, Player player, Tile tile, bool forced, int i) : base(stack, getAccumulatedPermutation, player, i)
        {
            this.tile = tile;
            this.forced = forced;
        }

        protected override IEnumerable<IAnimationEffect> GetAnimationEffects(Artifact artifact)
        {
            List<IAnimationEffect> effects = new List<IAnimationEffect>();
            artifact.AddDiscardTileEffects(player, tile,effects,forced,false);
            return effects;
        }

        protected override ArtifactAppendEffectBase GetEffectOfNextArtifact()
        {
            return new ArtifactOnTileDiscardAppendEffect(effectStack, permutation, player, tile, forced, i + 1);
        }
    }
}