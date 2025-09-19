namespace Aotenjo
{
    public class FreezeEffect : TransformMaskEffect
    {
        private readonly IceBladeArtifact artifact;

        public FreezeEffect(IceBladeArtifact artifact, Tile tile) : base(TileMask.Frozen(), artifact, tile, "effect_freeze")
        {
            this.artifact = artifact;
        }

        public override string GetSoundEffectName() => "Fracture";
    }
}