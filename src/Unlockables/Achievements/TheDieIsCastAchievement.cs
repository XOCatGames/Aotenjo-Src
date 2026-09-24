namespace Aotenjo
{
    public class TheDieIsCastAchievement : Achievement
    {
        public TheDieIsCastAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
        }

        private void PreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            Player player = permutationEvent.player;
            if (!permutationEvent.permutation.IsFullHand(permutationEvent.player)) return;
            if (permutationEvent.permutation.TilesFulfullAll(t => player.DetermineFontCompatibility(t, TileFont.RED)))
            {
                SetComplete();
            }
        }
    }
}