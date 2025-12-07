using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class NylonRopeArtifact : CountPairArtifact
    {
        private const double MUL_FAN = 1.5;

        public NylonRopeArtifact() : base("nylon_rope", Rarity.EPIC)
        {
        }

        public override string GetDescription(Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), MUL_FAN);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            effects.Add(new SilentEffect(() =>
            {
                foreach (var (b1, b2) in GetPairsWithDiff(permutation, 1, player))
                {
                    player.playHandEffectStack.Push(ScoreEffect.MulFan(MUL_FAN, this).OnMultipleTiles(b1.tiles.Union(b2.tiles).ToList(), b1.tiles[1]));
                }
            }));
        }
    }
}