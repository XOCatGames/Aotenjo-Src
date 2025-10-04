using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TrinityForceArtifact : CraftableArtifact
    {
        public TrinityForceArtifact() : base("trinity_force", Rarity.EPIC)
        {
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            if (permutation.ToTiles().Where(t => t.IsNumbered()).Select(t => t.GetOrder()).Distinct().Count() == 3)
            {
                effects.Add(ScoreEffect.MulFan(3, this));
            }
        }
    }
}