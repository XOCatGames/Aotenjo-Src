using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public partial class Player
    {
        // 保留 #343 的存档字段名；它只是读写适配快照，运行规则全部访问实例。
        [SerializeField] private SerializableMap<string, CustomArtifactAffixState> customArtifactAffixes = new();

        public string GetArtifactAffixId(Artifact artifact) => GetHeldArtifactAffix(artifact)?.Id ?? "";

        public bool SetCustomArtifactAffix(Artifact artifact, string id) =>
            !string.IsNullOrEmpty(id) && SetArtifactAffix(artifact, new ArtifactAffixState(ArtifactAffix.None, new CustomArtifactAffixState
            {
                id = id, owner = ModContentRegistry.OwnerOf("affix", id)
            }));

        public bool ClearArtifactAffix(Artifact artifact) => SetArtifactAffix(artifact, ArtifactAffix.None);

        public string GetArtifactAffixData(Artifact artifact, string key, string fallback = "") =>
            GetHeldArtifactAffix(artifact)?.GetData(key, fallback) ?? fallback;

        public bool SetArtifactAffixData(Artifact artifact, string key, string value)
        {
            if (IsReservedForRecipe(artifact) || HasPendingArtifactReplacement(artifact) ||
                GetHeldArtifactAffix(artifact)?.SetData(key, value) != true) return false;
            ClearCachedSelectedPerm();
            return true;
        }

        public string GetArtifactAffixName(Artifact artifact, Func<string, string> loc) =>
            GetHeldArtifactAffix(artifact)?.GetName(loc) ?? "";

        public string GetArtifactAffixDescription(Artifact artifact, Func<string, string> loc) =>
            GetHeldArtifactAffix(artifact)?.GetDescription(loc) ?? "";

        private void CaptureArtifactAffixSnapshots()
        {
            var builtins = new SerializableMap<string, ArtifactAffix>();
            var customs = new SerializableMap<string, CustomArtifactAffixState>();
            foreach (var artifact in heldArtifacts)
            {
                var state = artifact.RuntimeAffix;
                if (state.Builtin != ArtifactAffix.None) builtins.Add(artifact.InstanceId.ToString(), state.Builtin);
                if (state.Custom != null) customs.Add(artifact.InstanceId.ToString(), state.Custom.Copy());
            }
            heldArtifactAffixes = builtins;
            customArtifactAffixes = customs;
        }

        private void RestoreArtifactAffixSnapshots()
        {
            // 有实例目录的新档按编号还原；旧档按类型复制到每件宿主，词条成长不能共享引用。
            string Key(Artifact artifact) => artifactInstanceDefinitions == null ? artifact.GetRegName() : artifact.InstanceId.ToString();
            var builtinKeys = heldArtifactAffixes?.GetKeys() ?? Array.Empty<string>();
            var customKeys = customArtifactAffixes?.GetKeys() ?? Array.Empty<string>();
            foreach (var keys in new[] { builtinKeys, customKeys })
                if (keys.Any(string.IsNullOrEmpty) || keys.Distinct().Count() != keys.Length ||
                    keys.Any(id => !heldArtifacts.Any(a => Key(a) == id)))
                    throw new System.IO.InvalidDataException("词条存档指向非持有对象或包含重复键。");
            foreach (string id in builtinKeys.Concat(customKeys).Distinct())
            {
                var custom = customArtifactAffixes?.Get(id);
                if (customKeys.Contains(id) && CustomArtifactAffixes.Resolve(custom) == null)
                    throw new System.IO.InvalidDataException("自定义词条来源缺失或不匹配：" + id);
                foreach (var artifact in heldArtifacts.Where(a => Key(a) == id))
                    artifact.RuntimeAffix = new ArtifactAffixState(heldArtifactAffixes?.Get(id) ?? ArtifactAffix.None, custom?.Copy());
            }
        }

        public IEnumerable<string> GetMissingSavedArtifactAffixIds()
        {
            // 检查原始存档数据，不先按当前注册表过滤；队列中的升级快照也不能被忽略。
            var states = (customArtifactAffixes?.GetKeys() ?? Array.Empty<string>()).Select(id => customArtifactAffixes.Get(id))
                .Concat((inventoryRewards ?? new()).Where(r => !string.IsNullOrEmpty(r.customArtifactAffix?.id))
                    .Select(r => r.customArtifactAffix));
            return states.Where(s => CustomArtifactAffixes.Resolve(s) == null)
                .Select(s => string.IsNullOrEmpty(s?.id) ? "<empty affix ID>" :
                    string.IsNullOrEmpty(s.owner) ? s.id : $"{s.id} ({s.owner})").Distinct();
        }
    }
}
