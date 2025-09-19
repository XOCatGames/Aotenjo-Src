using System;

namespace Aotenjo
{
    public class DerivedEffect : Effect
    {
        private readonly Effect effectImplementation;

        public DerivedEffect(Effect effect)
        {
            effectImplementation = effect;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return effectImplementation.GetEffectDisplay(func);
        }

        public override Artifact GetEffectSource()
        {
            return effectImplementation.GetEffectSource();
        }

        public override void Ingest(Player player)
        {
            effectImplementation.Ingest(player);
        }

        public override bool ShouldWaitUntilFinished() => false;
    }
}