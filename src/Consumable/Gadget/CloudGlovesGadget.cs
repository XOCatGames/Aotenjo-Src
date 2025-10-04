using Aotenjo;

public class CloudGlovesGadget : ReusableGadget
{
    public CloudGlovesGadget() : base("cloud_gloves", 26, 5, 99)
    {
    }

    public override Rarity GetRarity()
    {
        return Rarity.EPIC;
    }

    public override bool CanObtainBy(Player player, bool inShop)
    {
        return false;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (ShouldHighlightTile(tile, player))
        {
            SneakyPlayer sneakyPlayer = (SneakyPlayer)player;
            MessageManager.Instance.OnSneakTile(tile, sneakyPlayer);
            return true;
        }

        return false;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return ((SneakyPlayer)player).CanSneakTile(tile);
    }

    public void Evolve()
    {
        maxUseCount += 2;
    }
}