using System;
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
        protected override BaseLockArtifact NextLock => Artifacts.GoldenLock;
        protected override int ShopCircle => 2;

        public SilverLockArtifact() : base("silver_lock", Rarity.RARE) { }

        protected override void RewardActivatedYakus(Player player, YakuType[] activated)
        {
            for (int i = 0; i < UPGRADE_COUNT; i++)
                UpgradeYakus(player, new[] { activated[player.GenerateRandomInt(activated.Length, "silver_lock")] });
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer),
                FAN_MULTIPLIER, Level, REQUIRED_ACTIVATIONS, UPGRADE_LEVELS, UPGRADE_COUNT);
    }
}
