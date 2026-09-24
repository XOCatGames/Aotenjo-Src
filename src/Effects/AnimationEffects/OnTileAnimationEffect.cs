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
            : this(tile, effect, isClone, false)
        {
        }

        private OnTileAnimationEffect(Tile tile, Effect effect, bool isClone, bool _)
        {
            this.tile = tile;
            this.effect = effect;
            this.isClone = isClone;
        }

        internal static OnTileAnimationEffect Create(Tile tile, Effect effect, bool isClone = false)
        {
            return new OnTileAnimationEffect(tile, effect, isClone, false);
        }
        
        public OnTileAnimationEffect Clone()
        {
            return Create(tile, effect, true);
        }

        public Effect GetEffect()
        {
            return effect.GetEffect();
        }

    }
}
