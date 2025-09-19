using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BodhiSeedArtifact : Artifact
    {
        private const double MUL = 1.2;

        public BodhiSeedArtifact() : base("bodhi_seed", Rarity.EPIC)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL);
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);

            if (tile.CompactWithMaterial(TileMaterial.BonePorcelain(), player))
            {
                if (perm.GetYakus(player).Any(yaku =>
                        YakuTester.InfoMap[yaku].rarity >= Rarity.EPIC &&
                        player.GetSkillSet().GetLevel(yaku) >= 1)) return;
                effects.Add(ScoreEffect.MulFan(MUL, this));
            }
        }
    }
}