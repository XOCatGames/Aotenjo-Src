using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialFamilleVertePorcelain : TileMaterialPorcelain
    {
        private const double MULT_PER_SAME_KIND = 0.25D;

        public TileMaterialFamilleVertePorcelain(int ID) : base(ID, "famille_verte_porcelain")
        {
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), MULT_PER_SAME_KIND, GetMult(player));
        }

        private double GetMult(Player player)
        {
            List<Tile> unused = player.GetUnusedTilesInHand();
            if (player.GetArtifacts().Contains(Artifacts.PorcelainSword) && player.GetCurrentSelectedPerm() != null)
                unused.AddRange(player.GetCurrentSelectedPerm().ToTiles());
            int diffTypeCount = unused.Where(t => t.CompatWithMaterial(FAMILLE_VERTE_PORCELAIN, player))
                .Select(t => t.GetCategory()).ToHashSet().Count();
            return 1D + diffTypeCount * MULT_PER_SAME_KIND;
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            effects.Add(ScoreEffect.MulFan(GetMult(player), null));
        }
    }
}