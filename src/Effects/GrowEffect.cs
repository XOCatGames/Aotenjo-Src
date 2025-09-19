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
            tile.SetMask(TileMask.Grow(), player);
        }

        public override string GetSoundEffectName()
        {
            return "Grow";
        }
    }
}