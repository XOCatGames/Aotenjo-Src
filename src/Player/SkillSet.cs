using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class SkillSet
    {
        [SerializeField] private List<YakuType> levelMapKey = new List<YakuType>();
        [SerializeField] private List<int> levelMapValue = new List<int>();
        [SerializeField] private List<int> levelUnlockProgressValue = new List<int>();

        [SerializeField]
        private SerializableMap<Skill.SkillType, int> skillLevelMap = new SerializableMap<Skill.SkillType, int>();

        private Player player;

        private SkillSet()
        {
        }

        public void SetPlayer(Player p)
        {
            player = p;
        }

        public static SkillSet NewSkillSet()
        {
            SkillSet Set = new();
            foreach (YakuType yaku in Enum.GetValues(typeof(YakuType)))
            {
                Set.levelMapKey.Add(yaku);
                Set.levelMapValue.Add(0);
                Set.levelUnlockProgressValue.Add(0);
            }

            return Set;
        }

        public static SkillSet StandardSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(YakuType.Base, 1);
            Set.SetLevel(YakuType.PingHu, 1);
            Set.SetLevel(YakuType.DuanYao, 1);
            Set.SetLevel(YakuType.ShuangKe, 1);
            Set.SetLevel(YakuType.JianKe, 1);
            Set.SetLevel(YakuType.MenFengKe, 1);
            Set.SetLevel(YakuType.QuanFengKe, 1);
            Set.SetLevel(YakuType.QiDui, 1);
            Set.SetLevel(YakuType.ShiSanYao, 1);
            Set.SetLevel(YakuType.HunYiSe, 1);
            return Set;
        }

        public static SkillSet ScarletSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(YakuType.Base, 1);
            Set.SetLevel(YakuType.PingHu, 1);
            Set.SetLevel(YakuType.DuanYao, 1);
            Set.SetLevel(YakuType.ShuangKe, 1);
            Set.SetLevel(YakuType.QiDui, 1);
            return Set;
        }

        public static SkillSet RainbowSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(YakuType.Base, 1);
            Set.SetLevel(YakuType.PingHu, 1);
            Set.SetLevel(YakuType.DuanYao, 1);
            Set.SetLevel(YakuType.ShuangKe, 1);
            Set.SetLevel(YakuType.JianKe, 1);
            Set.SetLevel(YakuType.MenFengKe, 1);
            Set.SetLevel(YakuType.QuanFengKe, 1);
            Set.SetLevel(YakuType.QiDui, 1);
            Set.SetLevel(YakuType.ShiSanYao, 1);
            Set.SetLevel(YakuType.HunYiSe, 1);
            Set.SetLevel(YakuType.ZhengHua, 1);
            Set.SetLevel(YakuType.YiTaiHua, 1);
            return Set;
        }

        public static SkillSet ShortenedSkillSet()
        {
            SkillSet Set = NewSkillSet();
            Set.SetLevel(YakuType.Base, 1);
            Set.SetLevel(YakuType.PingHu, 1);
            Set.SetLevel(YakuType.DuanYao, 1);
            Set.SetLevel(YakuType.ShuangKe, 1);
            Set.SetLevel(YakuType.JianKe, 1);
            Set.SetLevel(YakuType.QuanFengKe, 1);
            Set.SetLevel(YakuType.HunYiSe, 1);
            return Set;
        }

        public YakuPackConsumeResult Consume(YakuPack pack, Player p)
        {
            List<YakuType> availableYakus = p.deck.GetAvailableYakus().Select(y => y.yakuTypeID).ToList();
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


        private int GetYakuIndex(YakuType yaku)
        {
            return levelMapKey.IndexOf(yaku);
        }

        public void SetLevel(YakuType yaku, int level)
        {
            int index = GetYakuIndex(yaku);
            levelMapValue[index] = level;
        }

        public void AddLevel(YakuType yaku, int level)
        {
            int index = GetYakuIndex(yaku);

            level = player.OnPreUpgradeYaku(yaku, level).level;

            int levelBefore = levelMapValue[index];
            levelMapValue[index] += level;
            int levelAfter = levelMapValue[index];

            EventManager.Instance.OnUpgradeYakuEvent(yaku, levelBefore, levelAfter);
        }

        public void IncreaseLevel(YakuType yaku)
        {
            AddLevel(yaku, 1);
        }

        public void DecreaseLevel(YakuType yaku)
        {
            int index = GetYakuIndex(yaku);
            levelMapValue[index]--;
        }

        public void TryUnlockYakuIfLocked(YakuType yaku, bool fullHand)
        {
            int index = GetYakuIndex(yaku);

            if (GetLevel(yaku) > 1 || levelUnlockProgressValue[index] == -1)
            {
                return;
            }

            levelUnlockProgressValue[index]++;
            if (fullHand || levelUnlockProgressValue[index] >= YakuTester.GetUnlockRequirement(yaku))
            {
                levelUnlockProgressValue[index] = -1;
                IncreaseLevel(yaku);
            }
        }

        public int GetUnlockProgress(YakuType yaku)
        {
            int index = GetYakuIndex(yaku);
            return levelUnlockProgressValue[index];
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
            return yaku.GetLevel(player);
        }

        public int GetExtraLevel(YakuType yakuType)
        {
            int index = GetYakuIndex(yakuType);
            return levelMapValue[index];
        }

        public double CalculateFan(YakuType yaku, int blockCount, int extraLevel = 0)
        {
            blockCount = Math.Min(4, blockCount);
            if (GetLevel(yaku) == 0 && extraLevel == 0 && blockCount != 4)
            {
                return CalculateInheritedFan(yaku, blockCount);
            }

            if (yaku == YakuType.QuanDa)
            {
                Debug.Log(1);
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
            return levelMapKey.Where(yaku => GetLevel(yaku) > 0).ToArray();
        }

        public Yaku[] GetUnlockedYakus()
        {
            return levelMapKey.Where(yaku => GetLevel(yaku) > 0).Select(t => YakuTester.InfoMap[t]).ToArray();
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
            return levelMapKey.Where(yaku => GetExtraLevel(yaku) > 0).ToArray();
        }
    }
}