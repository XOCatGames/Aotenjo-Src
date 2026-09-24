using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaskSuppressed : TileMask
    {
        [SerializeReference] private TileMask ori;

        public TileMaskSuppressed(int id, TileMask ori) : base(id, "suppressed", null)
        {
            if (ori is TileMaskSuppressed suppressed)
            {
                this.ori = suppressed.ori;
            }
            else
            {
                this.ori = ori;
            }
        }

        public override TileMask Copy()
        {
            return new TileMaskSuppressed(1, ori.Copy());
        }

        public override bool IsDebuff()
        {
            return true;
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            EventBus.Subscribe<PlayerEvents.PostAddSingleTileAnimationEffectEvent>(player, Suppress);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            EventBus.Unsubscribe<PlayerEvents.PostAddSingleTileAnimationEffectEvent>(player, Suppress);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public void Suppress(Permutation permutation, Player player, List<OnTileAnimationEffect> list, OnTileAnimationEffect eff, Tile tile)
        {
            list.RemoveAll(e => e.tile?.properties.mask == this && e.GetEffect() is not CleanseEffect);
        }

        public void PostRoundEnd(PlayerEvent eventData)
        {
            Player player = eventData.player;
            foreach (var tile in player.GetAllTiles().Where(t => t.properties.mask == this))
            {
                tile.SetMask(ori, player, true);
            }
        }
    }
}
