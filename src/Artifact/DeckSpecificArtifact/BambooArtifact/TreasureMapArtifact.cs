using System;
using System.Linq;
using Aotenjo;
using static Aotenjo.BambooDeckPlayer;

public class TreasureMapArtifact : BambooArtifact
{
    public TreasureMapArtifact() : base("treasure_map", Rarity.RARE)
    {
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        if (player is BambooDeckPlayer bPlayer)
        {
            bPlayer.DetermineDoraEvent += DetermineDora;
        }
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        if (player is BambooDeckPlayer bPlayer)
        {
            bPlayer.DetermineDoraEvent -= DetermineDora;
        }
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        return ((BambooDeckPlayer)player).GetIndicators().Any(i => IsCorrespondingDora(i, tile));
    }

    public void DetermineDora(PlayerDetermineDoraEvent evt)
    {
        evt.yes = evt.yes || IsCorrespondingDora(evt.indicator, evt.cand);
    }

    private static bool IsCorrespondingDora(IndicatorTile indicator, Tile cand)
    {
        if (!indicator.isRevealed) return false;
        Tile indicatorTile = indicator.tile;
        if (!indicatorTile.IsSameCategory(cand)) return false;
        if (indicatorTile.IsNumbered())
        {
            if (indicatorTile.GetOrder() > 1)
            {
                return cand.Succ(indicatorTile);
            }

            return cand.GetOrder() == 9;
        }

        if (indicatorTile.GetCategory() == Tile.Category.Feng)
        {
            if (indicatorTile.GetOrder() > 1)
            {
                return cand.GetOrder() == indicatorTile.GetOrder() - 1;
            }

            return cand.GetOrder() == 4;
        }

        if (indicatorTile.GetCategory() == Tile.Category.Jian)
        {
            if (indicatorTile.GetOrder() > 5)
            {
                return cand.GetOrder() == indicatorTile.GetOrder() - 1;
            }

            return cand.GetOrder() == 7;
        }

        throw new ArgumentException("ILLEAGAL ARGUMENT PASSED: " + cand);
    }
}