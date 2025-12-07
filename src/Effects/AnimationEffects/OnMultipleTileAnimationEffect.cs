using System.Collections.Generic;

namespace Aotenjo
{
    public class OnMultipleTileAnimationEffect : IAnimationEffect
    {
        public readonly Effect effect;
        public readonly List<Tile> affectedTiles;
        public readonly Tile mainTile;

        public OnMultipleTileAnimationEffect(Effect effect, List<Tile> affectedTiles, Tile mainTile)
        {
            this.effect = effect;
            this.affectedTiles = affectedTiles;
            this.mainTile = mainTile;
        }
        
        public OnMultipleTileAnimationEffect(Effect effect)
        {
            this.effect = effect;
            this.affectedTiles = new List<Tile>();
            this.mainTile = null;
        }

        public Effect GetEffect()
        {
            return effect.GetEffect();
        }

        public virtual List<Tile> GetAffectedTiles(Player player)
        {
            return affectedTiles;
        }

        public virtual Tile GetMainTile(Player player)
        {
            return mainTile;
        }
    }
}