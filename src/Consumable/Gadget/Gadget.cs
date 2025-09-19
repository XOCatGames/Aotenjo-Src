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

    public virtual string GetDescription(Func<string, string> localize)
    {
        return localize($"gadget_{regName}_description");
    }

    public int GetID()
    {
        return id;
    }

    public virtual bool IsConsumable()
    {
        return false;
    }

    public virtual bool CanObtainBy(Player player, bool inShop)
    {
        return true;
    }

    public virtual int GetMaxOnUseNum()
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

    public virtual void ResetState(Player player)
    {
    }

    public virtual bool UseOnTile(Player player, Tile tile)
    {
        return false;
    }

    /// <summary>
    /// 对指定的手牌使用小道具
    /// </summary>
    /// <param name="player">玩家实体</param>
    /// <param name="tiles">对象手牌</param>
    /// <returns>受影响的手牌，将会播放入手动画</returns>
    public virtual List<Tile> UseOnTilesReturnInfluencedTiles(Player player, List<Tile> tiles)
    {
        return UseOnTiles(player, tiles) ? tiles : null;
    }

    public virtual bool UseOnTiles(Player player, List<Tile> tiles)
    {
        return UseOnTile(player, tiles[0]);
    }

    public virtual bool ShouldHighlightTile(Tile tile, Player player)
    {
        return ShouldHighlightTile(tile);
    }

    public virtual bool ShouldHighlightTile(Tile tile)
    {
        return true;
    }

    public virtual bool CanUseOnTiles(List<Tile> tiles, Player player)
    {
        return CanUseOnTiles(tiles);
    }

    public virtual bool CanUseOnTiles(List<Tile> tiles)
    {
        return tiles.Count == 1 && ShouldHighlightTile(tiles[0]);
    }

    public virtual bool UseOnBlock(Player player, Block block)
    {
        return false;
    }

    public virtual bool CanUseOnSettledTiles()
    {
        return false;
    }

    public Gadget SetUses(int v)
    {
        uses = v;
        return this;
    }

    public virtual Gadget Copy()
    {
        return ((Gadget)GetType().GetConstructor(new Type[] { }).Invoke(new object[] { })).SetUses(uses);
    }

    public int GetSellingPrice()
    {
        if (!IsConsumable()) return GetRarity() == Rarity.COMMON ? 2 : 3;
        return (uses >= 3 ? 2 : 1);
    }

    public virtual string GetInstruction(Func<string, string> getLocalizedText)
    {
        return getLocalizedText($"gadget_{regName}_instruction");
    }
}