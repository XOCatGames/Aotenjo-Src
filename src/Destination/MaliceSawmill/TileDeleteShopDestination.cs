using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TileDeleteShopDestination : Destination
    {
        public int unitPrice;
        public int maxPrice;

        public TileDeleteShopDestination(Player player, bool onSale, string eventName = "", int maxPrice = 3) : base(
            "tile_delete_shop", onSale, player, eventName)
        {
            unitPrice = onSale ? 0 : 1;
            this.maxPrice = maxPrice;
        }

        public virtual List<Tile> PollTilesToDelete()
        {
            List<Tile> tiles = player.DrawTilesFromPool(player.GetSelectionCount());
            tiles.Sort();
            return tiles;
        }

        public override void SetOnSale()
        {
            base.SetOnSale();
            unitPrice = 0;
        }

        public virtual int GetPrice(List<Tile> cands)
        {
            if (unitPrice == 0) return 0;
            return cands.Count * (unitPrice + GetNerfPrice());
        }

        public int GetNerfPrice()
        {
            return player.GetAllTiles().Count() switch
            {
                int n when n < 65 => 4,
                int n when n < 85 => 2,
                int n when n < 115 => 1,
                _ => 0
            };
        }

        public virtual int GetDeleteAmount()
        {
            return 4;
        }

        public virtual bool IsFree()
        {
            return false;
        }

        public virtual void OnPurchased(List<Tile> tiles, int price)
        {
        }

        public static TileDeleteShopDestination Create(Player player, DestinationEventType type)
        {
            switch (type)
            {
                case DestinationEventType.COMMON:
                    return new TileDeleteShopDestination(player, false);
                case DestinationEventType.YELLOW:
                    return new TileDeleteShopDestination(player, true);
                case DestinationEventType.RED:
                    return (player.GenerateRandomInt(4, "destination")) switch
                    {
                        0 => new Cleaving(player, false),
                        1 => new Exorcising(player, false),
                        2 => new Purifying(player, false),
                        3 => new Fulfilling(player, false),
                        _ => new TileDeleteShopDestination(player, false)
                    };
                default:
                    return new TileDeleteShopDestination(player, false);
            }
        }

        public override Destination GetRandomRedEventVariant(Player player)
        {
            return Create(player, DestinationEventType.RED);
        }

        private class Cleaving : TileDeleteShopDestination
        {
            public Cleaving(Player player, bool v) : base(player, v, "cleaving")
            {
            }

            public override int GetDeleteAmount()
            {
                return 2;
            }

            public override void OnPurchased(List<Tile> tiles, int price)
            {
                base.OnPurchased(tiles, price);
                foreach (Tile t in tiles)
                {
                    List<Tile> cands = player.GetTilePool().Where(t2 => t2.CompactWith(t)).ToList();
                    if (cands.Count == 0) continue;
                    foreach (Tile cand in cands)
                    {
                        bool res = player.RemoveTileFromPool(cand);
                        if (res)
                            MessageManager.Instance.OnRemoveTileEvent(new List<Tile> { cand });
                    }
                }
            }
        }

        private class Exorcising : TileDeleteShopDestination
        {
            public Exorcising(Player player, bool v) : base(player, v, "exorcising", 4)
            {
            }

            public override int GetPrice(List<Tile> cands)
            {
                return base.GetPrice(cands) -
                       (unitPrice + GetNerfPrice()) * cands.Where(t => t.IsYaoJiu(player)).Count();
            }
        }

        private class Purifying : TileDeleteShopDestination
        {
            public Purifying(Player player, bool v) : base(player, v, "purifying")
            {
            }

            public override int GetPrice(List<Tile> cands)
            {
                if (cands.Any(t => cands.Any(t2 => t != t2 && t.IsSameCategory(t2))))
                {
                    return base.GetPrice(cands);
                }

                return 0;
            }
        }

        private class Fulfilling : TileDeleteShopDestination
        {
            private List<Tile.Category> cands;

            public Fulfilling(Player player, bool v) : base(player, v, "fulfilling")
            {
            }

            public override List<Tile> PollTilesToDelete()
            {
                if (cands == null || cands.Count == 0) return base.PollTilesToDelete();
                List<Tile> tiles = player.DrawTilesFromPool(player.GetSelectionCount(),
                    t => cands.Any(c => t.CompactWithCategory(c)));
                tiles.Sort();
                return tiles;
            }

            public override void OnPurchased(List<Tile> tiles, int price)
            {
                base.OnPurchased(tiles, price);
                cands = tiles.Select(t => t.GetCategory()).Distinct().ToList();
            }
        }
    }
}