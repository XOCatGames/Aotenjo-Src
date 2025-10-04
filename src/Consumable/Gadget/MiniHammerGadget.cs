using System;
using System.Linq;
using Aotenjo;

[Serializable]
public class MiniHammerGadget : Gadget
{
    public MiniHammerGadget() : base("mini_hammer", 6, 2, 4)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        MessageManager.Instance.OnSoundEvent("MiniHammer");
        tile.SetMask(TileMask.Fractured(), player);
        var cands = player.GetHandDeckCopy().Where(t => t != tile && t.IsSameCategory(tile)
                                                                  && !t.properties.mask.GetRegName()
                                                                      .Equals(TileMask.Fractured().GetRegName()))
            .ToList();
        if (cands.Count == 0) return true;
        Tile other = cands[player.GenerateRandomInt(cands.Count)];
        other.SetMask(TileMask.Fractured(), player);
        return true;
    }

    public override bool IsConsumable()
    {
        return true;
    }

    public override int GetStackLimit()
    {
        return 5;
    }

    public override bool ShouldHighlightTile(Tile tile)
    {
        return true;
    }
}