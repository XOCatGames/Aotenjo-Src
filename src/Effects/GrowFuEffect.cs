using System;

namespace Aotenjo
{
    public class GrowFuEffect : Effect
    {
        private readonly Artifact artifact;
        private readonly Tile tile;
        private readonly string name;
        private readonly int amount;

        public GrowFuEffect(Artifact artifact, Tile tile, int amount, string name = "grow_name")
        {
            this.artifact = artifact;
            this.tile = tile;
            this.name = name;
            this.amount = amount;
        }

        public override void Ingest(Player player)
        {
            tile.addonFu += amount;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func($"effect_{name}");
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }

        public override string GetSoundEffectName()
        {
            return "AddExtraFu";
        }
    }
}