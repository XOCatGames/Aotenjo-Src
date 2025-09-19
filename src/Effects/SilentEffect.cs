using System;

namespace Aotenjo
{
    public class SilentEffect : Effect
    {
        private readonly Action action;

        public SilentEffect(Action action)
        {
            this.action = action;
        }

        public override string GetEffectDisplay(Func<string, string> func) => "";
        public override Artifact GetEffectSource() => null;
        public override void Ingest(Player player) => action();
        public override bool WillTrigger() => false;
    }
}