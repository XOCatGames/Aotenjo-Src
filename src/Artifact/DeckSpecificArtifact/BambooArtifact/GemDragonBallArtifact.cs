using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class GemDragonBallArtifact : BambooArtifact
{
    private const float FU = 20f;

    public GemDragonBallArtifact() : base("gem_dragon_ball", Rarity.COMMON)
    {
        SetHighlightRequirement((t, p) => ((BambooDeckPlayer)p).DetermineDora(t) > 0 &&
                                           MaterialSet.Ore.GetMaterials().Any(m =>
                                               m.GetRegName() == t.properties.material.GetRegName()));
    }

    public override string GetDescription(Func<string, string> localizer)
    {
        return string.Format(base.GetDescription(localizer), FU);
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!MaterialSet.Ore.GetMaterials().Any(m => m.GetRegName() == tile.properties.material.GetRegName())) return;
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)player;
        int count = bambooDeckPlayer.DetermineDora(tile);
        if (count > 0)
            effects.Add(ScoreEffect.AddFu(FU * count, this));
    }
}