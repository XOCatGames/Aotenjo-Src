using System.Collections.Generic;

namespace Aotenjo
{
    public class HeatJadeArtifact : SneakyArtifact
    {
        public HeatJadeArtifact() : base("heat_jade", Rarity.COMMON)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if (player is SneakyPlayer p)
            {
                foreach (var item in p.sneakedTiles)
                {
                    effects.Add(new GrowFuEffect(this, item, 5, "heat_jade").OnTile(item));
                }
            }
        }
    }
}