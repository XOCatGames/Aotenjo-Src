using System;
using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 构建并注册自定义番种
    /// </summary>
    public class CustomYakuBuilder
    {
        public static YakuType RegisterCustomYaku(string id, int baseFan, double growthFactor, double levelingFan,
            Func<Permutation, Player, bool> predicate, YakuType[] includedYakus, string[] groups, int[] yakuCategories,
            Rarity rarity, string exampleTiles)
        {
            var yakuType = new YakuType(id);
            var customYaku = new Yaku(yakuType, baseFan, growthFactor, levelingFan, includedYakus, groups, rarity, exampleTiles, yakuCategories);
            YakuTester.InfoMap.Add(yakuType, customYaku);
            YakuTester.YAKUS_PREDICATE_MAP.Add(yakuType, predicate);
            foreach (var i in yakuCategories)
            {
                YakuPack pack = RegisterManager.Instance.YakuPacks[i];
                switch (rarity)
                {
                    case Rarity.COMMON:
                        pack.common.Add(yakuType);
                        break;
                    case Rarity.RARE:
                        pack.rare.Add(yakuType);
                        break;
                    case Rarity.EPIC:
                        pack.epic.Add(yakuType);
                        break;
                    case Rarity.LEGENDARY:
                        pack.legendary.Add(yakuType);
                        break;
                    case Rarity.ANCIENT:
                        pack.ancient.Add(yakuType);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null);
                }
            }
            return yakuType;
        }
    }
}
