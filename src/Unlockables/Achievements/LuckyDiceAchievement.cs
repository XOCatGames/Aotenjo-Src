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
            player.PostObtainArtifactEvent += OnObtainArtifact;
        }

        private void OnObtainArtifact(PlayerArtifactEvent evt)
        {
            Player player = evt.player;
            if (!player.GetArtifacts().Contains(Artifacts.LeadBlock)) return;

            Artifact[] dice = { Artifacts.D10Dice, Artifacts.GoldenD4Dice, Artifacts.IvoryD3Dice };

            if (dice.All(d => player.GetArtifacts().Contains(d)))
            {
                SetComplete();
            }
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostObtainArtifactEvent -= OnObtainArtifact;
        }
    }
}