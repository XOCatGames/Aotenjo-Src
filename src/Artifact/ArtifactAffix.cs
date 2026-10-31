using System;
using System.Linq;

namespace Aotenjo
{
    // 枚举值已经写入存档，不能重排；词条归属运行实例或尚未准入的商品快照。
    public enum ArtifactAffix
    {
        None = 0,
        Rooted = 1,
        Miniature = 2,
        Gilded = 3,
        Lucky = 4,
        Unstable = 5
    }

    public static class ArtifactAffixes
    {
        public static bool IsKnown(ArtifactAffix affix) =>
            affix >= ArtifactAffix.None && affix <= ArtifactAffix.Unstable;

        public static string Name(ArtifactAffix affix, Func<string, string> loc) =>
            affix == ArtifactAffix.None || !IsKnown(affix) ? "" : loc($"artifact_affix_{affix.ToString().ToLowerInvariant()}_name");

        public static string Description(ArtifactAffix affix, Func<string, string> loc) =>
            affix == ArtifactAffix.None || !IsKnown(affix) ? "" : loc($"artifact_affix_{affix.ToString().ToLowerInvariant()}_description");

        public static string DecorateName(string name, ArtifactAffix affix, Func<string, string> loc) =>
            affix == ArtifactAffix.None || !IsKnown(affix) ? name : $"{Name(affix, loc)} · {name}";

        public static string DecorateDescription(string description, ArtifactAffix affix, Func<string, string> loc) =>
            affix == ArtifactAffix.None || !IsKnown(affix) ? description :
                $"{description}\n\n<b><style=\"yellow\">{Name(affix, loc)}</style></b>\n{Description(affix, loc)}";

        public static string DecorateName(string name, Player player, Artifact artifact, Func<string, string> loc) =>
            string.IsNullOrEmpty(player?.GetArtifactAffixId(artifact)) ? name :
                $"{player.GetArtifactAffixName(artifact, loc)} · {name}";

        public static string DecorateDescription(string description, Player player, Artifact artifact, Func<string, string> loc) =>
            string.IsNullOrEmpty(player?.GetArtifactAffixId(artifact)) ? description :
                $"{description}\n\n<b><style=\"yellow\">{player.GetArtifactAffixName(artifact, loc)}</style></b>\n{player.GetArtifactAffixDescription(artifact, loc)}";

        public static bool TryParse(string value, out ArtifactAffix affix)
        {
            // 轻盈沿用 Miniature 的存档编号和模组 ID；新增名称入口，旧命令继续可用。
            if (string.Equals(value, "lightweight", StringComparison.OrdinalIgnoreCase))
            {
                affix = ArtifactAffix.Miniature;
                return true;
            }
            switch (value)
            {
                case "无": case "清除": affix = ArtifactAffix.None; return true;
                case "筑根": affix = ArtifactAffix.Rooted; return true;
                case "轻盈": case "輕盈": case "微缩": affix = ArtifactAffix.Miniature; return true;
                case "金纹": affix = ArtifactAffix.Gilded; return true;
                case "幸运": affix = ArtifactAffix.Lucky; return true;
                case "不稳": affix = ArtifactAffix.Unstable; return true;
                default: return Enum.TryParse(value, true, out affix) && IsKnown(affix);
            }
        }

        // 保留包括稀有小权重在内的比例；零幸运必须维持原随机范围和序列。
        public static int[] AdjustRarityWeights(int[] weights, int luckyCount)
        {
            if (luckyCount <= 0) return weights.ToArray();
            double factor = 1 + 0.5 * luckyCount;
            double[] adjusted = weights.Select((w, r) => Math.Max(0, w) * Math.Pow(factor, r)).ToArray();
            double total = adjusted.Sum();
            double scale = total > 0 ? Math.Min(1000, (int.MaxValue / 2d) / total) : 1;
            return adjusted.Select(w => w <= 0 ? 0 : Math.Max(1, (int)Math.Round(w * scale))).ToArray();
        }
    }
}
