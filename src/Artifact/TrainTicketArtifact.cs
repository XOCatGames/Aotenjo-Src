using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TrainTicketArtifact : Artifact, IActivable
    {
        private const int MONEY_AMOUNT = 2;
        private bool first = true;

        public TrainTicketArtifact() : base("train_ticket", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MONEY_AMOUNT);
        }

        public override void ResetArtifactState()
        {
            first = true;
        }

        public override string Serialize()
        {
            return first.ToString();
        }

        public override void Deserialize(string data)
        {
            first = bool.Parse(data);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostRoundStartEvent += OnRoundStart;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostRoundStartEvent -= OnRoundStart;
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (first && block.IsABC() && permutation.blocks.Any(otherB =>
                    otherB != block && player.GetCombinator().ASuccB(block, otherB, false, 0)))
            {
                effects.Add(new EarnMoneyEffect(MONEY_AMOUNT, this));
                first = false;
            }
        }

        public void OnRoundStart(PlayerEvent playerEvent)
        {
            first = true;
        }

        public bool IsActivating()
        {
            return first;
        }
    }
}