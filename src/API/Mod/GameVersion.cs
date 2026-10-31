using System;
using System.Text.RegularExpressions;

namespace Aotenjo
{
    // The game's four-component versions are not SemVer. Keep this parser shared
    // by save validation and mod manifests so PRE10 never sorts before PRE9.
    // CI build variants share compatibility with their underlying game version.
    public readonly struct GameVersion : IComparable<GameVersion>
    {
        private static readonly Regex Pattern = new Regex(
            @"^(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?(?:-(PRE(\d+)|EA))?(?:-(?:debug|console-only))?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Version core;
        private readonly int? preview;

        private GameVersion(Version core, int? preview) { this.core = core; this.preview = preview; }

        public static bool TryParse(string text, out GameVersion result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(text)) return false;
            var match = Pattern.Match(text.Trim());
            if (!match.Success) return false;
            var parts = new int[4];
            for (int i = 0; i < parts.Length; i++)
                if (match.Groups[i + 1].Success && !int.TryParse(match.Groups[i + 1].Value, out parts[i])) return false;
            int? preview = null;
            if (match.Groups[6].Success)
            {
                if (!int.TryParse(match.Groups[6].Value, out int value)) return false;
                preview = value;
            }
            result = new GameVersion(new Version(parts[0], parts[1], parts[2], parts[3]), preview);
            return true;
        }

        public int CompareTo(GameVersion other)
        {
            int comparison = core.CompareTo(other.core);
            if (comparison != 0) return comparison;
            if (preview.HasValue != other.preview.HasValue) return preview.HasValue ? -1 : 1;
            return preview.HasValue ? preview.Value.CompareTo(other.preview.Value) : 0;
        }
    }

    [Serializable]
    public sealed class SupportedGameVersionRange
    {
        public string minInclusive;
        public string maxInclusive;

        public bool TryContains(GameVersion version, out bool contains)
        {
            contains = false;
            if (!GameVersion.TryParse(minInclusive, out var min) ||
                !GameVersion.TryParse(maxInclusive, out var max) || min.CompareTo(max) > 0) return false;
            contains = min.CompareTo(version) <= 0 && max.CompareTo(version) >= 0;
            return true;
        }

        public override string ToString() => minInclusive == maxInclusive
            ? minInclusive : minInclusive + " – " + maxInclusive;
    }
}
