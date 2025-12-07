using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CrystalMirrorArtifact : CountPairArtifact
    {
        private static readonly int FAN = 12;

        public CrystalMirrorArtifact() : base("crystal_mirror", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            effects.Add(new SilentEffect(() =>
            {
                foreach (var (b1, b2) in Utils.FindPairs(permutation,
                             (b1, b2) => player.DetermineShiftedPair(b1, b2, 0, false)))
                {
                    player.playHandEffectStack.Push(ScoreEffect.AddFan(FAN, this).OnMultipleTiles(b1.tiles.Union(b2.tiles).ToList(), b1.tiles[1]));
                }
            }));
        }
    }
}