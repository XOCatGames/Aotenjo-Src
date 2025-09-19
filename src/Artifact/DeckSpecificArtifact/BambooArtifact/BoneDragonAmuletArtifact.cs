using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class BoneDragonAmuletArtifact : BambooArtifact
{
    public BoneDragonAmuletArtifact() : base("bone_dragon_amulet", Rarity.RARE)
    {
        SetHighlightRequirement((t, p) => ((BambooDeckPlayer)p).DetermineDora(t) > 0);
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!player.Selecting(tile)) return;
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)player;

        if (bambooDeckPlayer.DetermineDora(tile) == 0)
        {
            return;
        }

        List<Tile> indicators = bambooDeckPlayer.GetRevealedIndicator()
            .Where(i => bambooDeckPlayer.DetermineDora(tile, i)).Select(i => i.tile).ToList();

        TileProperties newProperties = new(tile.properties);

        bool changed = false;

        foreach (Tile indicator in indicators)
        {
            if (indicator.properties.material.GetRegName() != TileMaterial.PLAIN.GetRegName() &&
                player.GenerateRandomInt(2) == 0)
            {
                newProperties.material = indicator.properties.material.Copy();
                changed = true;
            }

            if (indicator.properties.mask.GetRegName() != TileMask.NONE.GetRegName() &&
                player.GenerateRandomInt(2) == 0)
            {
                newProperties.mask = indicator.properties.mask.Copy();
                changed = true;
            }

            if (indicator.properties.font.GetRegName() != TileFont.PLAIN.GetRegName() &&
                player.GenerateRandomInt(2) == 0)
            {
                newProperties.font = indicator.properties.font.Copy();
                changed = true;
            }
        }

        if (changed)
        {
            effects.Add(new TransformEffect("bone_dragon", newProperties, this, tile));
        }
    }
}