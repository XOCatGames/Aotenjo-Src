using System.Collections.Generic;

namespace Aotenjo
{
    public abstract class OnMultipleTileAnimationEffect : IAnimationEffect
    {
        public readonly Effect effect;

        public OnMultipleTileAnimationEffect(Effect effect)
        {
            this.effect = effect;
        }

        public Effect GetEffect()
        {
            return effect.GetEffect();
        }

        public abstract List<Tile> GetAffectedTiles(Player player);

        public abstract Tile GetMainTile(Player player);
    }
}