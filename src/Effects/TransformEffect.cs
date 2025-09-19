using System;
using Aotenjo;

public class TransformEffect : Effect
{
    private string name;

    private TileProperties toBecome;
    private Artifact source;
    private Tile target;

    public TransformEffect(string name, TileProperties toBecome, Artifact source, Tile target)
    {
        this.name = name;
        this.toBecome = toBecome;
        this.source = source;
        this.target = target;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func($"effect_transform_{name}_name");
    }

    public override Artifact GetEffectSource()
    {
        return source;
    }

    public override void Ingest(Player player)
    {
        target.SetProperties(toBecome, player);
    }
}