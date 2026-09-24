namespace Aotenjo
{
    public class FishingRodGadget : ReusableGadget
    {
        protected override Gadget CreateCopy() => new FishingRodGadget();

        public FishingRodGadget() : base("fishing_rod", 28, 2, 13)
        {
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            return player.GetRiverTiles().Count != 0;
        }

        public override int GetMaxOnUseNum(Player player)
        {
            return 1;
        }

        public override bool CanObtainBy(Player player, bool inShop)
        {
            return player.materialSet.id == MaterialSet.Wood.id;
        }

        public override bool UseOnTile(Player player, Tile tile)
        {
            if (!ShouldHighlightTile(tile, player)) return false;
            MessageManager.Instance.OnUseFishingRodEvent(new PlayerGadgetEvent(player, this, tile));
            return true;
        }
    }
}