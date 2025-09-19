using System;
using Aotenjo;
using UnityEngine;

[Serializable]
public class SuppressEffect : Effect
{
    [SerializeField] private Tile tile;

    public SuppressEffect(Tile tile)
    {
        this.tile = tile;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func("effect_suppress");
    }

    public override Artifact GetEffectSource()
    {
        return null;
    }

    public override void Ingest(Player player)
    {
        tile?.Suppress(player);
    }

    public override string GetSoundEffectName()
    {
        return "Suppress";
    }
}