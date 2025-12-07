using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CopperMirrorArtifact : CountPairArtifact
    {
        private static readonly int FU = 40;

        public CopperMirrorArtifact() : base("copper_mirror", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            effects.Add(new SilentEffect(() =>
            {
                foreach (var (b1, b2) in GetPairsWithDiff(permutation, 0, player))
                {
                    player.playHandEffectStack.Push(ScoreEffect.AddFu(FU, this).OnMultipleTiles(b1.tiles.Union(b2.tiles).ToList(), b1.tiles[1]));
                }
            }));
        }
    }
}