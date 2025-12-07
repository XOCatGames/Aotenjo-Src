using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class IceAxeArtifact : CountPairArtifact
    {
        private const int ADD_FU = 40;

        public IceAxeArtifact() : base("ice_axe", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), ADD_FU);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            effects.Add(new SilentEffect(() =>
            {
                foreach (var (b1, b2) in GetPairsWithDiff(permutation, 1, player))
                {
                    player.playHandEffectStack.Push(ScoreEffect.AddFu(ADD_FU, this).OnMultipleTiles(b1.tiles.Union(b2.tiles).ToList(), b1.tiles[1]));
                }
            }));
        }
    }
}