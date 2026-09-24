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
            visibleTiles.AddRange(player.GetScoringTiles(perm).Except(visibleTiles));

            pool.AddRange(player.GetTilePool());
            pool.AddRange(player.GetHandDeckCopy());
            pool.AddRange(player.GetScoringTiles(perm));

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

            if (pool.Any(a => a != tile && IsMatchingTile(a, tile))) return false;
            return true;
        }

        private void OnRoundStart(PlayerEvent playerEvent)
        {
            scoredTiles.Clear();
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (isScored(tile) || tile.properties.mask is TileMaskSuppressed) return;

            scoredTiles.Add(tile);

            List<Tile> pool = new List<Tile>();

            pool.AddRange(player.GetTilePool());
            pool.AddRange(player.GetHandDeckCopy());

            Permutation perm = player.GetAccumulatedPermutation();
            pool.AddRange(player.GetScoringTiles(perm)
                .Where(t => perm?.jiang == null || (t != perm.jiang.tile1 && t != perm.jiang.tile2)));

            pool.RemoveAll(a => isScored(a));
            if (pool.Any(a => a != tile && IsMatchingTile(a, tile))) return;
            effects.Add(ScoreEffect.MulFan(1.5, this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PostSettlePermutationEvent>(player, OnRoundStart);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PostSettlePermutationEvent>(player, OnRoundStart);
        }

        private static bool IsMatchingTile(Tile candidate, Tile tile)
        {
            return candidate is FlowerTile || tile is FlowerTile
                ? candidate.GetCategory() == tile.GetCategory() && candidate.GetOrder() == tile.GetOrder()
                : candidate.CompatWith(tile);
        }

        private bool isScored(Tile t)
        {
            return scoredTiles.Contains(t);
        }
    }
}
