using System;
using System.Collections.Generic;
using Aotenjo;
using UnityEngine;

[Serializable]
public abstract class Gadget : IPriced, ITileHighlighter
{
    [SerializeField] public string regName;
    [SerializeField] private int id;

    [SerializeField] public int uses;

    [SerializeField] private int price;

    public int Price
    {
        get { return price; }
    }

    public Gadget(string name, int id, int uses, int price)
    {
        regName = name;
        this.id = id;
        this.uses = uses;
        this.price = price;
    }

    public virtual string GetName(Func<string, string> localize)
    {
        return localize($"gadget_{regName}_name");
    }

    public virtual string GetName(Func<string, string> localize, Player player)
    {
        return GetName(localize);
    }

    public virtual string GetDescription(Func<string, string> localize)
    {
        return localize($"gadget_{regName}_description");
    }

    public virtual string GetDescription(Func<string, string> localize, Player player)
    {
        return GetDescription(localize);
    }

    public int GetID()
    {
        return id;
    }

    public virtual int GetDisplayID(Player player)
    {
        return GetID();
    }

    public virtual string GetDisplayRegName(Player player)
    {
        return regName;
    }

    public virtual string GetSpriteNamespaceID(Player player, string nmSpace = "aotenjo")
    {
        return $"gadget:{nmSpace}:{GetDisplayRegName(player)}";
    }

    public virtual bool ShouldDisplayInGrayscale(Player player)
    {
        return false;
    }

    public virtual bool IsConsumable()
    {
        return false;
    }

    public virtual bool CanObtainBy(Player player, bool inShop)
    {
        return true;
    }

    public virtual int GetMaxOnUseNum(Player player)
    {
        return 1;
    }

    public virtual int GetStackLimit()
    {
        return 1;
    }

    public virtual Rarity GetRarity()
    {
        return IsConsumable() ? Rarity.COMMON : Rarity.RARE;
    }

    /// <summary>Called only when this instance enters the inventory, never when copied or restored.</summary>
    public virtual void OnObtained(Player player)
    {
    }

    public virtual void OnRoundStart(Player player)
    {
    }

    public virtual bool UseOnTile(Player player, Tile tile)
    {
        return false;
    }

    /// <summary>Returns success separately from the tiles that should receive an enter-hand animation.</summary>
    public virtual GadgetUseResult UseOnTiles(Player player, List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0 || uses <= 0 || !CanUseOnTiles(tiles, player))
            return GadgetUseResult.Failed;
        return GadgetUseResult.FromSuccess(UseOnTile(player, tiles[0]), tiles);
    }

    public virtual bool ShouldHighlightTile(Tile tile, Player player)
    {
        return true;
    }

    public virtual bool CanUseOnTiles(List<Tile> tiles, Player player)
    {
        return tiles != null && tiles.Count == 1 && tiles[0] != null &&
               ShouldHighlightTile(tiles[0], player);
    }

    public virtual bool UseOnBlock(Player player, Block block)
    {
        return false;
    }

    public virtual bool CanUseOnSettledTiles(Player player)
    {
        return false;
    }

    public Gadget SetUses(int v)
    {
        uses = v;
        return this;
    }

    /// <summary>Creates a detached copy without running inventory or round lifecycle hooks.</summary>
    public Gadget Copy()
    {
        Gadget copy = CreateCopy();
        if (copy == null || ReferenceEquals(copy, this) || copy.GetType() != GetType())
            throw new InvalidOperationException($"{GetType().Name}.CreateCopy must return a new instance of the same type.");
        copy.regName = regName;
        copy.id = id;
        copy.price = price;
        copy.uses = uses;
        CopyStateTo(copy);
        return copy;
    }

    // Parameterized gadgets must explicitly preserve their constructor configuration.
    protected abstract Gadget CreateCopy();

    /// <summary>Deep-copy any mutable instance data here. Never subscribe events or play effects.</summary>
    protected virtual void CopyStateTo(Gadget copy) { }

    public int GetSellingPrice()
    {
        if (!IsConsumable()) return GetRarity() == Rarity.COMMON ? 2 : 3;
        return (uses >= 3 ? 2 : 1);
    }

    public virtual string GetInstruction(Func<string, string> getLocalizedText)
    {
        return getLocalizedText($"gadget_{regName}_instruction");
    }

    public virtual string GetInstruction(Func<string, string> getLocalizedText, Player player)
    {
        return GetInstruction(getLocalizedText);
    }
}
