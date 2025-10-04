using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class FracturedMask : TileMask
    {
        private bool played;

        public FracturedMask(int id) : base(id, "fractured", null)
        {
            played = false;
        }

        public override TileMask Copy()
        {
            return new FracturedMask(3);
        }

        public override bool IsDebuff()
        {
            return true;
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            player.PostSettlePermutationEvent += ScoringListener;
            EventBus.Subscribe<PlayerRoundEvent.End.PostPre>(Vanish);
        }

        private void ScoringListener(PlayerPermutationEvent e)
        {
            foreach (var tile in e.permutation.ToTiles()
                         .Union(e.player.GetRiverTiles()
                             .Where(t => t.properties.material is TileMaterialPaleWood w && w.queue.Count != 0))
                         .Where(t => t.properties.mask == this))
            {
                played = true;
            }

            if (e.player is RainbowDeck.RainbowPlayer rainbowPlayer)
            {
                foreach (var item in rainbowPlayer.PlayedFlowerTiles.Where(t => t.properties.mask == this))
                {
                    played = true;
                }
            }
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            player.PostSettlePermutationEvent -= ScoringListener;
            EventBus.Unsubscribe<PlayerRoundEvent.End.PostPre>(Vanish);
        }

        private void Vanish(PlayerEvent eventData)
        {
            Player player = eventData.player;
            foreach (var tile in player.GetAllTiles().Where(t => (played && t.properties.mask == this)))
            {
                bool res;
                if (tile is FlowerTile f)
                {
                    if (player is RainbowDeck.RainbowPlayer rainbowPlayer)
                    {
                        res = rainbowPlayer.PlayedFlowerTiles.Remove(f);
                        if (res)
                            MessageManager.Instance.OnRemoveTileEvent(new List<Tile> { tile });
                    }
                }
                else
                {
                    res = player.RemoveTileFromDiscarded(tile, "fractured");
                    if (res)
                        MessageManager.Instance.OnRemoveTileEvent(new List<Tile> { tile });
                }
            }
        }
    }
}