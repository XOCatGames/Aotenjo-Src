using System;

namespace Aotenjo
{
    public class OnTileAnimationEffect : IAnimationEffect
    {
        public readonly Tile tile;
        public readonly Effect effect;
        public readonly bool isClone;

        [Obsolete("Use IEffect.OnTile(Tile) instead.")]
        public OnTileAnimationEffect(Tile tile, Effect effect, bool isClone = false)
        {
            this.tile = tile;
            this.effect = effect;
            this.isClone = isClone;
        }
        
        public OnTileAnimationEffect Clone()
        {
            return new OnTileAnimationEffect(tile, effect, true);
        }

        public Effect GetEffect()
        {
            return effect.GetEffect();
        }

    }
}
