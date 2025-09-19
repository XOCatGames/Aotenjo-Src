using System;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialGolden : TileMaterial
    {
        private const int MONEY = 1;
        private const int FAN = 0;

        public TileMaterialGolden(int ID) : base(ID, "golden", null)
        {
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer("tile_golden_material_description"), FAN, MONEY);
        }

        public override Effect[] GetEffects()
        {
            Effect[] baseEffects = base.GetEffects();
            Array.Resize(ref baseEffects, baseEffects.Length + 1);
            baseEffects[^1] = new EarnMoneyEffect(MONEY);
            return baseEffects;
        }
    }
}