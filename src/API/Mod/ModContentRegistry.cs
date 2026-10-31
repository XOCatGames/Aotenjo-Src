using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public static class ModContentRegistry
    {
        private static readonly Dictionary<string, string> owners = new Dictionary<string, string>(StringComparer.Ordinal);
        private static readonly HashSet<string> loaded = new HashSet<string>(StringComparer.Ordinal);
        public static bool Initialized { get; private set; }

        public static void BeginSession()
        {
            ArtifactRegistry.BeginSession();
            owners.Clear();
            loaded.Clear();
            foreach (string key in Capture().Keys) owners[key] = "";
            Initialized = true;
        }

        public static Dictionary<string, object> Capture()
        {
            var result = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var artifact in ArtifactRegistry.OrderedEntries) result[Key("artifact", artifact.GetRegName())] = artifact;
            foreach (var affix in CustomArtifactAffixes.Definitions) result[Key("affix", affix.Id)] = affix;
            foreach (var provider in TileMaterial.MaterialProviders)
                result[Key("material", provider().GetRegName())] = provider;
            foreach (var deck in MahjongDeck.decks) result[Key("deck", deck.regName)] = deck;
            foreach (var set in MaterialSet.MaterialSets) result[Key("set", set.regName)] = set;
            if (YakuTester.InfoMap != null)
                foreach (var pair in YakuTester.InfoMap) result[Key("yaku", pair.Key.ToString())] = pair.Value;
            return result;
        }

        public static void CompleteMod(string modId, Dictionary<string, object> before)
        {
            var current = Capture();
            foreach (string removed in owners.Keys.Where(k => !current.ContainsKey(k)).ToArray()) owners.Remove(removed);
            foreach (var pair in current)
                if (!before.TryGetValue(pair.Key, out var previous) || !ReferenceEquals(previous, pair.Value)) owners[pair.Key] = modId;
            foreach (var entry in ArtifactRegistry.OrderedEntries)
                entry.OwnerId = OwnerOf("artifact", entry.Id);
            loaded.Add(modId);
        }

        public static void RollbackMod(Dictionary<string, object> before)
        {
            ArtifactRegistry.RollbackRegistrations(before.Values.OfType<ArtifactRegistration>());
            CustomArtifactAffixes.Rollback(before.Values.OfType<CustomArtifactAffixDefinition>());
        }

        public static string OwnerOf(string kind, string id) => owners.TryGetValue(Key(kind, id), out var owner) ? owner : "";
        public static bool IsModLoaded(string id) => string.IsNullOrEmpty(id) || loaded.Contains(id);

        public static bool IsAvailable(string kind, string id, string expectedOwner = null)
        {
            if (string.IsNullOrEmpty(id)) return false;
            if (Initialized)
            {
                if (!owners.TryGetValue(Key(kind, id), out var owner) || !IsModLoaded(owner)) return false;
                if (expectedOwner != null && !string.Equals(owner, expectedOwner, StringComparison.Ordinal)) return false;
                return true;
            }
            // Also check the live registry: a later mod may have removed content.
            switch (kind)
            {
                case "artifact": return ArtifactRegistry.OrderedEntries.Any(a => a.GetRegName() == id);
                case "affix": return CustomArtifactAffixes.Find(id) != null;
                case "material": return TileMaterial.MaterialProviders.Any(p => p().GetRegName() == id);
                case "deck": return MahjongDeck.decks.Any(d => d.regName == id);
                case "set": return MaterialSet.MaterialSets.Any(s => s.regName == id);
                case "yaku": return YakuTester.InfoMap?.ContainsKey(YakuType.FromString(id)) == true;
                default: return false;
            }
        }

        private static string Key(string kind, string id) => kind + "/" + id;
    }

    [Serializable]
    public sealed class RunModInfo
    {
        public string id;
        public string version;
    }

    [Serializable]
    public sealed class RunEnvironment
    {
        public string runId;
        public string gameVersion;
        public bool known;
        public List<RunModInfo> mods = new List<RunModInfo>();

        public static RunEnvironment Capture() => new RunEnvironment
        {
            runId = Guid.NewGuid().ToString("N"), gameVersion = Utils.GetConfigValue("version"), known = true,
            mods = (ModManager.Instance?.LoadedMods ?? Array.Empty<Mod>())
                .Select(m => new RunModInfo { id = m.modID, version = m.version }).ToList()
        };

        public static RunEnvironment Legacy() => new RunEnvironment { runId = Guid.NewGuid().ToString("N") };
    }
}
