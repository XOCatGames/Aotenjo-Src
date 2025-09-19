using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class ArtifactOnTileAppendEffect : ArtifactAppendEffectBase
    {
        private readonly Tile tile;

        public ArtifactOnTileAppendEffect(Stack<IAnimationEffect> effectStack, Permutation permutation,
            Player player, int i, Tile tile) : base(effectStack, permutation, player, i)
        {
            this.tile = tile;
        }

        protected override IEnumerable<IAnimationEffect> GetAnimationEffects(Artifact artifact)
        {
            var onTileEffects = new List<Effect>();
            artifact.AppendOnTileEffects(player, permutation, tile, onTileEffects);
            var animationEffects = onTileEffects.Select(e => e.OnTile(tile));
            return animationEffects;
        }

        protected override ArtifactAppendEffectBase GetEffectOfNextArtifact()
        {
            return new ArtifactOnTileAppendEffect(effectStack, permutation, player, i + 1, tile);
        }
    }
}