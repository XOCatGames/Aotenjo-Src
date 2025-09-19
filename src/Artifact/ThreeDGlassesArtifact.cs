namespace Aotenjo
{
    public class ThreeDGlassesArtifact : Artifact
    {
        public ThreeDGlassesArtifact() : base("3d_glasses", Rarity.COMMON)
        {
            SetHighlightRequirement((t, p) => t.ContainsRed(p) || t.ContainsGreen(p));
        }

        public bool IsActive(Player player)
        {
            if (!player.GetArtifacts().Contains(this)) return false;
            return player.GetArtifacts()[0] == this;
        }
    }
}