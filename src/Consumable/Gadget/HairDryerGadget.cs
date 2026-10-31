using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class HairDryerGadget : ReusableGadget
    {
        protected override Gadget CreateCopy() => new HairDryerGadget();

        public HairDryerGadget() : base("hair_dryer", 19, 1, 9)
        {
        }

        public override bool CanObtainBy(Player player, bool inShop)
        {
            return player.deck.regName != MahjongDeck.GalaxyDeck.regName;
        }

        private static int GetShift(Block block)
        {
            int deviation = block.tiles[1].GetOrder() - 5;
            return deviation > 0 ? block.tiles.Select(t => 9 - t.GetOrder()).Min() :
                block.tiles.Select(t => 1 - t.GetOrder()).Max();
        }

        private static TileTransform ShiftTransform(Tile tile, int shift) =>
            new TileTransformHairDryer(tile.GetCategory(), tile.GetOrder() + shift,
                shift > 0 ? Direction.RIGHT : Direction.LEFT);

        protected override IReadOnlyList<GadgetTilePreview> BuildPreview(Player player, List<Tile> targets)
        {
            Block block = Block.FormValidBlock(targets.ToArray(), player);
            int shift = GetShift(block);
            return targets.Select(t => GadgetTilePreview.WithTransform(t, ShiftTransform(t, shift))).ToArray();
        }

        public override bool UseOnBlock(Player player, Block block)
        {
            if(!CanUseOnTiles(block.tiles.ToList(), player)) return false;

            int shiftValue = GetShift(block);

            foreach (var item in block.tiles)
            {
                item.AddTransform(
                    ShiftTransform(item, shiftValue),
                    player);
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

        public override bool CanUseOnSettledTiles(Player player)
        {
            return false;
        }

        public override int GetMaxOnUseNum(Player player)
        {
            return 4;
        }

        public override bool CanUseOnTiles(List<Tile> tiles, Player player)
        {
            if (tiles.Count < 3 || tiles.Any(t => !t.IsNumbered()) || uses <= 0) return false;
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

        public override GadgetUseResult UseOnTiles(Player player, List<Tile> tiles)
        {
            if (tiles == null || tiles.Count == 0 || uses <= 0) return GadgetUseResult.Failed;
            if(!CanUseOnTiles(tiles, player)) return GadgetUseResult.Failed;
            Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);
            return GadgetUseResult.FromSuccess(UseOnBlock(player, formedBlock), tiles);
        }
    }
}
