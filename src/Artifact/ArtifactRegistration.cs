using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Aotenjo
{
    /// <summary>
    /// 只保存类型身份、工厂和配置；这里没有等级、状态或事件监听。
    /// 构造默认配置只从一次未绑定的临时对象提取，临时对象随即丢弃。
    /// 运行目录和展示入口每次都调用正常构造，避免复制捕获旧 this 的委托。
    /// </summary>
    public sealed class ArtifactRegistration
    {
        private readonly Func<Artifact> factory;
        private ArtifactDefinitionSnapshot definition;
        private WeakReference<Artifact> lastConstructed;
        public string Id { get; }
        public string OwnerId { get; internal set; }
        public int? LegacyNumberId => Artifacts.LEGACY_NUMBER_ID_MAP.TryGetValue(Id, out int number) ? number : (int?)null;
        public ArtifactDefinitionSnapshot Definition => definition ??= new ArtifactDefinitionSnapshot(Construct());

        internal ArtifactRegistration(string id, Func<Artifact> factory, string ownerId = "")
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("遗物注册 ID 不能为空。", nameof(id));
            Id = id;
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            OwnerId = ownerId;
        }

        private Artifact Construct()
        {
            Artifact artifact = factory();
            if (artifact == null || artifact.GetRegName() != Id)
                throw new InvalidOperationException("遗物工厂返回了错误的 ID：" + Id);
            if (lastConstructed != null && lastConstructed.TryGetTarget(out var previous) && ReferenceEquals(previous, artifact))
                throw new InvalidOperationException("遗物工厂返回了缓存对象：" + Id);
            lastConstructed = new WeakReference<Artifact>(artifact);
            return artifact;
        }

        internal Artifact CreateRuntime()
        {
            var config = Definition;
            Artifact artifact = Construct();
            if (artifact.RuntimeOwner != null || artifact.IsPreview)
                throw new InvalidOperationException("遗物工厂复用了先前对象：" + Id);
            config.ApplyTo(artifact);
            return artifact;
        }

        internal void ApplyDefinition(ArtifactDefinitionData data)
        {
            if (data.regName != Id) throw new ArgumentException("遗物配置 ID 不匹配：" + Id);
            definition = new ArtifactDefinitionSnapshot(data);
        }

        // 这些兼容方法只提供稳定的静态信息，Lua 注册返回值不能再充当运行遗物。
        public string GetRegName() => Id;
        public string GetNameID() => Id;
        public string GetNameKey() => "artifact_" + Id + "_name";
        public int GetNumberID() => LegacyNumberId ?? 0;
        public Rarity GetRarity() => Definition.Rarity;
        public UnlockRequirement GetUnlockRequirement() => Constants.DEBUG_MODE
            ? UnlockRequirement.UnlockedByDefault()
            : ArtifactUnlockRequirements.artifactRequirementMap.GetValueOrDefault(Id, UnlockRequirement.UnlockedByDefault());
        public bool IsUnlocked(PlayerStats stats) => GetUnlockRequirement().IsUnlocked(stats);
    }

    /// <summary>冻结的配置值；集合先复制再封装，不与资源、Builder 或运行对象共享可写容器。</summary>
    public sealed class ArtifactDefinitionSnapshot
    {
        public Rarity Rarity { get; }
        public IReadOnlyList<string> DeckIn { get; }
        public IReadOnlyList<string> DeckBlocked { get; }
        public IReadOnlyList<string> SetIn { get; }
        public IReadOnlyList<string> SetBlocked { get; }
        public IReadOnlyList<string> MaterialRequired { get; }
        public IReadOnlyList<ArtifactTag> Tags { get; }

        internal ArtifactDefinitionSnapshot(Artifact artifact) : this(artifact.rarity, artifact.deckIn,
            artifact.deckBlocked, artifact.setIn, artifact.setBlocked, artifact.materialRequired, artifact.tags) { }
        internal ArtifactDefinitionSnapshot(ArtifactDefinitionData data) : this(data.baseRarity, data.deckIn,
            data.deckBlocked, data.setIn, data.setBlocked, data.materialRequired, data.tags) { }

        private ArtifactDefinitionSnapshot(Rarity rarity, IEnumerable<string> deckIn, IEnumerable<string> deckBlocked,
            IEnumerable<string> setIn, IEnumerable<string> setBlocked, IEnumerable<string> materials, IEnumerable<ArtifactTag> tags)
        {
            Rarity = rarity;
            DeckIn = Copy(deckIn); DeckBlocked = Copy(deckBlocked);
            SetIn = Copy(setIn); SetBlocked = Copy(setBlocked);
            MaterialRequired = Copy(materials); Tags = Copy(tags);
        }

        private static ReadOnlyCollection<T> Copy<T>(IEnumerable<T> values) =>
            new List<T>(values ?? Array.Empty<T>()).AsReadOnly();

        internal void ApplyTo(Artifact artifact)
        {
            artifact.rarity = Rarity;
            artifact.deckIn = new List<string>(DeckIn); artifact.deckBlocked = new List<string>(DeckBlocked);
            artifact.setIn = new List<string>(SetIn); artifact.setBlocked = new List<string>(SetBlocked);
            artifact.materialRequired = new List<string>(MaterialRequired); artifact.tags = new List<ArtifactTag>(Tags);
        }
    }
}
