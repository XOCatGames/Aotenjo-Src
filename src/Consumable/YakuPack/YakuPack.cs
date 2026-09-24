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
            return TryDraw(rng, includedYakus, stage, out DrawYakuResult result, minimumRarity)
                ? result
                : new DrawYakuResult(FixedYakuType.Base, Rarity.COMMON);
        }

        public bool TryDraw(Func<int, int> rng, List<YakuType> includedYakus, int stage,
            out DrawYakuResult result, int minimumRarity = 0)
        {
            result = null;
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

            // Filter before choosing a rarity: a weighted but unavailable tier is not a valid draw.
            LotteryPool<int> rangePool = new LotteryPool<int>();
            for (int i = Math.Max(0, minimumRarity); i < pool.Count; i++)
            {
                pool[i] = pool[i].Where(includedYakus.Contains).ToList();
                if (weightedWeights[i] > 0 && pool[i].Count > 0)
                    rangePool.Add(i, weightedWeights[i]);
            }

            if (rangePool.IsEmpty()) return false;
            int rarityIndex = rangePool.Draw(rng);
            List<YakuType> candidates = pool[rarityIndex];
            result = new DrawYakuResult(candidates[rng(candidates.Count)], (Rarity)rarityIndex);
            return true;
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
