using System;
using System.Collections.Generic;
using Aotenjo;

public class BoneDragonBallArtifact : BambooArtifact
{
    private const float MUL = 1.5f;

    public BoneDragonBallArtifact() : base("bone_dragon_ball", Rarity.EPIC)
    {
        SetHighlightRequirement((t, p) => ((BambooDeckPlayer)p).DetermineDora(t) > 0 && t.properties.IsDebuffed());
    }

    public override string GetDescription(Func<string, string> localizer)
    {
        return string.Format(base.GetDescription(localizer), MUL);
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);

        if (!tile.properties.IsDebuffed()) return;

        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)player;
        int count = bambooDeckPlayer.DetermineDora(tile);
        if (count > 0)
            effects.Add(ScoreEffect.MulFan(MUL, this));
    }
}