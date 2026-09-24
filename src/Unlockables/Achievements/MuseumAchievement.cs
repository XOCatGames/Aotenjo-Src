using System.Linq;

namespace Aotenjo
{
    public class MuseumAchievement : Achievement
    {
        private int number;

        public MuseumAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PostSettlePermutationEvent>(player, PostSettle);
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendPlayHand);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PostSettlePermutationEvent>(player, PostSettle);
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendPlayHand);
        }

        private void PreAppendPlayHand(PlayerPermutationEvent evt)
        {
            if (evt.player.GetArtifacts().Contains(Artifacts.CollectorsAmulet))
            {
                number = ((CollectorsAmuletArtifact)Artifacts.CollectorsAmulet).collectedMaterials.Count();
            }
            else
            {
                number = 0;
            }
        }

        private void PostSettle(PlayerPermutationEvent permutationEvent)
        {
            if (permutationEvent.player.GetArtifacts().Contains(Artifacts.CollectorsAmulet))
            {
                int diff = ((CollectorsAmuletArtifact)Artifacts.CollectorsAmulet).collectedMaterials.Count() - number;
                if (diff >= 4) SetComplete();
            }

            number = 0;
        }
    }
}