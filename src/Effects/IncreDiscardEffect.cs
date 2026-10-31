using System;

namespace Aotenjo
{
    public class IncreDiscardEffect : ArtifactEffect
    {
        private int num;
        private readonly Tile sourceTile;

        public IncreDiscardEffect(string name, Artifact artifact, int num, Tile sourceTile = null) : base(name, artifact)
        {
            this.num = num;
            this.sourceTile = sourceTile;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return string.Format(func("effect_increase_discard_name"), num);
        }

        public override void Ingest(Player player)
        {
            player.RequestDiscardReward(num, artifact, sourceTile);
        }
    }
}
