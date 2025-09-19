using System;
using Aotenjo;
using UnityEngine;

[Serializable]
public abstract class TileTransform
{
    [SerializeField] private string name;

    public TileTransform(string name)
    {
        this.name = name;
    }

    public virtual TileTransform Copy()
    {
        return this;
    }

    public virtual int GetTransformedOrder(Tile tile)
    {
        return tile.GetOrder();
    }

    public virtual Tile.Category GetTransformedCategory(Tile tile)
    {
        return tile.GetCategory();
    }

    public virtual bool ChangeBaseDisplay()
    {
        return false;
    }

    public virtual string GetNameKey()
    {
        return $"transform_{name}_name";
    }

    public int GetStackCount(Tile t)
    {
        int i = 0;
        foreach (var transform in t.GetTransforms())
        {
            if (transform.GetNameKey() == GetNameKey())
            {
                i++;
            }

            if (transform.ChangeBaseDisplay())
            {
                i = 0;
            }
        }

        return i;
    }

    public virtual int GetDisplayID(Tile tile)
    {
        return 45;
    }

    public virtual int GetDisplayID(Tile tile, Player player, PlayerStats stats)
    {
        return GetDisplayID(tile);
    }
}

[Serializable]
public abstract class TileTransformOrderShift : TileTransform
{
    [SerializeField] private int shift;

    public TileTransformOrderShift(int shift, string name) : base(name)
    {
        this.shift = shift;
    }

    public override int GetTransformedOrder(Tile tile)
    {
        return tile.GetOrder() + shift;
    }
}

public abstract class TileTransformCategoryMask : TileTransform
{
    private Tile.Category mask;

    public TileTransformCategoryMask(Tile.Category category, string name) : base(name)
    {
        mask = category;
    }

    public override Tile.Category GetTransformedCategory(Tile tile)
    {
        return mask;
    }
}