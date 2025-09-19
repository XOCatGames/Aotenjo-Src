using System;

namespace Aotenjo
{
    public class TileFontColorless : TileFont
    {
        private const double DEC_FU = 3;

        public TileFontColorless() : base(6, "colorless", ScoreEffect.AddFu(-DEC_FU, null))
        {
        }

        public override bool IsDebuff()
        {
            return true;
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), DEC_FU);
        }
    }
}