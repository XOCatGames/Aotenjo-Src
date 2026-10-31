using System.Linq;

namespace Aotenjo
{
    public class LuckyDiceAchievement : Achievement
    {
        public LuckyDiceAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PostObtainArtifactEvent>(player, OnObtainArtifact);
        }

        private void OnObtainArtifact(PlayerArtifactEvent evt)
        {
            Player player = evt.player;
            if (!player.HasArtifact(Artifacts.LeadBlock.Id)) return;

            string[] dice = { Artifacts.D10Dice.Id, Artifacts.GoldenD4Dice.Id, Artifacts.IvoryD3Dice.Id };

            if (dice.All(player.HasArtifact))
            {
                SetComplete();
            }
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PostObtainArtifactEvent>(player, OnObtainArtifact);
        }
    }
}