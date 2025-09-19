using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaskGrow : TileMask
    {
        public TileMaskGrow(int id) : base(id, "grow", null)
        {
        }

        public override bool IsDebuff()
        {
            return false;
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public void PostRoundEnd(PlayerEvent eventData)
        {
            Player player = eventData.player;
            foreach (var tile in player.GetAllTiles().Where(t => t.properties.mask == this))
            {
                if (tile.IsNumbered() && tile.GetBaseOrder() < 9)
                {
                    tile.ModifyOrder(tile.GetBaseOrder() + 1, player);
                }
                else
                {
                    tile.addonFu += 3;
                }

                tile.SetMask(NONE, player, true);
            }
        }
    }
}