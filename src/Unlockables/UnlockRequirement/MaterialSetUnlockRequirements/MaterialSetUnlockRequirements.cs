using System.Collections.Generic;
using Aotenjo;

public class MaterialSetUnlockRequirements
{
    public static Dictionary<MaterialSet, UnlockRequirement> matSetRequirements = new()
    {
        { MaterialSet.Basic, UnlockRequirement.UnlockedByDefault() },
        { MaterialSet.Ore, UnlockRequirement.UnlockedByDefault() },
        { MaterialSet.Porcelain, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.BambooDeck) },
        { MaterialSet.Monsters, UnlockRequirement.UnlockByPlayYaku(YakuType.QuanDaiYao, 72) },
        { MaterialSet.Wood, UnlockRequirement.UnlockByLevel(2, 16) },
        { MaterialSet.Dessert, UnlockRequirement.UnlockByCustomStats("eat_food", 10) }
    };
}