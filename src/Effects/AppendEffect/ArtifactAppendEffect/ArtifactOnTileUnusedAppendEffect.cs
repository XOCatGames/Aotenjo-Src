using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class ArtifactOnTileUnusedAppendEffect : ArtifactAppendEffectBase
    {
        private readonly Tile tile;

        public ArtifactOnTileUnusedAppendEffect(Stack<IAnimationEffect> effectStack, Tile tile, Permutation permutation, Player player, int i) : base(effectStack, permutation, player, i)
        {
            this.tile = tile;
        }

        protected override IEnumerable<IAnimationEffect> GetAnimationEffects(Artifact artifact)
        {
            List<Effect> effects = new();
            artifact.AppendOnUnusedTileEffects(player, permutation, tile, effects);
            return effects.Select(e => e.OnTile(tile));
        }

        protected override ArtifactAppendEffectBase GetEffectOfNextArtifact()
        {
            return new ArtifactOnTileUnusedAppendEffect(effectStack, tile, permutation, player, i + 1);
        }
    }
}