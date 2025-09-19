using System;

namespace Aotenjo
{
    public abstract class ArtifactEffect : Effect
    {
        private string name;
        protected Artifact artifact;

        public ArtifactEffect(string name, Artifact artifact)
        {
            this.name = name;
            this.artifact = artifact;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func($"effect_{name}_name");
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }
    }
}