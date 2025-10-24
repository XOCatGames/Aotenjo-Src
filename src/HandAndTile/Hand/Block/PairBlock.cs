using System;
using Aotenjo;

[Serializable]
public class PairBlock : Block
{
    public PairBlock(Tile[] tiles)
    {
        if (tiles.Length != 2) throw new ArgumentException();
        if (!tiles[0].CompatWith(tiles[1])) throw new ArgumentException();
        this.tiles = tiles;
    }

    public PairBlock(Tile t1, Tile t2) : this(new[] { t1, t2 })
    {
    }

    public override String ToFormat()
    {
        return tiles[0].GetOrder().ToString() + tiles[1].GetOrder() + Tile.GetCharFromCategory(GetCategory());
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
        return other is PairBlock p && IsNumbered() && other.IsNumbered() &&
               (p.tiles[0].GetOrder() + step == tiles[0].GetOrder() ||
                p.tiles[0].GetOrder() + step == tiles[0].GetOrder());
    }

    public override bool CompatWith(Block other)
    {
        if (other is not PairBlock) return false;
        return tiles[0].CompatWith(other.tiles[0])
               && tiles[1].CompatWith(other.tiles[1]);
    }

    public override bool OfSameCategory(Block other)
    {
        return tiles[0].IsSameCategory(other.tiles[0])
               && tiles[1].IsSameCategory(other.tiles[1]);
    }

    public override bool OfSameOrder(Block other)
    {
        if (other.OfCategory(Tile.Category.Feng) || other.OfCategory(Tile.Category.Jian))
        {
            return other.CompatWith(this);
        }

        return tiles[0].IsSameOrder(other.tiles[0])
               && tiles[1].IsSameOrder(other.tiles[1]);
    }
}