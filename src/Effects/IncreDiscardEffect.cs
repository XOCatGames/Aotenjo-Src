using System;

namespace Aotenjo
{
    public class IncreDiscardEffect : ArtifactEffect
    {
        private int num;

        public IncreDiscardEffect(string name, Artifact artifact, int num) : base(name, artifact)
        {
            this.num = num;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return string.Format(func("effect_increase_discard_name"), num);
        }

        public override void Ingest(Player player)
        {
            player.DiscardLeft += num;
            AudioSystem.PlayAddSwapChanceSound();
        }
    }
}