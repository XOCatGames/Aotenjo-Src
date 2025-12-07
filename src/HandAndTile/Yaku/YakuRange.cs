using System.Collections.Generic;

namespace Aotenjo
{
    public static class YakuRange
    {
        public static Dictionary<string, string> RangeExtensions = new Dictionary<string, string>()
        {
            {"extended", "standard"}
        };

        public static bool IsInRange(this Yaku yaku, string range)
        {
            if (yaku.AvailableIn(range)) return true;
            return RangeExtensions.TryGetValue(range, out string extension) && yaku.IsInRange(extension);
        }
    }
}