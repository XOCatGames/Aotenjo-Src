using System;

namespace Aotenjo
{
    public class SuppressTileEffect : Effect
    {
        private Tile tile;
        private Artifact artifact;

        public SuppressTileEffect(Tile tile, Artifact a)
        {
            this.tile = tile;
            artifact = a;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_suppress");
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }

        public override void Ingest(Player player)
        {
            tile.Suppress(player);
        }
    }
}