using System;
using System.Globalization;

namespace Aotenjo
{
    public class GoldenLockArtifact : BaseLockArtifact
    {
        private const double FAN_MULTIPLIER = 3;
        protected override double FanMultiplier => FAN_MULTIPLIER;
        protected override int ShopCircle => 3;

        public GoldenLockArtifact(): base("golden_lock", Rarity.EPIC)
        {
        }

        protected override void RewardActivatedYakus(Player player, YakuType[] activated) =>
            UpgradeYakus(player, activated);

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), FAN_MULTIPLIER, UPGRADE_LEVELS);
    }
}
