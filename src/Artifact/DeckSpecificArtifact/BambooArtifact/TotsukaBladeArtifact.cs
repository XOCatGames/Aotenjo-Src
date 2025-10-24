using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class TotsukaBladeArtifact : BambooArtifact
{
    private const int FU = 5;

    public TotsukaBladeArtifact() : base("totsuka_blade", Rarity.RARE)
    {
        SetHighlightRequirement((t, _) => t.CompatWithCategory(Tile.Category.Suo));
    }

    public override string GetDescription(Func<string, string> localizer)
    {
        return string.Format(base.GetDescription(localizer), FU);
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!player.Selecting(tile)) return;
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)player;
        int count = bambooDeckPlayer.DetermineDora(tile);
        if (count > 0)
        {
            effects.Add(new TotsukaBladeEffect(this, tile,
                player.GetHandDeckCopy().Where(t => t.CompatWithCategory(Tile.Category.Suo)).ToArray()));
        }
    }

    private class TotsukaBladeEffect : FractureEffect
    {
        private readonly Tile[] toGrow;

        public TotsukaBladeEffect(Artifact source, Tile target, Tile[] toGrow) : base(source, target)
        {
            this.toGrow = toGrow;
        }

        public override void Ingest(Player player)
        {
            base.Ingest(player);
            foreach (Tile item in toGrow)
            {
                item.addonFu += FU;
            }
        }
    }
}