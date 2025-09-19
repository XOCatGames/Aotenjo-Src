using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class VoidSteleArtifact : Artifact
    {
        private const float MUL = 0.3f;

        public VoidSteleArtifact() : base("void_stele", Rarity.EPIC)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL, 1 + MUL * GetUniqueEpicYakuPlayed(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            int count = GetUniqueEpicYakuPlayed(player);

            if (count > 0)
            {
                effects.Add(ScoreEffect.MulFan(1 + count * MUL, this));
            }
        }

        private int GetUniqueEpicYakuPlayed(Player player)
        {
            PlayerStats stats = player.stats;
            HashSet<YakuType> validTypesPlayed = new();
            foreach (var validRec in stats.GetPlayedHands())
            {
                foreach (var activatedYaku in validRec.activatedYakuTypes.Where(y =>
                             YakuTester.InfoMap[y].rarity >= Rarity.EPIC))
                {
                    if (YakuTester.InfoMap[activatedYaku].growthFactor == 1
                        || validRec.AllTiles.Count >= 14)
                        validTypesPlayed.Add(activatedYaku);
                }
            }

            return validTypesPlayed.Count;
        }
    }
}