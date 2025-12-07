using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 构建并注册自定义番种
    /// </summary>
    public class CustomYakuBuilder
    {
        public static YakuType RegisterCustomYaku(string id, int baseFan, double growthFactor, double levelingFan,
            Func<Permutation, Player, bool> predicate, string[] includedYakus, string[] groups, int[] yakuCategories,
            Rarity rarity, string exampleTiles)
        {
            var yakuType = new YakuType(id);
            
            YakuType[] includedYakuTypes = includedYakus.Select(y => Enum.TryParse<FixedYakuType>(y, true, out var res) ? res : YakuType.FromString(y)).ToArray();
            var customYaku = new Yaku(
                yakuType, 
                baseFan, 
                growthFactor, 
                levelingFan, 
                includedYakuTypes, 
                groups, 
                rarity, 
                exampleTiles, 
                yakuCategories
                );
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

        public static void AddInheritanceRelation(string nativeYakuTypeID, YakuType[] inheritedYakuTypes)
        {
            YakuType nativeYakuType = Enum.TryParse<FixedYakuType>(nativeYakuTypeID, true, out var res) ? res : YakuType.FromString(nativeYakuTypeID);
            Yaku yaku = YakuTester.InfoMap[nativeYakuType];
            yaku.includedYakus = yaku.includedYakus.Concat(inheritedYakuTypes).Distinct().ToArray();
        }
    }
}
