using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class YakuPack : IBook
    {
        public int id;
        public string name;

        public List<YakuType> common;
        public List<YakuType> rare;
        public List<YakuType> epic;
        public List<YakuType> legendary;
        public List<YakuType> ancient;

        public List<int> weights = new List<int>();

        public string GetNameKey()
        {
            return $"yakupack_{name}_name";
        }

        public string GetDescriptionKey()
        {
            return $"yakupack_{name}_description";
        }

        public bool ContainsYaku(Yaku yaku)
        {
            YakuType yakuTypeID = yaku.GetYakuType();
            return (common.Contains(yakuTypeID)
                    || rare.Contains(yakuTypeID)
                    || epic.Contains(yakuTypeID)
                    || legendary.Contains(yakuTypeID)
                    || ancient.Contains(yakuTypeID));
        }

        public DrawYakuResult Draw(Func<int, int> rng, List<YakuType> includedYakus, int stage, int minimumRarity = 0)
        {
            List<List<YakuType>> pool = new()
            {
                common,
                rare,
                epic,
                legendary,
                ancient
            };

            int[] weightedWeights = weights.ToArray();

            if (stage == 0)
            {
                weightedWeights[0] *= 2;
            }
            else if (stage == 1)
            {
                weightedWeights[0] *= 2;
                weightedWeights[1] *= 3;
            }
            else if (stage == 2)
            {
                weightedWeights[1] *= 3;
                weightedWeights[2] *= 2;
            }
            else if (stage == 3)
            {
                weightedWeights[2] *= 4;
                weightedWeights[3] *= 4;
            }
            else if (stage >= 4)
            {
                weightedWeights[0] /= 5;
                weightedWeights[3] *= 2;
            }

            for (int i = 0; i < minimumRarity; i++)
            {
                if (i >= weightedWeights.Length - 1 || weightedWeights[i + 1] == 0)
                {
                    break;
                }

                weightedWeights[i] = 0;
            }

            LotteryPool<List<YakuType>> rangePool = new LotteryPool<List<YakuType>>();

            for (int i = minimumRarity; i < 5; i++)
            {
                if (weightedWeights[i] > 0)
                    rangePool.Add(pool[i], weightedWeights[i]);
            }

            List<YakuType> range = rangePool.Draw(rng);

            Rarity rarity;

            if (range == common)
            {
                rarity = Rarity.COMMON;
            }
            else if (range == rare)
            {
                rarity = Rarity.RARE;
            }
            else if (range == epic)
            {
                rarity = Rarity.EPIC;
            }
            else if (range == legendary)
            {
                rarity = Rarity.LEGENDARY;
            }
            else
            {
                rarity = Rarity.ANCIENT;
            }

            if (range.Count == 0)
            {
                return new(FixedYakuType.Base, Rarity.COMMON);
            }

            List<YakuType> possibleYakus = new List<YakuType>(range);
            possibleYakus.RemoveAll(y => !includedYakus.Contains(y));

            int index = rng(possibleYakus.Count);

            return new(possibleYakus[index], rarity);
        }

        public string GetRegName()
        {
            return GetNameKey();
        }

        public List<Yaku> GetYakuPool(Player player)
        {
            List<List<YakuType>> pool = new()
            {
                common,
                rare,
                epic,
                legendary,
                ancient
            };
            return pool
                .SelectMany(l => l)
                .Where(y => player.deck.HasYakuType(y))
                .Select(t => YakuTester.InfoMap[t])
                .ToList();
        }

        public List<Yaku> DrawYakusToUpgrade(Player player)
        {
           int count = player.GetYakuPackResultCount();
           return Enumerable.Range(0, count)
               .Select(_ => Draw(player.GenerateRandomInt,
               GetYakuPool(player)
                   .Select(y => y.GetYakuType())
                   .ToList(), player.Level).yaku
               )
               .Select(type => YakuTester.InfoMap[type]).ToList();
        }
    }

    public class DrawYakuResult
    {
        public YakuType yaku;
        public Rarity rarity;

        public DrawYakuResult(YakuType yaku, Rarity rarity)
        {
            this.yaku = yaku;
            this.rarity = rarity;
        }
    }
}