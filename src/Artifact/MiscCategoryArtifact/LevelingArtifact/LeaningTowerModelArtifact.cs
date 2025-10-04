using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class LeaningTowerModelArtifact : LevelingArtifact, IFuProvider
    {
        private const int FU_PER_LEVEL = 30;

        public LeaningTowerModelArtifact() : base("leaning_tower_model", Rarity.RARE, 0)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU_PER_LEVEL, FU_PER_LEVEL * Level, FU_PER_LEVEL);
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

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.ChoosePathEvent += OnChoosePath;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.ChoosePathEvent -= OnChoosePath;
        }

        private void OnChoosePath(PlayerChoosePathEvent evt)
        {
            if (evt.direction == Direction.LEFT)
            {
                Level++;
            }
            else
            {
                Level--;
                if (Level < 0) Level = 0;
            }
        }
    }
}