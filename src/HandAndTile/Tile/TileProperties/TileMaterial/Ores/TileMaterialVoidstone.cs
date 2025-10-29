using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialVoidstone : TileMaterial
    {
        private const float MUL = 0.15f;

        public TileMaterialVoidstone(int ID) : base(ID, "voidstone", null)
        {
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer("tile_voidstone_material_description"), MUL);
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        public override Effect[] GetEffects(Player player, Permutation perm)
        {
            Effect[] baseEffects = base.GetEffects();

            int count = GetCount(perm, player);

            Array.Resize(ref baseEffects, baseEffects.Length + 1);

            baseEffects[baseEffects.Length - 1] = ScoreEffect.MulFan(1 + MUL * count, null);

            return baseEffects;
        }

        private static int GetCount(Permutation perm, Player player)
        {
            return perm.ToTiles().Where(t => player.DetermineMaterialCompatibility(t, PLAIN)).Count();
        }
    }
}