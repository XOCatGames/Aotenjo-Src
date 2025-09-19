using System.Collections.Generic;
using System.Text;

namespace Aotenjo
{
    public class YakuPackConsumeResult
    {
        public YakuPack yakuPack;
        public int count;

        public List<YakuLevelingResult> results = new();

        public int intensity = 1;

        public List<YakuType> yakus = new();

        public YakuPackConsumeResult(YakuPack yakuPack, int count)
        {
            this.yakuPack = yakuPack;
            this.count = count;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < results.Count; i++)
            {
                sb.Append(results[i]);
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public void Sort()
        {
            results.Sort();
        }
    }
}