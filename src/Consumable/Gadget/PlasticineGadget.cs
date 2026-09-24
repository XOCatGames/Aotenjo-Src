using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class PlasticineGadget : ReusableGadget
{
    protected override Gadget CreateCopy() => new PlasticineGadget();

    public PlasticineGadget() : base("plasticine", 44, 1, 9)
    {
    }

    private static Gadget GetCopiedGadget(Player player)
    {
        return player?.GetLastUsedConsumableGadget();
    }

    public override string GetName(Func<string, string> localize, Player player)
    {
        Gadget copiedGadget = GetCopiedGadget(player);
        return copiedGadget == null
            ? base.GetName(localize, player)
            : string.Format(localize("gadget_plasticine_active_name_format"),
                base.GetName(localize, player), copiedGadget.GetName(localize, player));
    }

    public override string GetDescription(Func<string, string> localize, Player player)
    {
        Gadget copiedGadget = GetCopiedGadget(player);
        return copiedGadget == null
            ? base.GetDescription(localize, player)
            : string.Format(localize("gadget_plasticine_active_description_format"),
                copiedGadget.GetDescription(localize, player));
    }

    public override int GetDisplayID(Player player)
    {
        return GetCopiedGadget(player)?.GetID() ?? GetID();
    }

    public override string GetDisplayRegName(Player player)
    {
        return GetCopiedGadget(player)?.regName ?? base.GetDisplayRegName(player);
    }

    public override string GetSpriteNamespaceID(Player player, string nmSpace = "aotenjo")
    {
        return GetCopiedGadget(player)?.GetSpriteNamespaceID(player, nmSpace) ??
               base.GetSpriteNamespaceID(player, nmSpace);
    }

    public override bool ShouldDisplayInGrayscale(Player player)
    {
        return GetCopiedGadget(player) != null;
    }

    public override int GetMaxOnUseNum(Player player)
    {
        return GetCopiedGadget(player)?.GetMaxOnUseNum(player) ?? base.GetMaxOnUseNum(player);
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return GetCopiedGadget(player)?.ShouldHighlightTile(tile, player) ?? false;
    }

    public override bool CanUseOnTiles(List<Tile> tiles, Player player)
    {
        return GetCopiedGadget(player)?.CanUseOnTiles(tiles, player) ?? false;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        return GetCopiedGadget(player)?.UseOnTile(player, tile) ?? false;
    }

    public override GadgetUseResult UseOnTiles(Player player, List<Tile> tiles)
    {
        if (tiles == null || uses <= 0) return GadgetUseResult.Failed;
        return GetCopiedGadget(player)?.UseOnTiles(player, tiles) ?? GadgetUseResult.Failed;
    }

    public override bool UseOnBlock(Player player, Block block)
    {
        return GetCopiedGadget(player)?.UseOnBlock(player, block) ?? false;
    }

    public override bool CanUseOnSettledTiles(Player player)
    {
        return GetCopiedGadget(player)?.CanUseOnSettledTiles(player) ?? false;
    }

    public override string GetInstruction(Func<string, string> getLocalizedText, Player player)
    {
        return GetCopiedGadget(player)?.GetInstruction(getLocalizedText, player) ??
               base.GetInstruction(getLocalizedText, player);
    }
}
