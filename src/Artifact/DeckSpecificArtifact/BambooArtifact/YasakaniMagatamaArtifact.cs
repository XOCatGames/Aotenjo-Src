using System;
using System.Collections.Generic;
using Aotenjo;

public class YasakaniMagatamaArtifact : LevelingArtifact, IMultiplierProvider
{
    private const double MUL_PER_LEVEL = 0.1D;

    public YasakaniMagatamaArtifact() : base("yasakani_magatama", Rarity.EPIC, 0)
    {
        SetHighlightRequirement((t, p) => ((BambooDeckPlayer)p).DetermineDora(t) > 0);
    }

    public override string GetDescription(Player p, Func<string, string> localizer)
    {
        return string.Format(base.GetDescription(localizer), MUL_PER_LEVEL, GetMul(p));
    }

    public override (string, double) GetAdditionalDisplayingInfo(Player player)
    {
        return ToMulFanFormat(GetMul(player));
    }

    public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
    {
        base.AppendOnSelfEffects(player, permutation, effects);
        if (Level > 0)
        {
            effects.Add(ScoreEffect.MulFan(GetMul(player), this));
        }
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        player.PostRemoveTileEvent += DetermineDoraAndLevelUp;
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        player.PostRemoveTileEvent -= DetermineDoraAndLevelUp;
    }

    public void DetermineDoraAndLevelUp(PlayerTileEvent evt)
    {
        BambooDeckPlayer player = (BambooDeckPlayer)evt.player;
        if (player.DetermineDora(evt.tile) > 0)
        {
            Level++;
        }
    }


    public double GetMul(Player player)
    {
        return 1 + MUL_PER_LEVEL * Level;
    }
}