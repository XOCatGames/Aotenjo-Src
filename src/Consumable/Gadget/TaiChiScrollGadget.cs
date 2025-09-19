using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class TaiChiScrollGadget : ReusableGadget
{
    public TaiChiScrollGadget() : base("tai_chi_scroll", 8, 1, 9)
    {
    }

    public override bool UseOnTile(Player player, Tile _)
    {
        if (uses <= 0) return false;
        return TaiChi(player);
    }

    private static bool TaiChi(Player player)
    {
        HashSet<Tile.Category> usedCategories = new HashSet<Tile.Category>();
        if (player.GetAccumulatedPermutation() != null)
        {
            if (player.GetAccumulatedPermutation() is ThirteenOrphansPermutation ||
                player.GetAccumulatedPermutation() is SevenPairsPermutation)
            {
                return false;
            }

            foreach (var item in player.GetAccumulatedPermutation().blocks.Select(b => b.GetCategory()))
            {
                usedCategories.Add(item);
            }
        }


        List<Tile> facades = player.GetUniqueFullDeck().Where(t => !usedCategories.Contains(t.GetCategory())).ToList();
        if (facades.Count == 0) return false;
        bool changed = false;
        foreach (Tile tile in player.GetHandDeckCopy())
        {
            if (tile is FlowerTile)
            {
                break;
            }

            if (facades.Count == 0) break;
            Tile cover = facades[player.GenerateRandomInt(facades.Count)];
            tile.AddTransform(new TileTransformTaiChi(cover.GetBaseOrder(), cover.GetBaseCategory()), player);
            changed = true;
        }

        return changed;
    }

    public override int GetMaxOnUseNum()
    {
        return 1;
    }
}