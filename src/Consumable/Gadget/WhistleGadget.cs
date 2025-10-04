using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class WhistleGadget : ReusableGadget
    {
        public WhistleGadget() : base("whistle", 11, 1, 7)
        {
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            Permutation perm = player.GetAccumulatedPermutation();
            if (perm == null) return false;
            if (perm.GetPermType() == PermutationType.THIRTEEN_ORPHANS)
            {
                return tile.IsYaoJiu(player) && perm.GetLastBlock().tiles.All(t => !t.CompactWith(tile));
            }

            return true;
        }

        public override bool CanUseOnTiles(List<Tile> tiles, Player player)
        {
            Permutation perm = player.GetAccumulatedPermutation();
            if (perm == null) return false;
            if (tiles.Count != 2) return false;
            Tile t1 = tiles[0];
            Tile t2 = tiles[1];
            if (!t1.CompactWith(t2)) return false;
            if (!ShouldHighlightTile(t1, player) || !ShouldHighlightTile(t2, player)) return false;
            return true;
        }

        public override bool UseOnTile(Player player, Tile tile)
        {
            List<Tile> selectingTiles = player.GetSelectedTilesCopy();
            if (!CanUseOnTiles(selectingTiles, player))
            {
                return false;
            }

            return UseOnTilesReturnInfluencedTiles(player, selectingTiles).Count != 0;
        }

        public override List<Tile> UseOnTilesReturnInfluencedTiles(Player player, List<Tile> tiles)
        {
            if (!CanUseOnTiles(tiles, player)) return null;
            Permutation perm = player.GetAccumulatedPermutation();
            Tile t1 = tiles[0];
            Tile t2 = tiles[1];
            player.RemoveTileFromHand(t1);
            player.RemoveTileFromHand(t2);
            Tile jiang1 = perm.jiang.tile1;
            Tile jiang2 = perm.jiang.tile2;
            player.AddTileToHand(jiang1);
            player.AddTileToHand(jiang2);
            player.RemoveTileFromDiscarded(jiang1);
            player.RemoveTileFromDiscarded(jiang2);
            player.AddTileToDiscarded(t1);
            player.AddTileToDiscarded(t2);
            perm.jiang.tile1 = t1;
            perm.jiang.tile2 = t2;
            MessageManager.Instance.OnSoundEvent("Whistle");
            return new List<Tile> { jiang1, jiang2 };
        }

        public override int GetMaxOnUseNum()
        {
            return 2;
        }
    }
}