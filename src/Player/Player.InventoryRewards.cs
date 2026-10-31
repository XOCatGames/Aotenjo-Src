using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public partial class Player
    {
        public Dictionary<Gadget, int> CaptureGadgetInventory() =>
            HeldGadgets.ToDictionary(g => g, GadgetUnits);

        private static int GadgetUnits(Gadget gadget) => gadget.IsConsumable() ? gadget.uses : 1;

        /// <summary>Keep acquisition effects separate from the existing ObtainGadgetEvent contract.</summary>
        public bool GrantGadget(Gadget gadget, bool allowPartial = false, Artifact source = null,
            bool preferEmptySlot = false)
        {
            if (checkingRecipeAdmission) return false;
            if (gadget == null || (gadget.IsConsumable() && gadget.uses <= 0)) return false;
            var before = CaptureGadgetInventory();
            bool result;
            if (preferEmptySlot && HeldGadgets.Count < GetGadgetLimit())
            {
                gadget.OnObtained(this);
                EventBus.Publish(new PlayerEvents.ObtainGadgetEvent(this, gadget));
                stats.OnBoughtGadget(gadget);
                HeldGadgets.Add(gadget);
                result = true;
            }
            else
            {
                result = AddGadgetCore(gadget, allowPartial);
            }
            if (result) NotifyGadgetGains(before, source);
            return result;
        }

        public void NotifyGadgetGains(Dictionary<Gadget, int> before, Artifact source = null)
        {
            foreach (var gain in GetGadgetGains(before))
                EventBus.Publish(new InventoryAcquiredEvent(this, gain.gadget, gain.amount, source));
        }

        internal List<(Gadget gadget, int amount)> GetGadgetGains(Dictionary<Gadget, int> before)
        {
            var gains = new List<(Gadget, int)>();
            // The per-type budget also prevents inventory reordering/replacing equal stacks from looking like gains.
            var remaining = HeldGadgets.GroupBy(g => g.regName)
                .ToDictionary(group => group.Key, group => group.Sum(GadgetUnits) -
                    before.Where(pair => pair.Key.regName == group.Key).Sum(pair => pair.Value));
            foreach (Gadget gadget in HeldGadgets.ToArray())
            {
                int delta = GadgetUnits(gadget) - (before.TryGetValue(gadget, out int previous) ? previous : 0);
                int gained = Math.Min(delta, remaining[gadget.regName]);
                if (gained <= 0) continue;
                remaining[gadget.regName] -= gained;
                gains.Add((gadget, gained));
            }
            return gains;
        }

        /// <summary>Grant as much as fits and discard all overflow immediately.</summary>
        public void AwardGadget(Gadget reward, Artifact source = null)
        {
            if (reward == null) return;
            GrantGadget(reward.Copy(), true, source);
        }

        public void FillEmptyGadgetSlots(Artifact source)
        {
            int empty = Math.Max(0, GetGadgetLimit() - HeldGadgets.Count);
            for (int i = 0; i < empty; i++)
            {
                // A fill reward intentionally starts a new stack so an automatic merge cannot leave a hole.
                Gadget reward = GenerateGadgets(1, false).FirstOrDefault();
                if (reward == null || !GrantGadget(reward, false, source, true)) break;
            }
        }

        public void CompleteGadgetRewardSelection(List<Gadget> inventory,
            Dictionary<Gadget, int> before)
        {
            // Only new instances get the obtain lifecycle; existing reusable gadgets keep their used charges.
            foreach (Gadget gadget in inventory.Where(g => !before.ContainsKey(g)))
                gadget.OnObtained(this);
            SetGadgets(inventory);
            foreach (var gain in GetGadgetGains(before))
            {
                // Merged consumables also count as acquired; achievements need the committed inventory.
                EventBus.Publish(new PlayerEvents.ObtainGadgetEvent(this, gain.gadget));
                stats.OnBoughtGadget(gain.gadget);
                EventBus.Publish(new InventoryAcquiredEvent(this, gain.gadget, gain.amount));
            }
        }
    }
}
