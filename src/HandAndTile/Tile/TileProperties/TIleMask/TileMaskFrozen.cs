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
            player.DetermineTileSelectivityEvent += RemoveFrozenTiles;
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            player.DetermineTileSelectivityEvent -= RemoveFrozenTiles;
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
