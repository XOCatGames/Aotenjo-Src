using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Aotenjo
{
    public static class ModCatalog
    {
        public static Mod Discover(string directory, bool workshop = false, string itemUrl = "")
        {
            Mod mod;
            try
            {
                mod = JsonConvert.DeserializeObject<Mod>(File.ReadAllText(Path.Combine(directory, "modinfo.json")));
                if (mod == null || string.IsNullOrWhiteSpace(mod.modID) ||
                    !Regex.IsMatch(mod.modID, @"^[A-Za-z0-9_.:-]+$") || string.IsNullOrWhiteSpace(mod.name))
                    throw new InvalidDataException("A manifest requires a name and a stable modID.");
            }
            catch (Exception error)
            {
                mod = new Mod { name = Path.GetFileName(directory), blockReasonKey = "ui_mod_invalid_manifest", diagnostic = error.Message };
            }
            mod.modDir = directory;
            mod.isFromWorkshop = workshop;
            mod.itemUrl = itemUrl;
            return mod;
        }

        // This pass must finish for the entire catalog before any mod can register
        // content. In particular, duplicates cannot be detected after loading one.
        public static void Evaluate(IReadOnlyList<Mod> mods, ModSettingsStore settings, string currentVersion)
        {
            bool validCurrent = GameVersion.TryParse(currentVersion, out var current);
            foreach (var mod in mods)
            {
                mod.requestedEnabled = settings.IsEnabled(mod.modID);
                if (!string.IsNullOrEmpty(mod.blockReasonKey)) continue;
                if (mod.supportedGameVersions == null || mod.supportedGameVersions.Count == 0)
                {
                    mod.blockReasonKey = "ui_mod_version_undeclared";
                    continue;
                }
                bool supported = false;
                foreach (var range in mod.supportedGameVersions)
                {
                    if (!validCurrent || range == null || !range.TryContains(current, out bool contains))
                    {
                        mod.blockReasonKey = "ui_mod_invalid_version";
                        break;
                    }
                    supported |= contains;
                }
                if (mod.CanEnable && !supported) mod.blockReasonKey = "ui_mod_incompatible";
            }
            foreach (var group in mods.Where(m => !string.IsNullOrEmpty(m.modID)).GroupBy(m => m.modID, StringComparer.Ordinal))
                if (group.Count() > 1)
                    foreach (var mod in group) mod.blockReasonKey = "ui_mod_duplicate_id";
        }
    }

    public sealed class ModSettingsStore
    {
        private sealed class Settings
        {
            public int formatVersion = 1;
            public List<string> disabledModIds = new List<string>();
        }

        private readonly string path;
        private Settings settings = new Settings();
        public bool IsReadable { get; private set; } = true;

        public ModSettingsStore(string path)
        {
            this.path = path;
            if (!File.Exists(path)) return;
            try
            {
                var parsed = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
                if (parsed == null || parsed.formatVersion != 1 || parsed.disabledModIds == null)
                    throw new InvalidDataException("Unsupported mod settings.");
                settings = parsed;
            }
            catch (Exception) { IsReadable = false; }
        }

        public bool IsEnabled(string id) => IsReadable && !string.IsNullOrEmpty(id) && !settings.disabledModIds.Contains(id);

        public void SetEnabled(string id, bool enabled)
        {
            if (!IsReadable || string.IsNullOrEmpty(id)) throw new InvalidDataException("Mod settings cannot be overwritten.");
            var updated = new Settings { disabledModIds = new List<string>(settings.disabledModIds) };
            updated.disabledModIds.RemoveAll(value => value == id);
            if (!enabled) updated.disabledModIds.Add(id);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temporary = path + ".tmp-" + Guid.NewGuid().ToString("N");
            try
            {
                File.WriteAllText(temporary, JsonConvert.SerializeObject(updated, Formatting.Indented));
                if (File.Exists(path)) File.Replace(temporary, path, path + ".backup");
                else File.Move(temporary, path);
                settings = updated;
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
