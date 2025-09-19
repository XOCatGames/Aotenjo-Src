using System;
using Aotenjo;
using UnityEngine;

[Serializable]
public class ChangeSuitEffect : Effect
{
    [SerializeField] private Tile tile;
    private readonly Tile.Category category;

    public ChangeSuitEffect(Tile tile, Tile.Category category)
    {
        this.tile = tile;
        this.category = category;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func("effect_change_suit");
    }

    public override Artifact GetEffectSource()
    {
        return null;
    }

    public override void Ingest(Player player)
    {
        if (!tile.IsNumbered()) return;
        tile.GetCategory();

        tile.AddTransform(new TileTransformTrivial(category, tile.GetOrder()), player);
    }
}