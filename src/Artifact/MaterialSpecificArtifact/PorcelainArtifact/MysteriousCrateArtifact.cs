using System.Collections.Generic;
using Aotenjo;

public class MysteriousCrateArtifact : Artifact
{
    private const int ADDON_FU = 50;

    public MysteriousCrateArtifact() : base("mysterious_cassette", Rarity.RARE)
    {
    }

    public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
    {
        base.AppendOnUnusedTileEffects(player, perm, tile, effects);
        if (tile.CompatWithMaterial(TileMaterial.MysteriousColorPorcelain(), player))
        {
            effects.Add(ScoreEffect.AddFu(ADDON_FU, null));
        }
    }
}