using System.Linq;

namespace Aotenjo
{
    public class WeightGadget : Gadget
    {
        public WeightGadget() : base("weight", 32, 3, 7)
        {
        }

        public override bool IsConsumable()
        {
            return true;
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            return true;
        }

        public override int GetStackLimit()
        {
            return 5;
        }

        public override bool UseOnTile(Player player, Tile tile)
        {
            foreach (Tile t in player.GetHandDeckCopy().Where(t => t.GetCategory() == tile.GetCategory()))
            {
                MessageManager.Instance.EnqueueToDiscard(t, true);
            }

            return true;
        }
    }
}