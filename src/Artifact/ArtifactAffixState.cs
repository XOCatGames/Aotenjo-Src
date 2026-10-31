using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 一件运行遗物独占的词条状态。外部只能读取快照；玩家入口负责容量、来源和待升级检查。
    /// 存档继续使用旧映射形状，所有映射和延迟奖励都深复制，不能成为第二份可写运行状态。
    /// </summary>
    public sealed class ArtifactAffixState
    {
        public ArtifactAffix Builtin { get; }
        public string Id => Custom?.id ?? (Builtin == ArtifactAffix.None ? "" : Builtin.ToString().ToLowerInvariant());
        public string CustomId => Custom?.id ?? "";
        public string OwnerId => Custom?.owner ?? "";
        internal CustomArtifactAffixState Custom { get; }
        public IReadOnlyDictionary<string, string> Data => new ReadOnlyDictionary<string, string>(
            Custom?.data?.GetKeys().ToDictionary(k => k, k => Custom.data.Get(k), StringComparer.Ordinal)
            ?? new Dictionary<string, string>(StringComparer.Ordinal));

        public ArtifactAffixState() : this(ArtifactAffix.None, null) { }
        internal ArtifactAffixState(ArtifactAffix builtin, CustomArtifactAffixState custom)
        {
            if (!ArtifactAffixes.IsKnown(builtin) ||
                (builtin != ArtifactAffix.None && !string.IsNullOrEmpty(custom?.id)))
                throw new InvalidDataException("遗物包含非法或互斥的词条状态。");
            Builtin = builtin;
            Custom = string.IsNullOrEmpty(custom?.id) ? null : custom.Copy();
        }

        public ArtifactAffixState Copy() => new ArtifactAffixState(Builtin, Custom);
    }
}
