using System;
using Aotenjo;

public class CorruptEffect : Effect
{
    private Tile tile;

    public CorruptEffect(Tile tile)
    {
        this.tile = tile;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func("effect_corrupt");
    }

    public override Artifact GetEffectSource()
    {
        return null;
    }

    public override void Ingest(Player player)
    {
        tile.SetMask(TileMask.Corrupted(tile), player);
    }

    public override string GetSoundEffectName()
    {
        return "Corrupt";
    }
}