using System;

namespace Aotenjo
{
    public class GrowEffect : Effect
    {
        private Tile tile;
        private Artifact artifact;

        public GrowEffect(Tile tile, Artifact artifact)
        {
            this.tile = tile;
            this.artifact = artifact;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_grow_name");
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }

        public override void Ingest(Player player)
        {
            if (tile.properties.mask.GetRegName() == TileMask.Grow().GetRegName()) return;
            tile.SetMask(TileMask.Grow(), player);
            if (tile.properties.mask.GetRegName() == TileMask.Grow().GetRegName())
            {
                EventBus.Publish(new MechanicalTileGrownEvent(player, tile, artifact));
            }
        }

        public override string GetSoundEffectName()
        {
            return "Grow";
        }
    }
}
