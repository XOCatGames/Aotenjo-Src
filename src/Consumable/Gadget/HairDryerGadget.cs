using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class HairDryerGadget : ReusableGadget
    {
        public HairDryerGadget() : base("hair_dryer", 19, 1, 9)
        {
        }

        public override bool CanObtainBy(Player player, bool inShop)
        {
            return player.deck.regName != MahjongDeck.GalaxyDeck.regName;
        }

        public override bool UseOnBlock(Player player, Block block)
        {
            if(!CanUseOnTiles(block.tiles.ToList(), player)) return false;

            int deviation = block.tiles[1].GetOrder() - 5;

            int shiftValue = deviation > 0
                ? block.tiles.Select(t => 9 - t.GetOrder()).Min()
                : block.tiles.Select(t => 1 - t.GetOrder()).Max();

            foreach (var item in block.tiles)
            {
                item.AddTransform(new TileTransformTrivial(item.GetCategory(), item.GetOrder() + shiftValue), player);
            }

            MessageManager.Instance.OnSoundEvent("HairDryer");
            return true;
        }

        public override bool UseOnTile(Player player, Tile tile)
        {
            if (uses <= 0) return false;
            if (!ShouldHighlightTile(tile, player)) return false;
            Permutation perm = player.GetAccumulatedPermutation();

            Block block = perm?.blocks.FirstOrDefault(b => b.tiles.Contains(tile));
            if (block == null) return false;

            return UseOnBlock(player, block);
        }

        public override bool CanUseOnSettledTiles()
        {
            return false;
        }

        public override int GetMaxOnUseNum()
        {
            return 4;
        }

        public override bool CanUseOnTiles(List<Tile> tiles)
        {
            if (tiles.Count < 3 || tiles.Any(t => !t.IsNumbered()) || uses <= 0) return false;
            Player player = GameManager.Instance.player;
            Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);
            if (formedBlock == null)
            {
                return false;
            }

            int deviation = formedBlock.tiles[1].GetOrder() - 5;

            if (deviation == 0)
            {
                return false;
            }

            return true;
        }

        public override bool UseOnTiles(Player player, List<Tile> tiles)
        {
            if(!CanUseOnTiles(tiles)) return false;
            Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);
            return UseOnBlock(player, formedBlock);
        }
    }
}