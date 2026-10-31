using System;
using System.Collections.Generic;
using System.Globalization;

namespace Aotenjo
{
    public class SilverLockArtifact : BaseLockArtifact
    {
        private const double FAN_MULTIPLIER = 3;
        private const int REQUIRED_ACTIVATIONS = 48;
        private const int UPGRADE_COUNT = 1;
        protected override double FanMultiplier => FAN_MULTIPLIER;
        protected override int TaskTarget => REQUIRED_ACTIVATIONS;
        protected override string NextLockId => Artifacts.GoldenLock.Id;
        protected override int ShopCircle => 2;

        public SilverLockArtifact() : base("silver_lock", Rarity.RARE) { }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            YakuType[] activated = GetActivatedCommonYakus(player, permutation);
            if (activated.Length == 0) return;
            for (int i = 0; i < UPGRADE_COUNT; i++)
                AddUpgradeEffect(effects, new[] { activated[player.GenerateRandomInt(activated.Length, "silver_lock")] });
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer),
                FAN_MULTIPLIER, Math.Max(0, REQUIRED_ACTIVATIONS - Level), REQUIRED_ACTIVATIONS, UPGRADE_LEVELS, UPGRADE_COUNT);
    }
}
