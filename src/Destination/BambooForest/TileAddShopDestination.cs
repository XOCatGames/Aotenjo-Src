using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TileAddShopDestination : Destination
    {
        public TileAddShopDestination(Player player, bool onSale) : base("tile_add_shop", onSale, player)
        {
        }

        public virtual Pair<Tile[], int> GeneratePool()
        {
            Tile[] tileDraws = GenerateGroup();
            Array.Sort(tileDraws);
            int price = GetPrice(tileDraws);
            return new(tileDraws, price);
        }

        protected virtual Tile[] GenerateGroup()
        {
            return player.GenerateRandomTileGroupWithEffects(GetGroupNum()).ToArray();
        }

        protected virtual int GetGroupNum()
        {
            return 3;
        }

        protected virtual int GetPrice(Tile[] tileDraws)
        {
            bool isAAA = tileDraws[0].CompatWith(tileDraws[1]);
            int price = (isAAA ? 5 : 3);
            if (tileDraws.Any(t => t.IsHonor(player)))
            {
                price += 2;
            }

            foreach (Tile t in tileDraws)
            {
                if (t.properties.material != TileMaterial.PLAIN)
                {
                    price++;
                }

                if (t.properties.font == TileFont.BLUE)
                {
                    price += 2;
                }
            }

            return price;
        }

        public virtual void OnPurchased(Tile[] tiles, int price)
        {
        }


        public static TileAddShopDestination Create(Player player, DestinationEventType type)
        {
            switch (type)
            {
                case DestinationEventType.COMMON:
                    return new TileAddShopDestination(player, false);
                case DestinationEventType.YELLOW:
                    return new TileAddShopDestination(player, true);
                case DestinationEventType.RED:
                    return (player.GenerateRandomInt(4, "destination")) switch
                    {
                        0 => new Corrupted(player, false),
                        1 => new Wild(player, false),
                        2 => new Psionic(player, false),
                        3 => new Overgrown(player, false),
                        _ => new TileAddShopDestination(player, false)
                    };
                default:
                    return new TileAddShopDestination(player, false);
            }
        }

        public override Destination GetRandomRedEventVariant(Player player)
        {
            return Create(player, DestinationEventType.RED);
        }
    }

    public class Corrupted : TileAddShopDestination
    {
        public Corrupted(Player player, bool onSale) : base(player, onSale)
        {
        }

        public override string GetSaleDescription(Func<string, string> localize)
        {
            return localize($"event_{name}_corrupted_description");
        }

        public override bool IsOnEvent()
        {
            return true;
        }

        public override Pair<Tile[], int> GeneratePool()
        {
            Pair<Tile[], int> pair = base.GeneratePool();
            Tile[] tiles = pair.elem1;
            foreach (Tile t in tiles)
            {
                if (player.GenerateRandomInt(3) == 0)
                {
                    t.properties.mask = TileMask.Corrupted();
                }
            }

            return pair;
        }

        protected override int GetPrice(Tile[] tileDraws)
        {
            int price = base.GetPrice(tileDraws);
            return Math.Max(1, price / 3);
        }
    }

    public class Wild : TileAddShopDestination
    {
        public Wild(Player player, bool onSale) : base(player, onSale)
        {
        }

        public override string GetSaleDescription(Func<string, string> localize)
        {
            return localize($"event_{name}_wild_description");
        }

        public override bool IsOnEvent()
        {
            return true;
        }

        public override void OnPurchased(Tile[] tiles, int price)
        {
            base.OnPurchased(tiles, price);
            foreach (Tile _ in tiles)
            {
                List<Tile> cands = new(player.GetTilePool().Except(tiles));
                if (cands.Count == 0) break;
                Tile cand = cands[player.GenerateRandomInt(cands.Count)];
                bool res = player.RemoveTileFromPool(cand);
                if (res)
                    MessageManager.Instance.OnRemoveTileEvent(new List<Tile> { cand });
            }
        }
    }

    public class Psionic : TileAddShopDestination
    {
        private Tile.Category cat;

        public override bool IsOnEvent()
        {
            return true;
        }

        public override string GetSaleDescription(Func<string, string> localize)
        {
            return localize($"event_{name}_psionic_description");
        }

        public Psionic(Player player, bool onSale) : base(player, onSale)
        {
            cat = (Tile.Category)player.GenerateRandomInt(3);
        }

        protected override Tile[] GenerateGroup()
        {
            return player.GenerateRandomTileGroupWithEffects(GetGroupNum()).Select(t => t.SetCategoryForced(cat))
                .ToArray();
        }
    }

    public class Overgrown : TileAddShopDestination
    {
        public Overgrown(Player player, bool onSale) : base(player, onSale)
        {
        }

        public override string GetSaleDescription(Func<string, string> localize)
        {
            return localize($"event_{name}_overgrown_description");
        }

        public override bool IsOnEvent()
        {
            return true;
        }

        protected override int GetGroupNum()
        {
            return 4;
        }

        protected override int GetPrice(Tile[] tileDraws)
        {
            return (int)(base.GetPrice(tileDraws) * 0.6f);
        }

        protected override Tile[] GenerateGroup()
        {
            return player.GenerateRandomTileGroupWithEffects(GetGroupNum(), 60, 30, 10, 35).ToArray();
        }
    }
}