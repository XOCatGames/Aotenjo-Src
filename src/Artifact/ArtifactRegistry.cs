using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 有序定义目录。内置静态字段全部完成后才读取显式顺序，模组按加载顺序追加。
    /// 冻结后禁止替换工厂；重新加载模组必须先释放旧局，再开始新的注册会话。
    /// </summary>
    public static class ArtifactRegistry
    {
        private static List<ArtifactRegistration> entries;
        private static Dictionary<string, ArtifactRegistration> byId;
        private static bool frozen;
        public const int RuntimeApiVersion = 2;
        private static readonly List<WeakReference<Player>> runtimes = new List<WeakReference<Player>>();
        internal static void TrackRuntime(Player player) => runtimes.Add(new WeakReference<Player>(player));
        public static IReadOnlyList<ArtifactRegistration> OrderedEntries
        {
            get { EnsureInitialized(); return entries.AsReadOnly(); }
        }

        private static void EnsureInitialized()
        {
            if (entries != null) return;
            entries = new List<ArtifactRegistration>(Artifacts.BuiltinEntries);
            byId = entries.ToDictionary(e => e.Id, StringComparer.Ordinal);
        }

        public static ArtifactRegistration GetDefinition(string id) => TryGetDefinition(id, out var value)
            ? value : throw new KeyNotFoundException("未注册的遗物：" + id);
        public static bool TryGetDefinition(string id, out ArtifactRegistration definition)
        {
            EnsureInitialized();
            definition = null;
            return id != null && byId.TryGetValue(id, out definition);
        }
        public static ArtifactRegistration FindLegacyDefinition(int number) =>
            OrderedEntries.FirstOrDefault(e => e.LegacyNumberId == number);

        public static void ApplyDefinition(ArtifactDefinitionData data)
        {
            EnsureMutable();
            GetDefinition(data.regName).ApplyDefinition(data);
        }

        public static ArtifactRegistration Register(string id, Func<Artifact> factory, string ownerId = "")
        {
            EnsureMutable();
            if (byId.ContainsKey(id)) throw new InvalidOperationException("重复的遗物注册 ID：" + id);
            var entry = new ArtifactRegistration(id, factory, ownerId);
            entries.Add(entry);
            byId.Add(id, entry);
            return entry;
        }

        private static void EnsureMutable()
        {
            EnsureInitialized();
            if (frozen) throw new InvalidOperationException("遗物注册目录已冻结，不能在对局中修改。");
        }
        public static void Freeze()
        {
            EnsureInitialized();
            foreach (var entry in entries) _ = entry.Definition;
            frozen = true;
        }

        public static void BeginSession()
        {
            // 工厂会闭包引用 Lua 函数；切换会话先解绑旧目录，再丢弃工厂，最后才能释放 LuaEnv。
            Exception failure = null;
            foreach (var reference in runtimes)
                if (reference.TryGetTarget(out var player))
                    try { player.ReleaseArtifactRuntime(); }
                    catch (Exception error) { failure ??= error; }
            runtimes.Clear();
            EnsureInitialized();
            entries = new List<ArtifactRegistration>(Artifacts.BuiltinEntries);
            byId = entries.ToDictionary(e => e.Id, StringComparer.Ordinal);
            frozen = false;
            CustomArtifactAffixes.BeginLoadSession();
            if (failure != null) throw new InvalidOperationException("旧遗物会话清理失败。", failure);
        }

        internal static void RollbackRegistrations(IEnumerable<ArtifactRegistration> previous)
        {
            EnsureMutable();
            entries = previous.ToList();
            byId = entries.ToDictionary(e => e.Id, StringComparer.Ordinal);
        }

        /// <summary>展示对象不做随机预初始化，不订阅，不进入任何玩家库存。</summary>
        public static Artifact CreatePreview(string id)
        {
            var artifact = GetDefinition(id).CreateRuntime();
            artifact.IsPreview = true;
            return artifact;
        }
    }
}
