using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BagOfMarblesArtifact : SneakyArtifact
    {
        public BagOfMarblesArtifact() : base("bag_of_marbles", Rarity.COMMON)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if (player is SneakyPlayer p)
            {
                List<Tile> tilesTraversed = new List<Tile>();
                foreach (var item in p.sneakedTiles)
                {
                    if (tilesTraversed.Any(t => t.CompactWith(item)) || item.GetCategory() != Tile.Category.Bing)
                    {
                        continue;
                    }

                    tilesTraversed.Add(item);
                    effects.Add(new OnTileAnimationEffect(item, new EarnMoneyEffect(1, this)));
                }
            }
        }
    }
}