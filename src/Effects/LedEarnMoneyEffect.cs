using System;

namespace Aotenjo
{
    public enum LedDetectedColor { Red, Green, Blue, Black, Faded }

    /// <summary>One LED payout, with the color and installed slot captured when effects are built.</summary>
    [Serializable]
    public sealed class LedEarnMoneyEffect : EarnMoneyEffect
    {
        public readonly LedDetectedColor detectedColor;
        public readonly int partSlot;

        public LedEarnMoneyEffect(int amount, LedDetectedColor detectedColor, int partSlot) : base(amount)
        {
            this.detectedColor = detectedColor;
            this.partSlot = partSlot;
        }
    }
}
