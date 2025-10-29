using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class Block
    {
        public Tile[] tiles;

        public Block()
        {
        }

        public Block(Tile[] tiles)
        {
            this.tiles = tiles;
            Array.Sort(this.tiles, (x, y) => CompareTile(x, y, tiles));
            if (tiles.Length != 3 && tiles.Count(t => t.CompatWith(tiles[0])) != 4)
            {
                throw new ArgumentException("Invalid Block structure Received");
            }
        }

        private static int CompareTile(Tile x, Tile y, Tile[] tiles)
        {
            int x_order = x.GetOrder();
            int y_order = y.GetOrder();
            if (tiles.Any(a => a.GetOrder() == 1) && tiles.Any(a => a.GetOrder() == 9))
            {
                if (x_order <= 5) x_order += 10;
                if (y_order <= 5) y_order += 10;
            }

            if (tiles.Any(a => a.CompatWith(new Tile("1z"))) && tiles.Any(a => a.CompatWith(new Tile("4z"))))
            {
                if (x_order <= 2) x_order += 4;
                if (y_order <= 2) y_order += 4;
            }

            return x_order.CompareTo(y_order);
        }

        public Block(string representation)
        {
            char[] chars = representation.ToCharArray();

            char men = chars[3];

            tiles = new Tile[]
            {
                new(chars[0].ToString() + men),
                new(chars[1].ToString() + men),
                new(chars[2].ToString() + men)
            };
        }

        public virtual bool All(Predicate<Tile> predicate)
        {
            return tiles.All(a => predicate(a));
        }

        public virtual bool Any(Predicate<Tile> predicate)
        {
            return tiles.Any(a => predicate(a));
        }

        public virtual bool CompatWithNumbers(string numbers)
        {
            string strRepresentation = ToFormat();
            return numbers.All(c => strRepresentation.Contains(c));
        }

        public virtual bool IsABC()
        {
            return tiles.Any(a => tiles.Any(b => a != b && !a.CompatWith(b)));
        }

        public virtual bool IsAAA()
        {
            return tiles[1].IsSameOrder(tiles[0]) && tiles[2].IsSameOrder(tiles[0]) && tiles[1].IsSameOrder(tiles[2]);
        }

        public virtual bool IsAAAA()
        {
            if (tiles.Length != 4) return false;
            return tiles[1].IsSameOrder(tiles[0]) && tiles[2].IsSameOrder(tiles[0]) && tiles[1].IsSameOrder(tiles[2]) &&
                   tiles[3].IsSameOrder(tiles[0]);
        }

        public bool IsNumbered()
        {
            return tiles.All(t => t.IsNumbered());
        }

        public virtual Tile.Category GetCategory()
        {
            foreach (Tile.Category category in Enum.GetValues(typeof(Tile.Category)))
            {
                if (OfCategory(category))
                {
                    return category;
                }
            }

            Debug.LogError("Block has Invalid Category");
            throw new ArgumentException("Block has Invalid Category");
        }

        public virtual bool Succ(Block other, int step)
        {
            if (other == this || !IsNumbered() || !other.IsNumbered())
            {
                return false;
            }

            if (this is PairBlock)
            {
                return other is PairBlock && tiles[0].GetOrder() == other.tiles[0].GetOrder() + step;
            }

            if (IsAAA())
            {
                return other.IsAAA() && tiles[0].GetOrder() == other.tiles[0].GetOrder() + step;
            }

            return other.IsABC() && tiles[0].GetOrder() == other.tiles[0].GetOrder() + step;
        }

        public virtual bool CompatWith(Block other)
        {
            return tiles[0].CompatWith(other.tiles[0])
                   && tiles[1].CompatWith(other.tiles[1])
                   && tiles[2].CompatWith(other.tiles[2]);
        }
        
        public virtual bool CompatWithRepresentation(string representation)
        {
            return CompatWith(new Block(representation));
        }
        
        public virtual bool IsAAAOf(string representation)
        {
            return IsAAA() && All(t => t.CompatWith(representation));
        }
        
        public virtual bool OfCategory(Tile.Category category)
        {
            return All(t => t.CompatWithCategory(category));
        }

        public virtual bool OfSameCategory(Block other)
        {
            return tiles[0].IsSameCategory(other.tiles[0])
                   && tiles[1].IsSameCategory(other.tiles[1])
                   && tiles[2].IsSameCategory(other.tiles[2]);
        }

        public virtual bool OfSameOrder(Block other)
        {
            if (other.OfCategory(Tile.Category.Feng) || other.OfCategory(Tile.Category.Jian))
            {
                return other.CompatWith(this);
            }

            return tiles[0].IsSameOrder(other.tiles[0])
                   && tiles[1].IsSameOrder(other.tiles[1])
                   && tiles[2].IsSameOrder(other.tiles[2]);
        }

        public virtual string ToFormat()
        {
            return tiles[0].GetOrder().ToString() + tiles[1].GetOrder() + tiles[2].GetOrder() +
                   Tile.GetCharFromCategory(tiles[0].GetCategory());
        }

        public virtual string GetSpriteString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tile tile in tiles)
            {
                sb.Append(tile.GetSpriteString());
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new("Block{");
            sb.Append("tiles=").Append(tiles);
            sb.Append('}');
            return sb.ToString();
        }

        public static Block FormValidBlock(Tile[] tiles, Player player)
        {
            if (tiles.Length != 3 && tiles.Length != 4)
            {
                return null;
            }

            if (tiles.Length == 4)
            {
                if (tiles[0].CompatWith(tiles[1]) && tiles[1].CompatWith(tiles[2]) && tiles[2].CompatWith(tiles[3]))
                {
                    return new Block(tiles);
                }

                return null;
            }

            Tile tile1 = tiles[0];
            Tile tile2 = tiles[1];
            Tile tile3 = tiles[2];

            BlockCombinator combinator = player.GetCombinator();

            if (combinator.IsIdentical(tile1, tile2) && combinator.IsIdentical(tile2, tile3))
            {
                return new Block(tiles);
            }

            if (tiles.Any(t1 => tiles.Any(t2 =>
                    t1 != t2 && combinator.ASuccBFormingBlock(t2, t1) && tiles.Any(t3 =>
                        t3 != t2 && t3 != t1 && combinator.ASuccBFormingBlock(t3, t2)))))
            {
                return new Block(tiles);
            }

            return null;
        }

        public bool Kong(Tile tile)
        {
            if (tiles.Length != 3 || IsAAAA() || !IsAAA() || !tiles.All(t => t.CompatWith(tile)))
            {
                return false;
            }

            Array.Resize(ref tiles, 4);
            tiles[3] = tile;
            return true;
        }

        internal bool IsYinSeq()
        {
            return IsABC() && tiles[0].GetOrder() + 2 == tiles[1].GetOrder();
        }
        internal bool IsYangSeq()
        {
            return IsABC() && tiles[0].GetOrder() + 1 == tiles[1].GetOrder();
        }
        

        internal bool IsHonor(Player player)
        {
            return tiles.All(t => t.IsHonor(player));
        }

        public bool Jumped(Tile tile, Player player)
        {
            if (!IsYinSeq()) return false;

            if (tile.GetCategory() != GetCategory())
                return false;

            int minOrder = tiles.Min(t => t.GetOrder());
            int maxOrder = tiles.Max(t => t.GetOrder());
            int ord = tile.GetOrder();

            bool between = ord > minOrder && ord < maxOrder;
            bool inThisHead = tiles.Any(t => t.GetOrder() == ord);

            return between && !inThisHead;
        }

        public bool SelectingBy(Player player)
        {
            return All(player.Selecting);
        }

        #region 雀头类

        [Serializable]
        public class Jiang
        {
            public Tile tile1;
            public Tile tile2;

            public Jiang(Tile tile1, Tile tile2)
            {
                this.tile1 = tile1;
                this.tile2 = tile2;
            }

            public Jiang(string stringRepresentation)
            {
                tile1 = new Tile(stringRepresentation[1..]);
                tile2 = new Tile(stringRepresentation[1..]);
            }

            public override string ToString()
            {
                return "Jiang{" + "tile1=" + tile1 + ", tile2=" + tile2 + '}';
            }

            public virtual string ToFormat()
            {
                return (tile1.GetOrder()).ToString() + tile2.GetOrder() + Tile.GetCharFromCategory(tile1.GetCategory());
            }

            public Tile.Category GetCategory()
            {
                foreach (Tile.Category category in Enum.GetValues(typeof(Tile.Category)))
                {
                    if (tile1.CompatWithCategory(category))
                    {
                        return category;
                    }
                }

                throw new ArgumentException("Jiang has Invalid Category");
            }

            public bool All(Predicate<Tile> predicate)
            {
                return predicate.Invoke(tile1) && predicate.Invoke(tile2);
            }

            public bool Contains(Tile tile)
            {
                return tile1 == tile || tile2 == tile;
            }

            public bool Any(Predicate<Tile> predicate)
            {
                return predicate.Invoke(tile1) || predicate.Invoke(tile2);
            }

            public PairBlock ToPairBlock()
            {
                return new PairBlock(tile1, tile2);
            }
        }

        #endregion

        public bool IsMirageOf(Block block1, Player status, bool catSensitive = false)
        {
            return status.DetermineShiftedPair(this, block1, 0, catSensitive);
        }
    }
}