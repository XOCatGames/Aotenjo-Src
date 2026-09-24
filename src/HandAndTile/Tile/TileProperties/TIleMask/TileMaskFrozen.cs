using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaskFrozen : TileMask
    {
        public TileMaskFrozen(int id) : base(id, "frozen", null) { }

        public override TileMask Copy() => new TileMaskFrozen(5);

        public override bool IsDebuff() => true;

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            EventBus.Subscribe<PlayerEvents.DetermineTileSelectivityEvent>(player, RemoveFrozenTiles);
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            EventBus.Unsubscribe<PlayerEvents.DetermineTileSelectivityEvent>(player, RemoveFrozenTiles);
        }

        public void RemoveFrozenTiles(PlayerTileEvent evt)
        {
            if (evt.tile?.properties?.mask == this && !evt.player.GetArtifacts().Contains(Artifacts.MeteoriteKnife))
            {
                evt.canceled = true;
            }
        }
    }
}
