using System;
using System.Globalization;

namespace Aotenjo
{
    public class CopperLockArtifact : BaseLockArtifact
    {
        private const double FAN_MULTIPLIER = 2;
        private const int REQUIRED_ACTIVATIONS = 24;
        protected override double FanMultiplier => FAN_MULTIPLIER;
        protected override int TaskTarget => REQUIRED_ACTIVATIONS;
        protected override BaseLockArtifact NextLock => Artifacts.SilverLock;
        protected override int ShopCircle => 1;

        public CopperLockArtifact() : base("copper_lock", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), FAN_MULTIPLIER, Level, REQUIRED_ACTIVATIONS);
    }
}
