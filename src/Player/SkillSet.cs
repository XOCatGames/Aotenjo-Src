using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Aotenjo
{
    [Serializable]
    public class SkillSet
    {
        [SerializeField] private List<FixedYakuType> levelMapKey = new List<FixedYakuType>();

        [SerializeField]
        private SerializableMap<string, int> levelMap = new SerializableMap<string, int>();
        [SerializeField]
        private SerializableMap<string, int> unlockProgressMap = new SerializableMap<string, int>();

        [SerializeField] private List<int> levelMapValue = new List<int>();
        [SerializeField] private List<int> levelUnlockProgressValue = new List<int>();

        [SerializeField]
        private SerializableMap<Skill.SkillType, int> skillLevelMap = new SerializableMap<Skill.SkillType, int>();

        private Player player;

        private SkillSet()
        {
        }

        private void TrySyncLegacyValue()
        {
            if (!levelMap.IsEmpty() || !unlockProgressMap.IsEmpty() || levelMapKey.Count <= 0) return;
            
            Debug.Log("Import legacy skillset value");
            
            for (int i = 0; i < levelMapKey.Count; i++)
            {
                var key = levelMapKey[i].ToString();
                levelMap.Add(key, levelMapValue[i]);
                unlockProgressMap.Add(key, levelUnlockProgressValue[i]);
            }
        }

        public void SetPlayer(Player p)
        {
            player = p;
        }

        public static SkillSet NewSkillSet()
        {
            SkillSet Set = new();
            return Set;
        }

        public static SkillSet StandardSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(FixedYakuType.Base, 1);
            Set.SetLevel(FixedYakuType.PingHu, 1);
            Set.SetLevel(FixedYakuType.DuanYao, 1);
            Set.SetLevel(FixedYakuType.ShuangKe, 1);
            Set.SetLevel(FixedYakuType.JianKe, 1);
            Set.SetLevel(FixedYakuType.MenFengKe, 1);
            Set.SetLevel(FixedYakuType.QuanFengKe, 1);
            Set.SetLevel(FixedYakuType.QiDui, 1);
            Set.SetLevel(FixedYakuType.ShiSanYao, 1);
            Set.SetLevel(FixedYakuType.HunYiSe, 1);
            return Set;
        }

        public static SkillSet ScarletSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(FixedYakuType.Base, 1);
            Set.SetLevel(FixedYakuType.PingHu, 1);
            Set.SetLevel(FixedYakuType.DuanYao, 1);
            Set.SetLevel(FixedYakuType.ShuangKe, 1);
            Set.SetLevel(FixedYakuType.QiDui, 1);
            return Set;
        }

        public static SkillSet RainbowSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(FixedYakuType.Base, 1);
            Set.SetLevel(FixedYakuType.PingHu, 1);
            Set.SetLevel(FixedYakuType.DuanYao, 1);
            Set.SetLevel(FixedYakuType.ShuangKe, 1);
            Set.SetLevel(FixedYakuType.JianKe, 1);
            Set.SetLevel(FixedYakuType.MenFengKe, 1);
            Set.SetLevel(FixedYakuType.QuanFengKe, 1);
            Set.SetLevel(FixedYakuType.QiDui, 1);
            Set.SetLevel(FixedYakuType.ShiSanYao, 1);
            Set.SetLevel(FixedYakuType.HunYiSe, 1);
            Set.SetLevel(FixedYakuType.ZhengHua, 1);
            Set.SetLevel(FixedYakuType.YiTaiHua, 1);
            return Set;
        }

        public static SkillSet ShortenedSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(FixedYakuType.Base, 1);
            Set.SetLevel(FixedYakuType.PingHu, 1);
            Set.SetLevel(FixedYakuType.DuanYao, 1);
            Set.SetLevel(FixedYakuType.ShuangKe, 1);
            Set.SetLevel(FixedYakuType.JianKe, 1);
            Set.SetLevel(FixedYakuType.QuanFengKe, 1);
            Set.SetLevel(FixedYakuType.HunYiSe, 1);
            return Set;
        }

        public YakuPackConsumeResult Consume(YakuPack pack, Player p)
        {
            List<YakuType> availableYakus = p.deck.GetAvailableYakus().Select(y => y.GetYakuType()).ToList();
            YakuPackConsumeResult result = new YakuPackConsumeResult(pack, 1);
            for (int i = 0; i < p.GetYakuPackResultCount(); i++)
            {
                DrawYakuResult drawResult = pack.Draw(r => p.GenerateRandomInt(r, "yakupack"), availableYakus,
                    p.Level / 4);
                YakuType yaku = drawResult.yaku;
                result.yakus.Add(yaku);
            }

            return result;
        }

        public void SetLevel(YakuType yaku, int level)
        {
            TrySyncLegacyValue();
            levelMap[yaku.ToString()] = level;
        }

        public void AddLevel(YakuType yaku, int level)
        {
            TrySyncLegacyValue();

            level = player.OnPreUpgradeYaku(yaku, level).level;

            int levelBefore = GetLevel(yaku);
            SetLevel(yaku, levelBefore + level);
            int levelAfter = GetLevel(yaku);

            MessageManager.Instance.OnUpgradeYakuEvent(yaku, levelBefore, levelAfter);
        }

        public void IncreaseLevel(YakuType yaku)
        {
            AddLevel(yaku, 1);
        }

        public void DecreaseLevel(YakuType yaku)
        {
            AddLevel(yaku, -1);
        }

        public void TryUnlockYakuIfLocked(YakuType yaku, bool fullHand)
        {
            if (GetLevel(yaku) > 1 || unlockProgressMap[yaku.ToString()] == -1)
            {
                return;
            }

            unlockProgressMap[yaku.ToString()]++;
            if (fullHand || unlockProgressMap[yaku.ToString()] >= YakuTester.GetUnlockRequirement(yaku))
            {
                unlockProgressMap[yaku.ToString()] = -1;
                IncreaseLevel(yaku);
            }
        }

        public int GetUnlockProgress(YakuType yaku)
        {
            TrySyncLegacyValue();
            return unlockProgressMap[yaku.ToString()];
        }

        public int GetLevel(YakuType yaku)
        {
            return GetLevel(YakuTester.InfoMap[yaku]);
        }

        public int GetUnlockStatus(YakuType yaku)
        {
            if (GetLevel(yaku) > 0)
            {
                return 1;
            }

            return 0;
        }

        public int GetLevel(Yaku yaku)
        {
            TrySyncLegacyValue();
            return yaku.GetLevel(player);
        }

        public int GetExtraLevel(YakuType yakuType)
        {
            TrySyncLegacyValue();
            return levelMap[yakuType.ToString()];
        }

        public double CalculateFan(YakuType yaku, int blockCount, int extraLevel = 0)
        {
            blockCount = Math.Min(4, blockCount);
            if (GetLevel(yaku) == 0 && extraLevel == 0 && blockCount != 4)
            {
                return CalculateInheritedFan(yaku, blockCount);
            }

            var fan = YakuTester.InfoMap[yaku].fullFan;

            if (GetLevel(yaku) == 0 && extraLevel == 0)
            {
                fan /= 2;
            }


            double fanMultiplier = Math.Pow(1 / YakuTester.InfoMap[yaku].growthFactor, 4 - blockCount);
            fanMultiplier *= player.GetYakuMultiplier(yaku);
            return (fan + CalculateIncrementFanFromLevel(yaku, blockCount, extraLevel)) *
                   fanMultiplier + CalculateInheritedFan(yaku, blockCount);
        }

        public double CalculateInheritedFan(YakuType yaku, int blockCount)
        {
            List<YakuType> children = YakuTester.InfoMap[yaku].includedYakus.ToList();

            if (GetLevel(yaku) == 0 && blockCount < 4)
            {
                return children.Sum(child => CalculateFan(child, blockCount));
            }

            return children.Sum(child =>
                CalculateIncrementFanFromLevel(child, blockCount) + CalculateInheritedFan(child, blockCount));
        }

        public double CalculateIncrementFanFromLevel(YakuType yaku, int blockCount, int extraLevel = 0)
        {
            if (GetLevel(yaku) == 0)
            {
                return 0;
            }

            var increment = YakuTester.InfoMap[yaku].levelingFactor;
            return (GetLevel(yaku) - 1 + extraLevel) * increment *
                   Math.Pow(1 / YakuTester.InfoMap[yaku].growthFactor, 4 - blockCount);
        }

        public YakuType[] GetYakus()
        {
            TrySyncLegacyValue();
            return levelMap.GetKeys().Where(yaku => levelMap[yaku] > 0).Select(YakuType.FromString).ToArray();
        }

        public Yaku[] GetUnlockedYakus()
        {
            TrySyncLegacyValue();
            return levelMap.GetKeys()
                .Where(yaku => levelMap[yaku] > 0)
                .Select(YakuType.FromString)
                .Select(yakuType => YakuTester.InfoMap[yakuType])
                .ToArray();;
        }

        internal int ClearLevel(YakuType yakuTypeID)
        {
            int level = GetLevel(yakuTypeID);
            SetLevel(yakuTypeID, 0);
            return level;
        }

        public YakuPackConsumeResult ConsumeMultiple(IEnumerable<YakuPack> pack, Player player)
        {
            throw new Exception("Not implemented");
            //return pack.Select(p => Consume(p, player)).Aggregate(new YakuPackConsumeResult(pack.First(), 0), (a, b) => a.CombineResult(b));
        }

        internal YakuType[] GetExtraLeveledYakus()
        {
            TrySyncLegacyValue();
            return levelMap.GetKeys()
                .Select(YakuType.FromString)
                .Where(yaku => GetExtraLevel(yaku) > 0)
                .ToArray();;
        }
    }
}