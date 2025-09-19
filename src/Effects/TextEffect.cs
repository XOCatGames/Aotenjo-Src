using System;
using Aotenjo;

namespace Aotenjo
{
    [Serializable]
    public class TextEffect : Effect
    {
        private string text;
        private Artifact source;
        private string soundName;

        public TextEffect(string text, Artifact source = null, string soundName = "AddFu")
        {
            this.text = text;
            this.source = source;
            this.soundName = soundName;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func(text);
        }

        public override Artifact GetEffectSource()
        {
            return source;
        }

        public override string GetSoundEffectName()
        {
            return soundName;
        }

        public override void Ingest(Player player)
        {
        }
    }
}