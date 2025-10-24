using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CakeExpertArtifact : Artifact
    {
        private List<Tile> scoredTiles = new List<Tile>();

        public CakeExpertArtifact() : base("cake_expert", Rarity.RARE)
        {
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            Permutation perm = player.GetAccumulatedPermutation();

            List<Tile> visibleTiles = new List<Tile>();
            List<Tile> pool = new List<Tile>();

            if (perm != null)
                visibleTiles.AddRange(perm.ToTiles());
            visibleTiles.AddRange(player.GetHandDeckCopy());

            pool.AddRange(player.GetTilePool());
            pool.AddRange(player.GetHandDeckCopy());
            if (perm != null)
                pool.AddRange(perm.ToTiles());

            foreach (Tile t in visibleTiles)
            {
                if (t != tile)
                {
                    pool.Remove(t);
                }
                else
                {
                    break;
                }
            }

            if (pool.Any(a => a != tile && a.CompatWith(tile))) return false;
            return true;
        }

        private void OnRoundStart(PlayerEvent playerEvent)
        {
            scoredTiles.Clear();
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile is FlowerTile)
            {
                return;
            }

            if (isScored(tile)) return;

            scoredTiles.Add(tile);

            List<Tile> pool = new List<Tile>();

            pool.AddRange(player.GetTilePool());
            pool.AddRange(player.GetHandDeckCopy());

            Permutation perm = player.GetAccumulatedPermutation();
            if (perm != null)
                pool.AddRange(
                    perm.ToTiles().Where(t => t != perm.jiang.tile1 && t != perm.jiang.tile2)
                );

            pool.RemoveAll(a => isScored(a));
            if (pool.Any(a => a != tile && a.CompatWith(tile))) return;
            effects.Add(ScoreEffect.MulFan(1.5, this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostSettlePermutationEvent += OnRoundStart;
        }

        private bool isScored(Tile t)
        {
            return scoredTiles.Contains(t);
        }
    }
}