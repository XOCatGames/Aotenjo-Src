using System;

namespace Aotenjo
{
    public class SimpleEffect : TextEffect
    {
        private Action<Player> consume;

        private Func<Player, Func<string, string>, string> GetText;

        public SimpleEffect(string text, Artifact source, Action<Player> consume, string soundName = "AddFu") : base(
            text, source, soundName)
        {
            this.consume = consume;
            GetText = base.GetEffectDisplay;
        }

        public SimpleEffect(Func<Player, Func<string, string>, string> provider, Artifact source,
            Action<Player> consume, string soundName = "AddFu") : base("ui_error", source, soundName)
        {
            this.consume = consume;
            GetText = provider;
        }

        public override void Ingest(Player player)
        {
            base.Ingest(player);
            consume(player);
        }

        public override string GetEffectDisplay(Player player, Func<string, string> localizationMethod)
        {
            return GetText(player, localizationMethod);
        }
    }
}