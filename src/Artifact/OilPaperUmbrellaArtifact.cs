using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class OilPaperUmbrellaArtifact : Artifact
    {
        private const int moneyBonus = 3;

        public OilPaperUmbrellaArtifact() : base("oil_paper_umbrella", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> func)
        {
            return string.Format(base.GetDescription(func), moneyBonus);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (permutation is SevenPairsPermutation || permutation is ThirteenOrphansPermutation)
            {
                return;
            }

            if (block.Any(t => !player.Selecting(t))) return;

            if (player.GetCombinator().IsKong(block))
            {
                effects.Add(new EarnMoneyEffect(moneyBonus, this));
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PreKongTileEvent>(player, OnKong);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PreKongTileEvent>(player, OnKong);
        }

        private void OnKong(PlayerKongTileEvent eventData)
        {
            Player player = eventData.player;
            player.EarnMoney(moneyBonus);
            MessageManager.Instance.OnArtifactEarnMoney(moneyBonus, this);
        }
    }
}
