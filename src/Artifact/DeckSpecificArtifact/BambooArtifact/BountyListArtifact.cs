using System.Collections.Generic;
using Aotenjo;

public class BountyListArtifact : BambooArtifact
{
    public BountyListArtifact() : base("bounty_list", Rarity.COMMON)
    {
        SetHighlightRequirement((t, p) => ((BambooDeckPlayer)p).DetermineDora(t) > 0);
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!player.Selecting(tile)) return;
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)player;
        int count = bambooDeckPlayer.DetermineDora(tile);
        if (count > 0)
            effects.Add(new EarnMoneyEffect(count, this));
    }
}