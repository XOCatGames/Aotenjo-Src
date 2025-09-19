namespace Aotenjo
{
    public class TheDieIsCastAchievement : Achievement
    {
        public TheDieIsCastAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PreAppendSettleScoringEffectsEvent += PreAppendSettleScoringEffects;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PreAppendSettleScoringEffectsEvent -= PreAppendSettleScoringEffects;
        }

        private void PreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            Player player = permutationEvent.player;
            if (!permutationEvent.permutation.IsFullHand()) return;
            if (permutationEvent.permutation.TilesFulfullAll(t => player.DetermineFontCompactbility(t, TileFont.RED)))
            {
                SetComplete();
            }
        }
    }
}