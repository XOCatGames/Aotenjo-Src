using System;
using Aotenjo;

public class IncreaseTargetEffect : Effect
{
    private double ratio;
    private string name;
    private readonly Artifact artifact;

    public IncreaseTargetEffect(double v1, string v2, Artifact artifact = null)
    {
        ratio = v1;
        name = v2;
        this.artifact = artifact;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func($"effect_{name}");
    }

    public override Artifact GetEffectSource()
    {
        return artifact;
    }

    public override void Ingest(Player player)
    {
        player.levelTarget = (player.GetLevelTarget() * ratio);
    }
}