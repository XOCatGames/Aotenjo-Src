using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class FracturedMask : TileMask
    {
        [SerializeField] private bool played;

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
            EventBus.Subscribe<PlayerEvents.PostSettlePermutationEvent>(player, ScoringListener);
            EventBus.Subscribe<PlayerRoundEvent.End.PostPre>(Vanish);
        }

        private void ScoringListener(PlayerPermutationEvent e)
        {
            foreach (var tile in e.player.GetScoringTiles(e.permutation)
                         .Union(e.player.GetRiverTiles()
                             .Where(t => t.properties.material is TileMaterialPaleWood w && w.queue.Count != 0))
                         .Where(t => t.properties.mask == this))
            {
                played = true;
            }

        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            EventBus.Unsubscribe<PlayerEvents.PostSettlePermutationEvent>(player, ScoringListener);
            EventBus.Unsubscribe<PlayerRoundEvent.End.PostPre>(Vanish);
        }

        private void Vanish(PlayerEvent eventData)
        {
            Player player = eventData.player;
            foreach (var tile in player.GetAllTiles().Where(t => played && t.properties.mask == this).ToList())
            {
                bool res = player.RemoveTileFromDiscarded(tile, "fractured");
                if (res)
                    MessageManager.Instance.OnRemoveTileEvent(new List<Tile> { tile });
            }
        }
    }
}
