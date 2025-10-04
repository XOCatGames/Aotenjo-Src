using System;
using System.Collections.Generic;
using System.Linq;
using static Aotenjo.Tile;

namespace Aotenjo
{
    public class ScarletKaleidoscopeArtifact : Artifact
    {
        private const double FAN_MULTIPLIER = 1.5; //番数倍数
        private const int LEVEL_BONUS = 1; //等级增加数

        public ScarletKaleidoscopeArtifact() : base("scarlet_kaleidoscope", Rarity.EPIC)
        {
            
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);

            if (player is ScarletPlayer scarlet)
            {
                scarlet.GetScarletCoreLevelEvent += OnGetCoreLevel;
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);

            if (player is ScarletPlayer scarlet)
            {
                scarlet.GetScarletCoreLevelEvent -= OnGetCoreLevel;
            }
        }

        private void OnGetCoreLevel(PlayerGetScarletCoreLevelEvent evt)
        {
            if (evt.message == "from_kaleidoscope") return;
            if (evt.player is ScarletPlayer scarlet &&
                HasAllThreeCoreLevels(scarlet))
            {
                evt.level += LEVEL_BONUS;
            }
        }

        public override void AddOnSelfEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, perm, effects);

            if (player is ScarletPlayer scarlet &&
                HasAllThreeCoreLevels(scarlet))
            {
                effects.Add(ScoreEffect.MulFan(FAN_MULTIPLIER, this));
            }
        }

        private bool HasAllThreeCoreLevels(ScarletPlayer scarlet)
        {
            return new[] { Category.Wan, Category.Suo, Category.Bing }.All(c =>
                scarlet.GetCategoryLevel(c, "from_kaleidoscope") >= 1);
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN_MULTIPLIER, LEVEL_BONUS);
        }
    }
}