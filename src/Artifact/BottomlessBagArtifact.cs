using System;

namespace Aotenjo
{
    public class BottomlessBagArtifact : Artifact
    {
        private static readonly int CHANCE = 3;

        public BottomlessBagArtifact() : base("bottomless_bag", Rarity.COMMON)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostUseGadgetEvent += OnPostUseGadgetEvent;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostUseGadgetEvent -= OnPostUseGadgetEvent;
        }

        private void OnPostUseGadgetEvent(PlayerGadgetEvent evt)
        {
            Gadget gadget = evt.gadget;

            if (gadget.IsConsumable() && evt.player.GetGadgets().TrueForAll(g => g.regName != gadget.regName))
            {
                if (evt.player.GenerateRandomDeterminationResult(CHANCE))
                {
                    Gadget newGadget = gadget.Copy();
                    newGadget.SetUses(1);
                    evt.player.AddGadget(newGadget);
                }
            }
        }
    }
}