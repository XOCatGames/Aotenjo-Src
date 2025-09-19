using System;

namespace Aotenjo
{
    public class YakuLevelingResult : IComparable<YakuLevelingResult>
    {
        public readonly Yaku yakuBefore;
        public readonly Yaku yakuAfter;
        public readonly int levelBefore;
        public readonly int levelAfter;

        public YakuLevelingResult(Yaku yakuBefore, Yaku yakuAfter, int levelBefore, int levelAfter)
        {
            this.yakuBefore = yakuBefore;
            this.yakuAfter = yakuAfter;
            this.levelBefore = levelBefore;
            this.levelAfter = levelAfter;
        }

        public YakuLevelingResult(YakuType yakuBefore, YakuType yakuAfter, int levelBefore, int levelAfter)
        {
            Yaku yaku1 = YakuTester.InfoMap[yakuBefore];
            Yaku yaku2 = YakuTester.InfoMap[yakuAfter];
            this.yakuBefore = yaku1;
            this.yakuAfter = yaku2;
            this.levelBefore = levelBefore;
            this.levelAfter = levelAfter;
        }

        public int CompareTo(YakuLevelingResult other)
        {
            return yakuAfter.CompareTo(other.yakuAfter) == 0
                ? levelAfter.CompareTo(other.levelAfter)
                : yakuAfter.CompareTo(other.yakuAfter);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1} -> {2}:{3}", yakuBefore, levelBefore, yakuAfter, levelAfter);
        }
    }
}