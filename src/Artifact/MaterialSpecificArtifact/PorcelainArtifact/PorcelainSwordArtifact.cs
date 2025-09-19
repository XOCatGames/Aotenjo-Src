using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class PorcelainSwordArtifact : Artifact
{
    public PorcelainSwordArtifact() : base("porcelain_sword", Rarity.RARE)
    {
        SetHighlightRequirement((tile, _) => tile.properties.material is TileMaterialPorcelain);
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (tile.properties.material is not TileMaterialPorcelain) return;
        effects.Add(new TextEffect("effect_porcelain_sword"));
        effects.Add(new TileUnusedScoringEffectAppendEffect(player, tile, permutation, player.playHandEffectStack));
    }
}