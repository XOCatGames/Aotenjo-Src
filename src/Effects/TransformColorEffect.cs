using System;
using Aotenjo;

public class TransformColorEffect : Effect
{
    private Artifact artifact;
    private Tile tile;
    private string display;
    private TileFont font;

    public TransformColorEffect(TileFont font, Artifact artifact, Tile tile, string display)
    {
        this.artifact = artifact;
        this.tile = tile;
        this.display = display;
        this.font = font;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func(display);
    }

    public override Artifact GetEffectSource()
    {
        return artifact;
    }

    public override void Ingest(Player player)
    {
        tile.SetFont(font, player);
    }
}