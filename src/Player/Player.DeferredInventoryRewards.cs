using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public partial class Player
    {
        [SerializeField] private List<PendingInventoryReward> inventoryRewards = new();
        [NonSerialized] private Dictionary<string, InventoryRewardRequestedEvent> inventoryRewardEvents;
        public bool HasPendingInventoryRewards => inventoryRewards != null && inventoryRewards.Count > 0;
        internal bool HasPendingInventoryReward(string id) => inventoryRewards?.Any(r => r.id == id) == true;
        public bool HasPendingArtifactReplacement(Artifact source) =>
            source != null && inventoryRewards?.Any(r =>
                ReferenceEquals(ResolveArtifactReference(r.replacementId, r.replacementInstanceId), source)) == true;

        public IReadOnlyList<InventoryRewardRequestedEvent> GetPendingInventoryRewards() =>
            (inventoryRewards ??= new()).Select(GetInventoryRewardEvent).ToArray();

        private InventoryRewardRequestedEvent GetInventoryRewardEvent(PendingInventoryReward reward)
        {
            inventoryRewardEvents ??= new();
            if (!inventoryRewardEvents.TryGetValue(reward.id, out var evt))
                inventoryRewardEvents.Add(reward.id, evt = new InventoryRewardRequestedEvent(this, reward));
            return evt;
        }

        private bool QueueInventoryReward(PendingInventoryReward reward, Artifact source)
        {
            reward.id = Guid.NewGuid().ToString("N");
            reward.sourceInstanceId = source?.InstanceId ?? 0;
            (inventoryRewards ??= new()).Add(reward);
            var evt = new InventoryRewardRequestedEvent(this, reward, source);
            (inventoryRewardEvents ??= new()).Add(reward.id, evt);
            DispatchInventoryReward(evt);
            return !evt.IsCompleted || evt.Succeeded;
        }

        private static void DispatchInventoryReward(InventoryRewardRequestedEvent evt)
        {
            if (evt.IsCompleted || evt.IsHandled) return;
            EventBus.Publish(evt);
            // Headless logic and missing presentation still deliver earned rewards.
            if (!evt.IsHandled) evt.Complete();
        }

        public void ResumeInventoryRewards()
        {
            foreach (var evt in GetPendingInventoryRewards()) DispatchInventoryReward(evt);
        }

        /// <summary>Earned discards become usable on arrival; without a presenter they apply immediately.</summary>
        public bool RequestDiscardReward(int amount, Artifact source = null, Tile sourceTile = null)
        {
            if (checkingRecipeAdmission || amount <= 0 || (source != null && !OwnsArtifactInstance(source))) return false;
            return QueueInventoryReward(new PendingInventoryReward
            {
                discardCount = amount, sourceId = source?.GetRegName(), sourceTile = sourceTile
            }, source);
        }

        public bool RequestGadgetReward(Gadget gadget, Artifact source,
            bool preferEmptySlot = false, bool allowPartial = true)
        {
            if (checkingRecipeAdmission) return false;
            if ((source != null && !OwnsArtifactInstance(source)) || gadget == null || (gadget.IsConsumable() && gadget.uses <= 0)) return false;
            if (source == null)
            {
                return GrantGadget(gadget, allowPartial, null, preferEmptySlot);
            }
            var reward = new PendingInventoryReward
            {
                gadget = gadget.Copy(), sourceId = source.GetRegName(),
                allowPartial = allowPartial, preferEmptySlot = preferEmptySlot
            };
            int accepted = PlanGadgetReward(ProjectGadgetInventory(), GetGadgetLimit(), reward).Sum(d => d.amount);
            if (accepted == 0) return false;
            // Overflow is destroyed now, even if a slot opens before the flight arrives or after a reload.
            if (reward.gadget.IsConsumable()) reward.gadget.uses = accepted;
            return QueueInventoryReward(reward, source);
        }

        public bool RequestArtifactReward(Artifact artifact, Artifact source, bool forced = false)
        {
            if (IsReservedForRecipe(artifact)) return false;
            if (source == null) return GrantArtifact(artifact, null, forced);
            if (!OwnsArtifactInstance(source) || !IsArtifactAvailableForInventory(artifact) ||
                (!forced && !TryObtainArtifact(artifact))) return false;
            // 准入回调可能重入获取或预约；通过后仍需重新校验唯一性。
            if (!IsArtifactAvailableForInventory(artifact)) return false;
            // 现在预约抽选结果，不提前入库或执行获取效果。
            ArtifactBank.Remove(artifact.GetRegName());
            return QueueInventoryReward(new PendingInventoryReward
            {
                artifactId = artifact.GetRegName(), artifactInstanceId = artifact.InstanceId, sourceId = source.GetRegName(),
                temporary = artifact.IsTemporary, temporaryOccupiesSlot = artifact.status.temporaryOccupiesSlot,
                broken = artifact.IsBroken, debuffed = artifact.IsSuppressed
            }, source);
        }

        public void RequestArtifactReplacement(Artifact source, Artifact next)
        {
            if (IsReservedForRecipe(source) || IsReservedForRecipe(next) || !OwnsArtifactInstance(source) ||
                !IsArtifactAvailableForInventory(next) || !GetArtifacts().Contains(source) || HasPendingArtifactReplacement(source))
                return;
            ArtifactBank.Remove(next.GetRegName());
            var reward = new PendingInventoryReward
            {
                sourceId = source.GetRegName(), replacementId = source.GetRegName(), artifactId = next.GetRegName(),
                replacementInstanceId = source.InstanceId, artifactInstanceId = next.InstanceId,
                temporary = source.IsTemporary, temporaryOccupiesSlot = source.status.temporaryOccupiesSlot,
                broken = source.IsBroken, debuffed = source.IsSuppressed
            };
            var affix = source.Query(new ArtifactQueryEvent.Transfer(this, source)).Affix;
            reward.artifactAffix = affix.Builtin;
            reward.customArtifactAffix = affix.Custom;
            QueueInventoryReward(reward, source);
        }

        public void RequestFillEmptyGadgetSlots(Artifact source)
        {
            int empty = Math.Max(0, GetGadgetLimit() - ProjectGadgetInventory().Count);
            for (int i = 0; i < empty; i++)
            {
                Gadget reward = GenerateGadgets(1, false).FirstOrDefault();
                if (reward == null || !RequestGadgetReward(reward, source, preferEmptySlot: true)) break;
            }
        }

        private int ReservedArtifactSlots => reservedRecipeSlots + (inventoryRewards?.Count(r => !string.IsNullOrEmpty(r.artifactId) &&
            string.IsNullOrEmpty(r.replacementId) && (!r.temporary || r.temporaryOccupiesSlot)) ?? 0);

        internal bool CompleteInventoryReward(PendingInventoryReward reward)
        {
            if (checkingRecipeAdmission) return false;
            // 只信任队列中稳定 ID 对应的权威载荷；先消费记录，再执行可能重入的回调。
            var pending = inventoryRewards?.FirstOrDefault(r => r.id == reward?.id);
            if (pending == null) return false;
            reward = pending;
            inventoryRewards.Remove(pending);
            inventoryRewardEvents?.Remove(reward.id);
            if (reward.discardCount > 0)
            {
                DiscardLeft += reward.discardCount;
                AudioSystem.PlayAddSwapChanceSound();
                return true;
            }
            Artifact source = ResolveArtifactReference(reward.sourceId, reward.sourceInstanceId);
            if (reward.gadget != null)
            {
                // Recheck capacity on arrival; any newly overflowing portion is also destroyed.
                return GrantGadget(reward.gadget.Copy(), true, source, reward.preferEmptySlot);
            }

            Artifact artifact = ResolveArtifactReference(reward.artifactId, reward.artifactInstanceId);
            if (artifact == null || GetArtifacts().Contains(artifact)) return false;
            int index = -1;
            if (!string.IsNullOrEmpty(reward.replacementId))
            {
                Artifact previous = ResolveArtifactReference(reward.replacementId, reward.replacementInstanceId);
                index = GetArtifacts().IndexOf(previous);
                if (!RemoveArtifact(previous, true, false)) return false;
                artifact.ResetArtifactState();
            }
            artifact.IsTemporary = reward.temporary;
            artifact.status.temporaryOccupiesSlot = reward.temporaryOccupiesSlot;
            artifact.IsBroken = reward.broken;
            artifact.IsSuppressed = reward.debuffed;
            // Admission was checked and reserved when the request was made.
            artifact.RuntimeAffix = new ArtifactAffixState(reward.artifactAffix, reward.customArtifactAffix);
            bool obtained = GrantArtifact(artifact, source, true);
            if (obtained && index >= 0)
            {
                var order = GetArtifacts();
                order.Remove(artifact);
                order.Insert(Math.Min(index, order.Count), artifact);
                SetArtifactOrder(order.ToArray());
            }
            return obtained;
        }

        internal List<GadgetRewardDestination> GetGadgetRewardDestinations(PendingInventoryReward reward) =>
            PlanGadgetReward(HeldGadgets, GetGadgetLimit(), reward);

        private List<Gadget> ProjectGadgetInventory()
        {
            var inventory = HeldGadgets.Select(g => g.Copy()).ToList();
            foreach (var reward in inventoryRewards ?? new List<PendingInventoryReward>())
            {
                if (reward.gadget == null) continue;
                foreach (var destination in PlanGadgetReward(inventory, GetGadgetLimit(), reward))
                {
                    if (destination.target != null) destination.target.uses += destination.amount;
                    else
                    {
                        Gadget copy = reward.gadget.Copy();
                        if (copy.IsConsumable()) copy.uses = destination.amount;
                        inventory.Add(copy);
                    }
                }
            }
            return inventory;
        }

        // Preview the same merge order as AddGadgetCore, without lifecycle hooks or random draws.
        private static List<GadgetRewardDestination> PlanGadgetReward(List<Gadget> inventory, int limit,
            PendingInventoryReward reward)
        {
            var result = new List<GadgetRewardDestination>();
            if (reward.gadget == null) return result;
            Gadget gadget = reward.gadget;
            int remaining = GadgetUnits(gadget);
            bool hasSpace = inventory.Count < limit;
            if (reward.preferEmptySlot && hasSpace)
            {
                result.Add(new GadgetRewardDestination(inventory.Count, null, remaining));
                return result;
            }
            for (int i = 0; i < inventory.Count; i++)
            {
                Gadget target = inventory[i];
                if (!target.IsConsumable() || target.regName != gadget.regName) continue;
                int space = target.GetStackLimit() - target.uses;
                if (space <= 0 || (!hasSpace && !reward.allowPartial && space < remaining)) continue;
                int amount = Math.Min(space, remaining);
                result.Add(new GadgetRewardDestination(i, target, amount));
                remaining -= amount;
                if (remaining == 0) return result;
            }
            if (remaining > 0 && hasSpace) result.Add(new GadgetRewardDestination(inventory.Count, null, remaining));
            return result;
        }
    }
}
