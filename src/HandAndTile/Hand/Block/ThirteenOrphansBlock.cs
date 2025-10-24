using System;
using System.Linq;
using Aotenjo;

[Serializable]
public class ThirteenOrphansBlock : Block
{
    public ThirteenOrphansBlock(Tile[] tiles)
    {
        if (tiles.Length != 12) throw new ArgumentException();

        Tile[] yaoJius = new Hand("19m19s19p1234567z").tiles.ToArray();

        if (!(yaoJius.Any(excluded => yaoJius.All(t => t != excluded || tiles.Any(inBlock => inBlock.CompatWith(t))))))
            throw new ArgumentException();
        this.tiles = tiles;
    }

    public override String ToFormat()
    {
        return tiles.Select(a => a.ToString()).Aggregate("", (a, b) => a + b);
    }

    public override bool IsABC()
    {
        return false;
    }

    public override bool IsAAA()
    {
        return false;
    }

    public override bool IsAAAA()
    {
        return false;
    }

    public override bool Succ(Block other, int step)
    {
        return false;
    }

    public override bool CompatWith(Block other)
    {
        return false;
    }

    public override bool OfSameCategory(Block other)
    {
        return false;
    }

    public override bool OfSameOrder(Block other)
    {
        return false;
    }

    public override Tile.Category GetCategory()
    {
        return Tile.Category.Feng;
    }
}