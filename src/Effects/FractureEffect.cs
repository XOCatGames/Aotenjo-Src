using System;
using Aotenjo;

public class FractureEffect : Effect
{
    private Artifact source;
    private Tile target;
    private string soundEffectName;

    public FractureEffect(Artifact source, Tile target, string soundEffectName = "effect_fracture_name")
    {
        this.source = source;
        this.target = target;
        this.soundEffectName = soundEffectName;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func(soundEffectName);
    }

    public override Artifact GetEffectSource()
    {
        return source;
    }

    public override void Ingest(Player player)
    {
        target.SetMask(TileMask.Fractured(), player);
    }

    public override string GetSoundEffectName()
    {
        return "Fracture";
    }
}