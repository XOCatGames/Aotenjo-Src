using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class LeaningTowerModelArtifact : LevelingArtifact, IFuProvider
    {
        private const int FU_PER_LEVEL = 20;

        public LeaningTowerModelArtifact() : base("leaning_tower_model", Rarity.RARE, 0)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU_PER_LEVEL, FU_PER_LEVEL * Level);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player) => ToAddFuFormat(GetFu(player));

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFu(() => GetFu(player), this));
        }

        public double GetFu(Player player)
        {
            return Level * FU_PER_LEVEL;
        }

        [SubscribeToEvent]
        private void OnPostGenerateDestination(PlayerEvents.PostGenerateDestinationEvent evt)
        {
            Player player = evt.player;
            List<Destination> destinations = evt.destinations;
            // Path selection displays the first half on the left, before a choice is made.
            var candidates = new List<int>();
            for (int i = 0; i < destinations.Count / 2; i++)
            {
                Destination destination = destinations[i];
                if (destination.IsOnEvent() && !destination.onSale) continue;
                // Wasteland has no red-event variant: gold is already its final upgrade.
                if (destination.onSale && destination is WastelandDestination) continue;
                candidates.Add(i);
            }

            if (candidates.Count == 0) return;
            int index = candidates[player.GenerateRandomInt(candidates.Count, "leaning_tower_model")];
            if (destinations[index].onSale)
                destinations[index] = destinations[index].GetRandomRedEventVariant(player);
            else
                destinations[index].SetOnSale();
        }

        [SubscribeToEvent]
        private void OnChoosePath(PlayerEvents.ChoosePathEvent evt)
        {
            if (evt.direction == Direction.LEFT)
            {
                Level++;
            }
        }
    }
}
