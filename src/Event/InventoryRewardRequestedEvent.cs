using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    /// <summary>只保存载荷，不包含委托、场景对象或动画状态。</summary>
    [Serializable]
    public sealed class PendingInventoryReward
    {
        public string id;
        public string sourceId;
        public int discardCount;
        [SerializeReference] public Tile sourceTile;
        [SerializeReference] public Gadget gadget;
        public string artifactId;
        public string replacementId;
        // 类型字段用于旧档兼容和缺失内容提示，实例编号决定本次奖励的实际对象。
        public int sourceInstanceId;
        public int artifactInstanceId;
        public int replacementInstanceId;
        public ArtifactAffix artifactAffix;
        [SerializeField] internal CustomArtifactAffixState customArtifactAffix;
        public bool allowPartial;
        public bool preferEmptySlot;
        public bool temporary;
        public bool temporaryOccupiesSlot;
        public bool broken;
        public bool debuffed;
    }

    /// <summary>The presenter claims this request and completes it when the reward reaches its slot.</summary>
    public sealed class InventoryRewardRequestedEvent : PlayerEvent
    {
        internal readonly PendingInventoryReward reward;
        public string Id => reward.id;
        public readonly Artifact source;
        public readonly Gadget displayGadget;
        public bool IsDiscardReward => reward.discardCount > 0;
        public Tile SourceTile => reward.sourceTile;
        public Artifact artifact => player.ResolveArtifactReference(reward.artifactId, reward.artifactInstanceId);
        public Artifact replacement => player.ResolveArtifactReference(reward.replacementId, reward.replacementInstanceId);
        public int amount => IsDiscardReward ? reward.discardCount :
            displayGadget == null || !displayGadget.IsConsumable() ? 1 : displayGadget.uses;
        public bool IsCompleted => !player.HasPendingInventoryReward(Id);
        public bool IsHandled { get; private set; }
        public bool Succeeded { get; private set; }
        public IReadOnlyList<Gadget> ReceivedGadgets { get; private set; } = Array.Empty<Gadget>();

        internal InventoryRewardRequestedEvent(Player player, PendingInventoryReward reward, Artifact source = null) : base(player)
        {
            this.reward = reward;
            this.source = source ?? player.ResolveArtifactReference(reward.sourceId, reward.sourceInstanceId);
            displayGadget = reward.gadget?.Copy();
        }

        public bool TryHandle()
        {
            if (IsHandled || IsCompleted) return false;
            IsHandled = true;
            return true;
        }

        public void Complete()
        {
            if (IsCompleted) return;
            var before = reward.gadget != null ? player.CaptureGadgetInventory() : null;
            Succeeded = player.CompleteInventoryReward(reward);
            if (before != null)
                ReceivedGadgets = player.GetGadgetGains(before).Select(g => g.gadget).ToArray();
        }

        public List<GadgetRewardDestination> GetGadgetDestinations() => player.GetGadgetRewardDestinations(reward);
        public int GetArtifactSlot() => replacement != null && player.GetArtifacts().Contains(replacement)
            ? player.GetArtifacts().IndexOf(replacement) : player.GetArtifacts().Count;
    }

    public readonly struct GadgetRewardDestination
    {
        public readonly int slot;
        public readonly Gadget target;
        public readonly int amount;
        public GadgetRewardDestination(int slot, Gadget target, int amount)
        {
            this.slot = slot;
            this.target = target;
            this.amount = amount;
        }
    }
}
