using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Aotenjo
{
    // 定义只保存静态配置与回调；成长数据由遗物实例独占，不能跨遗物或跨局共享。
    internal sealed class CustomArtifactAffixDefinition
    {
        internal string Id, NameKey, DescriptionKey;
        internal Func<Player, Artifact, bool> CanApply;
        internal Func<Player, Func<string, string>, Artifact, string> Description;
        internal Action<Player, Permutation, List<Effect>, Artifact> OnSelf;
        internal Action<Player, Permutation, Tile, List<Effect>, Artifact> OnTile;
        internal Action<Player, Permutation, Block, List<Effect>, Artifact> OnBlock;
        internal Action<Player, Permutation, List<IAnimationEffect>, Artifact> OnRoundEnd;
        internal CustomArtifactAffixDefinition Copy() => (CustomArtifactAffixDefinition)MemberwiseClone();
    }

    internal static class CustomArtifactAffixes
    {
        private static readonly Dictionary<string, CustomArtifactAffixDefinition> definitions = new(StringComparer.Ordinal);
        internal static IEnumerable<CustomArtifactAffixDefinition> Definitions => definitions.Values;
        internal static CustomArtifactAffixDefinition Find(string id) =>
            id != null && definitions.TryGetValue(id, out var definition) ? definition : null;

        // 实局、赋予预检和读档共用来源校验；每次解析当前注册表，避免模组回滚后仍调用旧定义。
        internal static CustomArtifactAffixDefinition Resolve(CustomArtifactAffixState state) =>
            state != null && ModContentRegistry.IsAvailable("affix", state.id, state.owner) ? Find(state.id) : null;

        internal static bool Register(CustomArtifactAffixDefinition definition)
        {
            if (string.IsNullOrEmpty(definition.Id) || definition.Id.Length > 128 ||
                !Regex.IsMatch(definition.Id, @"\A[a-z][a-z0-9_.-]*:[a-z][a-z0-9_.-]*\z") ||
                string.IsNullOrWhiteSpace(definition.NameKey) ||
                (string.IsNullOrWhiteSpace(definition.DescriptionKey) && definition.Description == null) ||
                definitions.ContainsKey(definition.Id)) return false;
            definitions.Add(definition.Id, definition);
            return true;
        }

        internal static void Rollback(IEnumerable<CustomArtifactAffixDefinition> previous)
        {
            definitions.Clear();
            foreach (var definition in previous) definitions.Add(definition.Id, definition);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void BeginLoadSession() => definitions.Clear();
    }

    [Serializable]
    internal sealed class CustomArtifactAffixState
    {
        public string id;
        public string owner;
        public SerializableMap<string, string> data = new();

        // 延迟升级保存独立快照，避免到账前的成长或重入回调修改同一份数据。
        internal CustomArtifactAffixState Copy() => new() { id = id, owner = owner, data = data?.Clone() ?? new() };
    }
}
