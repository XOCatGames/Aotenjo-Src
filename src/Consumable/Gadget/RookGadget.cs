using Aotenjo;

public class RookGadget : ReusableGadget
{
    public RookGadget() : base("rook", 20, 2, 9)
    {
    }

    public override Rarity GetRarity()
    {
        return Rarity.RARE;
    }

    public override bool CanObtainBy(Player player, bool inShop)
    {
        return player.deck.regName == MahjongDeck.BambooDeck.regName;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (ShouldHighlightTile(tile))
        {
            EventManager.Instance.OnUseRookEvent(this, tile);
            return true;
        }

        return false;
    }
}