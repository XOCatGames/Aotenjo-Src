using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class GadgetsShopDestination : Destination
    {
        public GadgetsShopDestination(Player player, bool onSale, string eventName = "") : base("gadgets_shop", onSale,
            player, eventName)
        {
        }

        public virtual List<ReducableContainer<Gadget>> GetGadgets()
        {
            int gadgetPolls = 5; // 假设玩家可以从5个小工具中选择
            List<ReducableContainer<Gadget>> gadgetDraws = player.GenerateGadgets(gadgetPolls)
                .Select(g => new ReducableContainer<Gadget>(false, g)).ToList();
            if (onSale)
            {
                gadgetDraws.ForEach(g => { g.reduced = true; });
            }

            return gadgetDraws;
        }

        public virtual int GetPrice(Gadget gadget)
        {
            if (onSale) return (int)(gadget.Price * 0.6f);
            return gadget.Price;
        }

        public static GadgetsShopDestination Create(Player player, DestinationEventType type)
        {
            switch (type)
            {
                case DestinationEventType.COMMON:
                    return new GadgetsShopDestination(player, false);
                case DestinationEventType.YELLOW:
                    return new GadgetsShopDestination(player, true);
                case DestinationEventType.RED:
                    return (player.GenerateRandomInt(2, "destination")) switch
                    {
                        0 => new Unguarded(player, false),
                        1 => new Toolshop(player, false),
                        //2 => new Purifying(player, false),
                        //3 => new Fulfilling(player, false),
                        _ => new GadgetsShopDestination(player, false)
                    };
                default:
                    return new GadgetsShopDestination(player, false);
            }
        }

        public override Destination GetRandomRedEventVariant(Player player)
        {
            return Create(player, DestinationEventType.RED);
        }

        private class Unguarded : GadgetsShopDestination
        {
            public Unguarded(Player player, bool onSale) : base(player, onSale, "unguarded")
            {
            }

            public override int GetPrice(Gadget gadget)
            {
                return 0;
            }
        }

        private class Toolshop : GadgetsShopDestination
        {
            public Toolshop(Player player, bool onSale) : base(player, onSale, "toolshop")
            {
            }

            public override List<ReducableContainer<Gadget>> GetGadgets()
            {
                int gadgetPolls = 5; // 假设玩家可以从5个小工具中选择
                List<ReducableContainer<Gadget>> gadgetDraws = player.GenerateGadgets(gadgetPolls, true, 0, 20)
                    .Select(g => new ReducableContainer<Gadget>(true, g)).ToList();
                if (player.GenerateRandomInt(100) == 0)
                {
                    gadgetDraws[0] = new ReducableContainer<Gadget>(true, Gadgets.Eraser);
                }

                return gadgetDraws;
            }

            public override int GetPrice(Gadget gadget)
            {
                return (int)(gadget.Price * 0.6f);
            }
        }
    }
}