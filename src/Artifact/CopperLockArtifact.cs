using System;
using System.Globalization;

namespace Aotenjo
{
    public class CopperLockArtifact : BaseLockArtifact
    {
        private const double FAN_MULTIPLIER = 1.5;
        private const int REQUIRED_ACTIVATIONS = 36;
        protected override double FanMultiplier => FAN_MULTIPLIER;
        protected override int TaskTarget => REQUIRED_ACTIVATIONS;
        protected override string NextLockId => Artifacts.SilverLock.Id;
        protected override int ShopCircle => 1;

        public CopperLockArtifact() : base("copper_lock", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer),
                FAN_MULTIPLIER, Math.Max(0, REQUIRED_ACTIVATIONS - Level), REQUIRED_ACTIVATIONS);
    }
}
