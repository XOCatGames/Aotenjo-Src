using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TileModifyShopDestination : Destination
    {
        private List<TileProperties> queue;

        public TileModifyShopDestination(Player player, bool onSale, string eventName = "") : base("tile_modify_shop",
            onSale, player, eventName)
        {
            queue = new List<TileProperties>();
        }

        public Tile[] GetTiles()
        {
            int tilePolls = player.GetSelectionCount();
            Tile[] tileDraws = player.DrawTilesFromPool(tilePolls, t => t.IsHonor(player) || t.IsNumbered()).ToArray();
            Array.Sort(tileDraws);
            return tileDraws;
        }

        public virtual void OnPurchased(List<Tile> tiles, PropertiesPack pack)
        {
        }

        public virtual List<PropertiesPack> GenerateRandomProperties()
        {
            return player.GeneratePropertyPacks();
        }

        public static TileModifyShopDestination Create(Player player, DestinationEventType type)
        {
            switch (type)
            {
                case DestinationEventType.COMMON:
                    return new TileModifyShopDestination(player, false);
                case DestinationEventType.YELLOW:
                    return new TileModifyShopDestination(player, true);
                case DestinationEventType.RED:
                    return (player.GenerateRandomInt(3, "destination")) switch
                    {
                        0 => new Thundering(player, false),
                        1 => new Fissuring(player, false),
                        2 => new Heavenly(player, false),
                        //差一个强化遗物的事件
                        _ => new TileModifyShopDestination(player, false)
                    };
                default:
                    return new TileModifyShopDestination(player, false);
            }
        }

        public override Destination GetRandomRedEventVariant(Player player)
        {
            return Create(player, DestinationEventType.RED);
        }
    }

    public class PropertiesPack
    {
        public TileProperties bluePrint;
        public int count;
        public int price;

        public PropertiesPack(TileProperties bluePrint, int count, int price)
        {
            this.bluePrint = bluePrint;
            this.count = count;
            this.price = price;
        }
    }

    public class Thundering : TileModifyShopDestination
    {
        public Thundering(Player player, bool onSale) : base(player, onSale, "thunder")
        {
        }

        public override void OnPurchased(List<Tile> tiles, PropertiesPack pack)
        {
            TileProperties bluePrint = pack.bluePrint;
            foreach (Tile t in tiles)
            {
                List<Tile> cands = player.GetAllTiles().Where(t2 => t2 != t && t2.CompatWith(t)).ToList();
                List<Tile> prefCands = cands
                    .Where(t2 => t2.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName()).ToList();
                if (prefCands.Count != 0) cands = prefCands;
                if (cands.Count == 0) continue;
                Tile cand = cands[player.GenerateRandomInt(cands.Count)];
                cand.MergeAndSetProperties(new(bluePrint), player);
            }
        }
    }

    public class Fissuring : TileModifyShopDestination
    {
        public Fissuring(Player player, bool onSale) : base(player, onSale, "fissure")
        {
        }

        public override List<PropertiesPack> GenerateRandomProperties()
        {
            List<PropertiesPack> propertiesPacks = base.GenerateRandomProperties();
            foreach (var item in propertiesPacks)
            {
                item.price = 0;
            }

            return propertiesPacks;
        }

        public override void OnPurchased(List<Tile> tiles, PropertiesPack pack)
        {
            foreach (Tile t in tiles)
            {
                t.SetMask(TileMask.Fractured(), player);
            }
        }
    }

    public class Heavenly : TileModifyShopDestination
    {
        public Heavenly(Player player, bool onSale) : base(player, onSale, "heavenly")
        {
        }

        public override List<PropertiesPack> GenerateRandomProperties()
        {
            List<PropertiesPack> propertiesPacks = player.GeneratePropertyPacks(0, 100);
            foreach (var item in propertiesPacks)
            {
                item.price = 0;
            }

            return propertiesPacks;
        }
    }
}