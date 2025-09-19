using Aotenjo;

public class TransformMaterialEffect : TextEffect
{
    private Tile tile;
    private TileMaterial mat;

    public TransformMaterialEffect(TileMaterial mat, Artifact artifact, Tile tile, string display,
        string soundName = "AddFu") : base(display, artifact, soundName)
    {
        this.tile = tile;
        this.mat = mat;
    }

    public override void Ingest(Player player)
    {
        tile.SetMaterial(mat, player);
    }
}