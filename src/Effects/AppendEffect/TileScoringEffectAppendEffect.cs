using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TileScoringEffectAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Tile tile;
        private readonly Permutation permutation;
        private readonly Player player;

        public TileScoringEffectAppendEffect(Player player, Tile tile, Permutation permutation,
            Stack<IAnimationEffect> effectStack) : base(effectStack)
        {
            this.tile = tile;
            this.player = player;
            this.permutation = permutation;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            return new List<IAnimationEffect>()
            {
                SimpleAppendEffect.Create(effectStack, () => player.GetBaseEffectFromTile(tile)),
                SimpleAppendEffect.Create(effectStack, () =>
                {
                    List<Effect> styleEffects = new();
                    tile.properties.AppendBonusEffects(styleEffects, permutation, player, tile);
                    return styleEffects.Select(e => e.OnTile(tile)).ToList<IAnimationEffect>();
                }),
                new ArtifactOnTileAppendEffect(effectStack, permutation, player, 0, tile),
                SimpleAppendEffect.Create(effectStack, () =>
                {
                    List<IAnimationEffect> styleEffects = new();
                    player.TriggerOnAddSingleTileScoringEffectEvent(styleEffects, tile, permutation);
                    return styleEffects.ToList();
                }),
            };
        }
    }
}