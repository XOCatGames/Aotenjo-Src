using System;
using Aotenjo;

public class TransformMaskEffect : Effect
{
    private Artifact artifact;
    private Tile tile;
    private string display;
    private TileMask mask;

    public TransformMaskEffect(TileMask mask, Artifact artifact, Tile tile, string display)
    {
        this.artifact = artifact;
        this.tile = tile;
        this.display = display;
        this.mask = mask;
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
        tile.SetMask(mask, player);
    }
}