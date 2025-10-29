using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class OnBlockAnimationEffect : OnMultipleTileAnimationEffect
    {
        private Block block;
        public OnBlockAnimationEffect(Block block, Effect effect): base(effect)
        {
            this.block = block;
        }

        public override List<Tile> GetAffectedTiles(Player player)
        {
            return block.tiles.ToList();
        }

        public override Tile GetMainTile(Player player)
        {
            return block.tiles[1];
        }
    }
}