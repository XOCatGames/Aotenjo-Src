using System.Linq;
using System.Collections.Generic;

namespace Aotenjo
{
    public class MuseumAchievement : Achievement
    {
        private readonly Dictionary<CollectorsAmuletArtifact, int> counts = new();

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
            counts.Clear();
        }

        private void PreAppendPlayHand(PlayerPermutationEvent evt)
        {
            counts.Clear();
            foreach (var artifact in evt.player.GetArtifacts().OfType<CollectorsAmuletArtifact>())
                counts.Add(artifact, artifact.collectedMaterials.Count);
        }

        private void PostSettle(PlayerPermutationEvent permutationEvent)
        {
            // 分别比较每件遗物的新增收藏，不把多件的增长相加，也不因重排切换比较对象。
            if (permutationEvent.player.GetArtifacts().OfType<CollectorsAmuletArtifact>()
                .Any(artifact => artifact.collectedMaterials.Count - counts.GetValueOrDefault(artifact) >= 4)) SetComplete();
            counts.Clear();
        }
    }
}
