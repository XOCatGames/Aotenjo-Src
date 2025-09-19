namespace Aotenjo
{
    public class PlayerArtifactEvent : PlayerEvent
    {
        public Artifact artifact;

        public PlayerArtifactEvent(Player player, Artifact artifact) : base(player)
        {
            this.artifact = artifact;
        }

        public class DetermineGettability : PlayerArtifactEvent
        {
            public bool res;

            public DetermineGettability(Player player, Artifact artifact, bool res) : base(player, artifact)
            {
                this.res = res;
            }
        }
    }
}