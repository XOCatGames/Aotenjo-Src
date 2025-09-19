using System;
using System.Collections.Generic;
using Aotenjo;

public class PrayerBeadsArtifact : BambooArtifact
{
    private const float MUL = 0.2f;


    public PrayerBeadsArtifact() : base("prayer_beads", Rarity.EPIC)
    {
        SetHighlightRequirement((t, p) =>
            ((BambooDeckPlayer)p).DetermineDora(t) > 0 && t.CompactWithMaterial(TileMaterial.PLAIN, p));
    }

    public override string GetDescription(Func<string, string> localizer)
    {
        return string.Format(base.GetDescription(localizer), 1f + MUL);
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!tile.CompactWithMaterial(TileMaterial.PLAIN, player)) return;
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)player;
        int count = bambooDeckPlayer.DetermineDora(tile);
        if (count > 0)
            effects.Add(ScoreEffect.MulFan(1f + MUL * count, this));
    }
}