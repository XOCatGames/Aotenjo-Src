using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public partial class Player
    {
        [NonSerialized] private Dictionary<string, Artifact> runArtifacts;
        [NonSerialized] private Dictionary<int, Artifact> artifactInstances;
        [NonSerialized] private int nextArtifactInstanceId;
        [NonSerialized] private List<Artifact> orderedRunArtifacts;
        [NonSerialized] private List<Artifact> heldArtifacts;
        [NonSerialized] private HashSet<Artifact> subscribedArtifacts;
        [NonSerialized] private bool artifactPreview;
        [SerializeField] private SerializableMap<string, string> artifactOwners;
        [SerializeField] private SerializableMap<int, string> artifactInstanceDefinitions;
        [SerializeField] private List<int> heldArtifactInstanceIds;

        /// <summary>
        /// 只构造目录，不重置、不抽随机数也不触发获取。新局仍在原来的牌池初始化之后预初始化；
        /// JsonUtility 续局则先调用此入口恢复非序列化容器，再从保存快照显式恢复全部字段。
        /// </summary>
        public void InitializeArtifactRuntime()
        {
            if (runArtifacts != null) throw new InvalidOperationException("本局遗物目录已初始化。");
            if (!artifactPreview) ArtifactRegistry.Freeze();
            runArtifacts = new Dictionary<string, Artifact>(StringComparer.Ordinal);
            artifactInstances = new Dictionary<int, Artifact>();
            orderedRunArtifacts = new List<Artifact>();
            heldArtifacts = new List<Artifact>();
            subscribedArtifacts = new HashSet<Artifact>();
            nextArtifactInstanceId = 1;
            if (artifactInstanceDefinitions != null)
            {
                foreach (int instanceId in artifactInstanceDefinitions.GetKeys())
                    AddArtifactInstance(artifactInstanceDefinitions.Get(instanceId), instanceId);
            }
            // 保留原目录顺序与未持有的随机预设；新增副本才追加编号，不占用游戏随机序列。
            foreach (var entry in ArtifactRegistry.OrderedEntries)
                if (!runArtifacts.ContainsKey(entry.Id)) AddArtifactInstance(entry.Id, nextArtifactInstanceId);
            if (!artifactPreview) ArtifactRegistry.TrackRuntime(this);
        }

        private Artifact AddArtifactInstance(string id, int instanceId)
        {
            if (instanceId <= 0 || instanceId == int.MaxValue || artifactInstances.ContainsKey(instanceId))
                throw new InvalidDataException("遗物实例编号无效或重复。");
            var artifact = artifactPreview ? ArtifactRegistry.CreatePreview(id) : ArtifactRegistry.GetDefinition(id).CreateRuntime();
            artifact.InstanceId = instanceId;
            if (!artifactPreview) artifact.RuntimeOwner = this;
            artifactInstances.Add(instanceId, artifact);
            orderedRunArtifacts.Add(artifact);
            if (!runArtifacts.ContainsKey(id)) runArtifacts.Add(id, artifact);
            nextArtifactInstanceId = Math.Max(nextArtifactInstanceId, instanceId + 1);
            return artifact;
        }

        /// <summary>创建本局独立副本；构造、成长、状态和词条均不借用已有同名遗物。</summary>
        public Artifact CreateArtifactInstance(string id)
        {
            RequireArtifactRuntime();
            if (artifactPreview) throw new InvalidOperationException("历史预览不能创建运行遗物。");
            var artifact = AddArtifactInstance(id, nextArtifactInstanceId);
            artifact.ResetArtifactState(this);
            artifact.PreGameInitialized(this);
            return artifact;
        }

        public Artifact GetArtifactInstance(int instanceId)
        {
            RequireArtifactRuntime();
            return artifactInstances.TryGetValue(instanceId, out var artifact) ? artifact
                : throw new KeyNotFoundException("本局不存在遗物实例：" + instanceId);
        }

        internal Artifact ResolveArtifactReference(string id, int instanceId)
        {
            if (instanceId == 0) return string.IsNullOrEmpty(id) ? null : GetRunArtifact(id);
            var artifact = GetArtifactInstance(instanceId);
            if (artifact.GetRegName() != id) throw new InvalidDataException("遗物实例与类型不匹配。");
            return artifact;
        }

        // 实际发奖或合成时复用未持有、未预约的候选；查询不能创建副本或抽取随机预设。
        internal Artifact GetAvailableArtifact(string id) => orderedRunArtifacts.FirstOrDefault(a =>
            a.GetRegName() == id && IsArtifactAvailableForInventory(a) && !IsReservedForRecipe(a)) ?? CreateArtifactInstance(id);

        public bool HasArtifactRuntime => runArtifacts != null && heldArtifacts != null;

        private void RequireArtifactRuntime()
        {
            if (runArtifacts == null || heldArtifacts == null)
                throw new InvalidOperationException("本局遗物目录尚未恢复或已经释放。");
        }

        public Artifact GetRunArtifact(string id)
        {
            RequireArtifactRuntime();
            return TryGetRunArtifact(id, out var artifact) ? artifact
                : throw new KeyNotFoundException("本局不存在遗物：" + id);
        }

        public bool TryGetRunArtifact(string id, out Artifact artifact)
        {
            RequireArtifactRuntime();
            if (TryGetHeldArtifact(id, out artifact)) return true;
            return id != null && runArtifacts.TryGetValue(id, out artifact);
        }

        public IReadOnlyList<Artifact> GetRunArtifacts()
        {
            RequireArtifactRuntime();
            return orderedRunArtifacts.AsReadOnly();
        }

        public bool HasArtifact(string id) => TryGetHeldArtifact(id, out _);
        public bool TryGetHeldArtifact(string id, out Artifact artifact)
        {
            RequireArtifactRuntime();
            artifact = (recipeInventoryPreview ?? heldArtifacts).FirstOrDefault(a => a.GetRegName() == id);
            return artifact != null;
        }
        public bool TryGetHeldArtifact<T>(string id, out T artifact) where T : Artifact
        {
            artifact = null;
            if (!TryGetHeldArtifact(id, out var candidate) || !(candidate is T typed)) return false;
            artifact = typed;
            return true;
        }

        public bool OwnsArtifactInstance(Artifact artifact) => !artifactPreview && artifact != null &&
            artifactInstances != null && artifactInstances.TryGetValue(artifact.InstanceId, out var current) &&
            ReferenceEquals(current, artifact);

        public bool HoldsArtifact(Artifact artifact) => OwnsArtifactInstance(artifact) &&
            (recipeInventoryPreview ?? heldArtifacts).Contains(artifact);

        public virtual List<Artifact> GetArtifacts()
        {
            RequireArtifactRuntime();
            return new List<Artifact>(recipeInventoryPreview ?? heldArtifacts);
        }

        /// <summary>只读取兼容存档字段，不把未知现代 ID 过滤后错误回退成旧数字库存。</summary>
        public List<string> GetSavedArtifactIds()
        {
            if (NewHeldArtifacts != null && NewHeldArtifacts.Count > 0) return new List<string>(NewHeldArtifacts);
            return (HeldArtifacts ?? new List<int>()).Select(number =>
                ArtifactRegistry.FindLegacyDefinition(number)?.Id ?? "legacy:" + number).ToList();
        }

        // 旧档可能没有来源快照，删除的未持有遗物仍会留在商店池；恢复和写入前统一报告，
        // 不静默删掉池内容，避免读档成功后才在场景内的严格查找中失败。
        public IEnumerable<string> GetMissingSavedArtifactIds() => GetSavedArtifactIds()
            .Concat(ArtifactBank ?? Enumerable.Empty<string>())
            .Concat((artifactInstanceDefinitions?.GetKeys() ?? Array.Empty<int>()).Select(id => artifactInstanceDefinitions.Get(id)))
            .Where(id => !ArtifactRegistry.TryGetDefinition(id, out _))
            .Select(id => string.IsNullOrEmpty(id) ? "<empty artifact ID>" : id).Distinct();

        private void RestoreHeldArtifactReferences(IEnumerable<string> ids)
        {
            var saved = ids.ToList();
            if (artifactInstanceDefinitions != null)
                heldArtifacts = (heldArtifactInstanceIds ?? throw new InvalidDataException("缺少遗物实例库存。")).Select(GetArtifactInstance).ToList();
            else
            {
                // 旧档仅按类型保存状态；重复类型恢复成独立对象，各自读取同一份旧载荷。
                foreach (string id in saved)
                {
                    var artifact = GetRunArtifact(id);
                    if (heldArtifacts.Contains(artifact)) artifact = AddArtifactInstance(id, nextArtifactInstanceId);
                    heldArtifacts.Add(artifact);
                }
            }
            ValidateArtifactRuntime();
        }

        public void ValidateArtifactRuntime()
        {
            RequireArtifactRuntime();
            if (heldArtifacts.Distinct().Count() != heldArtifacts.Count ||
                heldArtifacts.Any(a => !artifactInstances.TryGetValue(a.InstanceId, out var current) || !ReferenceEquals(a, current)))
                throw new InvalidDataException("持有遗物与本局目录的引用不一致。");
        }

        /// <summary>
        /// 按实例编号保存成长、状态与库存顺序，同时保留类型列表供旧历史和缺失内容检查使用。
        /// 两个保存入口共用此方法，避免遗漏未持有的随机预设与尚未到账的奖励实例。
        /// 此方法必须在主线程调用；Lua 序列化及 Unity JsonUtility 不能搬到异步文件写入线程。
        /// </summary>
        public void CaptureArtifactState(out SerializableMap<string, string> data, out SerializableMap<string, string> statuses)
        {
            ValidateArtifactRuntime();
            if (artifactPreview) throw new InvalidOperationException("历史预览不能保存为对局。");
            var captured = new SerializableMap<string, string>();
            var capturedStatuses = new SerializableMap<string, string>();
            var definitions = new SerializableMap<int, string>();
            foreach (var artifact in orderedRunArtifacts)
            {
                definitions.Add(artifact.InstanceId, artifact.GetRegName());
                captured.Add(artifact.InstanceId.ToString(), artifact.Serialize());
                capturedStatuses.Add(artifact.InstanceId.ToString(), JsonUtility.ToJson(artifact.status));
            }
            // 所有序列化成功后才更新兼容库存；空库存同时清空旧数字列表，防止续局复活旧遗物。
            CaptureArtifactAffixSnapshots();
            var owners = new SerializableMap<string, string>();
            foreach (var artifact in orderedRunArtifacts)
                owners.Add(artifact.GetRegName(), ArtifactRegistry.GetDefinition(artifact.GetRegName()).OwnerId);
            artifactOwners = owners;
            artifactInstanceDefinitions = definitions;
            heldArtifactInstanceIds = heldArtifacts.Select(a => a.InstanceId).ToList();
            NewHeldArtifacts = heldArtifacts.Select(a => a.GetRegName()).ToList();
            HeldArtifacts = new List<int>();
            data = captured;
            statuses = capturedStatuses;
        }

        public void RestoreArtifactRuntime(Round round)
        {
            var ids = GetSavedArtifactIds();
            if (artifactOwners != null)
                foreach (string id in artifactOwners.GetKeys())
                    if (!ArtifactRegistry.TryGetDefinition(id, out var definition) ||
                        !string.Equals(definition.OwnerId, artifactOwners.Get(id), StringComparison.Ordinal))
                        throw new InvalidDataException("遗物定义来源缺失或不匹配：" + id);
            try
            {
                InitializeArtifactRuntime();
                RestoreHeldArtifactReferences(ids);
                foreach (var artifact in orderedRunArtifacts)
                {
                    try
                    {
                        string key = artifactInstanceDefinitions == null ? artifact.GetNameKey() : artifact.InstanceId.ToString();
                        if (round.artifactData != null && round.artifactData.Contains(key))
                        {
                            artifact.ResetArtifactState();
                            artifact.Deserialize(round.artifactData.Get(key));
                        }
                        else if (heldArtifacts.Contains(artifact) || artifactInstanceDefinitions?.Contains(artifact.InstanceId) == true)
                            throw new InvalidDataException("遗物实例缺少保存载荷。");
                        else
                        {
                            // 旧档未记录的候选须完整初始化；已有载荷不重抽，避免覆盖状态或推进随机序列。
                            artifact.ResetArtifactState(this);
                            artifact.PreGameInitialized(this);
                        }

                        artifact.status = new Artifact.ArtifactStatus();
                        if (round.artifactStatusData != null && round.artifactStatusData.Contains(key))
                        {
                            string status = round.artifactStatusData.Get(key);
                            if (string.IsNullOrWhiteSpace(status) || !status.TrimStart().StartsWith("{"))
                                throw new InvalidDataException("遗物状态格式无效。");
                            artifact.status = JsonUtility.FromJson<Artifact.ArtifactStatus>(status)
                                ?? throw new InvalidDataException("遗物状态缺失。");
                        }
                        else if (heldArtifacts.Contains(artifact) || artifactInstanceDefinitions?.Contains(artifact.InstanceId) == true)
                            throw new InvalidDataException("遗物实例缺少状态键。");
                    }
                    catch (Exception error)
                    {
                        throw new SaveCompatibilityException(new SaveLoadIssue(SaveLoadIssueKind.ArtifactState,
                            artifact.GetRegName()), error);
                    }
                }
                RestoreArtifactAffixSnapshots();
                ValidatePendingArtifactRewards();
            }
            catch
            {
                // 恢复阶段从未绑定事件；只丢弃候选目录，不调用模组解绑或失去持有效果。
                runArtifacts = null; artifactInstances = null; orderedRunArtifacts = null; heldArtifacts = null; subscribedArtifacts = null;
                inventoryRewardEvents = null;
                throw;
            }
        }

        private void ValidatePendingArtifactRewards()
        {
            var rewards = inventoryRewards ?? new List<PendingInventoryReward>();
            if (rewards.Any(r => r == null || string.IsNullOrEmpty(r.id)) || rewards.Select(r => r.id).Distinct().Count() != rewards.Count)
                throw new InvalidDataException("待发奖励包含空记录或重复奖励 ID。");
            if (rewards.Where(r => !string.IsNullOrEmpty(r.artifactId))
                    .Select(r => ResolveArtifactReference(r.artifactId, r.artifactInstanceId)).Distinct().Count() !=
                rewards.Count(r => !string.IsNullOrEmpty(r.artifactId)))
                throw new InvalidDataException("同一遗物被重复预约。");
            foreach (var reward in rewards)
            {
                // 一个稳定奖励 ID 只能携带一种载荷；提前拒绝损坏队列，避免场景首存后才在到账时出错。
                bool artifactReward = !string.IsNullOrEmpty(reward.artifactId);
                int kinds = (artifactReward ? 1 : 0) + (reward.gadget != null ? 1 : 0) + (reward.discardCount > 0 ? 1 : 0);
                if (kinds != 1 || reward.discardCount < 0 ||
                    (!artifactReward && (!string.IsNullOrEmpty(reward.replacementId) ||
                        reward.artifactAffix != ArtifactAffix.None || reward.customArtifactAffix != null)))
                    throw new InvalidDataException("待发奖励载荷缺失或类型冲突。");
                _ = new ArtifactAffixState(reward.artifactAffix, reward.customArtifactAffix);
                if (!string.IsNullOrEmpty(reward.customArtifactAffix?.id) && CustomArtifactAffixes.Resolve(reward.customArtifactAffix) == null)
                    throw new InvalidDataException("待发词条来源缺失或不匹配。");
                var artifact = ResolveArtifactReference(reward.artifactId, reward.artifactInstanceId);
                var source = ResolveArtifactReference(reward.sourceId, reward.sourceInstanceId);
                var replacement = ResolveArtifactReference(reward.replacementId, reward.replacementInstanceId);
                // 旧奖励仅有类型 ID；读档时立即固定实例，之后重排或出现同名副本也不改变目标。
                reward.artifactInstanceId = artifact?.InstanceId ?? 0;
                reward.sourceInstanceId = source?.InstanceId ?? 0;
                reward.replacementInstanceId = replacement?.InstanceId ?? 0;
                if (artifact != null && HoldsArtifact(artifact))
                    throw new InvalidDataException("待发遗物已经持有：" + reward.artifactId);
                if (replacement != null && !HoldsArtifact(replacement))
                    throw new InvalidDataException("待升级的原遗物已经缺失：" + reward.replacementId);
            }
            inventoryRewardEvents = null;
        }

        internal void TrackArtifactSubscription(Artifact artifact) =>
            (subscribedArtifacts ??= new HashSet<Artifact>()).Add(artifact);
        internal void ForgetArtifactSubscription(Artifact artifact) => subscribedArtifacts?.Remove(artifact);

        internal void RebindArtifactSubscriptions(Artifact changed)
        {
            if (subscribedArtifacts?.Contains(changed) != true) return;
            if (updatingArtifactOrder)
            {
                // 宿主订阅回调可以更换自己的词条；此时只补上当前词条，不能递归重绑整份库存。
                changed.Affix?.SubscribeToPlayer(this);
                return;
            }
            // 已持有遗物更换词条时，沿用宿主的订阅生命周期恢复原库存顺序。
            // 只重绑原本激活的宿主，不激活尚未订阅的实例，也不触发获取、移除或自动合成。
            var active = heldArtifacts.Where(subscribedArtifacts.Contains).ToArray();
            updatingArtifactOrder = true;
            try
            {
                foreach (var artifact in active) artifact.UnsubscribeToPlayer(this);
                foreach (var artifact in active) artifact.SubscribeToPlayer(this);
            }
            finally { updatingArtifactOrder = false; }
        }

        public void ActivateArtifactRuntime()
        {
            ValidateArtifactRuntime();
            if (artifactPreview) throw new InvalidOperationException("预览遗物不能绑定战斗事件。");
            try
            {
                foreach (var artifact in heldArtifacts)
                {
                    if (subscribedArtifacts?.Contains(artifact) == true) continue;
                    // 先记下正在绑定的对象，子类部分绑定后抛错也必须进入清理列表。
                    TrackArtifactSubscription(artifact);
                    artifact.SubscribeToPlayer(this);
                }
            }
            catch
            {
                try { DeactivateArtifactRuntime(); }
                catch (Exception cleanup) { Debug.LogException(cleanup); }
                throw;
            }
        }

        public void DeactivateArtifactRuntime()
        {
            if (subscribedArtifacts == null) return;
            // 不调用 OnRemoved：退出不能额外触发卖出、回池或临时栏位结算。
            // 即使一个模组解绑失败，也继续释放其余对象的监听。
            Exception failure = null;
            foreach (var artifact in subscribedArtifacts.ToArray())
                try { artifact.UnsubscribeToPlayer(this); }
                catch (Exception error) { failure ??= error; }
            subscribedArtifacts.Clear();
            if (failure != null) throw new InvalidOperationException("遗物监听清理失败。", failure);
        }

        public void ReleaseArtifactRuntime()
        {
            try { DeactivateArtifactRuntime(); }
            finally
            {
                runArtifacts = null; artifactInstances = null; orderedRunArtifacts = null; heldArtifacts = null; subscribedArtifacts = null;
                inventoryRewardEvents = null;
            }
        }
    }
}
